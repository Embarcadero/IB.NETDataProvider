using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Storage;
using InterBaseSql.Data.InterBaseClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using InterBaseSql.Data.Isql;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Update.Internal;

class IBServerModificationCommandBatch : SingularModificationCommandBatch
{

	public IBServerModificationCommandBatch(ModificationCommandBatchFactoryDependencies dependencies)
		: base(dependencies) { }

	protected override int ConsumeResultSetWithoutPropagation(int commandIndex, Microsoft.EntityFrameworkCore.Storage.RelationalDataReader reader)
	{
		var expectedRowsAffected = 1;
		while (++commandIndex < CommandResultSet.Count
			&& CommandResultSet[commandIndex - 1] == ResultSetMapping.NotLastInResultSet)
		{
			Debug.Assert(!ModificationCommands[commandIndex].RequiresResultPropagation);

			expectedRowsAffected++;
		}
		using (var ibcommand = (IBCommand)reader.DbCommand)
		{
			// Inserts and Deletes will not have result sets.  In that case just check the records affected property
			if (!ibcommand.HasFields)
			{
				if(ibcommand.RecordsAffected != expectedRowsAffected)
				{
					ThrowAggregateUpdateConcurrencyException(commandIndex, expectedRowsAffected, ibcommand.RecordsAffected);
				}
			}
			else
			if (reader.Read())
			{
				var rowsAffected = ibcommand.RecordsAffected;
				if (rowsAffected != expectedRowsAffected)
				{
					ThrowAggregateUpdateConcurrencyException(commandIndex, expectedRowsAffected, rowsAffected);
				}
			}
			else
			{
				ThrowAggregateUpdateConcurrencyException(commandIndex, 1, 0);
			}
		}

		return commandIndex;
	}

	protected override int ConsumeResultSetWithPropagation(int commandIndex, RelationalDataReader reader)
	{
		var rowsAffected = 0;
		do
		{
			var tableModification = ModificationCommands[commandIndex];
			Debug.Assert(tableModification.RequiresResultPropagation);

			if (!reader.Read())
			{
				var expectedRowsAffected = rowsAffected + 1;
				while (++commandIndex < CommandResultSet.Count
					&& CommandResultSet[commandIndex - 1] == ResultSetMapping.NotLastInResultSet)
				{
					expectedRowsAffected++;
				}

				ThrowAggregateUpdateConcurrencyException(commandIndex, expectedRowsAffected, rowsAffected);
			}

			var valueBufferFactory = CreateValueBufferFactory(tableModification.ColumnModifications);

			tableModification.PropagateResults(valueBufferFactory.Create(reader.DbDataReader));
			rowsAffected++;
		}
		while (++commandIndex < CommandResultSet.Count
			&& CommandResultSet[commandIndex - 1] == ResultSetMapping.NotLastInResultSet);

		return commandIndex;
	}

	public override void Execute(IRelationalConnection connection)
	{
		var storeCommand = CreateStoreCommand();
		try
		{
			using (var dataReader = storeCommand.RelationalCommand.ExecuteReader(
				new RelationalCommandParameterObject(
					connection,
					storeCommand.ParameterValues,
					null,
					Dependencies.CurrentContext.Context,
					Dependencies.Logger)))
			{
				Consume(dataReader);
			}
		}
		catch (DbUpdateException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DbUpdateException(RelationalStrings.UpdateStoreException, ex);
		}
	}
}
