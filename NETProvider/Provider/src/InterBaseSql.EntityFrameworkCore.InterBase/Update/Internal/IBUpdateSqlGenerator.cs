/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/raw/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    The Initial Developer(s) of the Original Code are listed below.
 *    Portions created by Embarcadero are Copyright (C) Embarcadero.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Update.Internal;


public class IBUpdateSqlGenerator : UpdateSqlGenerator, IIBUpdateSqlGenerator
{
	public IBUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
		: base(dependencies)
	{ }

public override ResultSetMapping AppendInsertOperation(StringBuilder commandStringBuilder, IReadOnlyModificationCommand command, int commandPosition, out bool requiresTransaction)
	{
		var result = ResultSetMapping.NoResults;
		var name = command.TableName;
		var operations = command.ColumnModifications;
		var writeOperations = operations.Where(o => o.IsWrite).ToList();
		var readOperations = operations.Where(o => o.IsRead).ToList();
		var anyRead = readOperations.Any();
		AppendInsertCommandHeader(commandStringBuilder, name, null, writeOperations);
		AppendValuesHeader(commandStringBuilder, writeOperations);
		AppendValues(commandStringBuilder, name, null, writeOperations);
		//		if (anyRead)
		//		{
		//			commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
		//			var keyOperations = operations.Where(o => o.IsKey).ToList();

		//			result = AppendSelectCommand(commandStringBuilder, name, null, readOperations, keyOperations, commandPosition);
		//				throw new NotSupportedInInterBase("InterBase does not support returning values from an insert statement");
		//		}
		commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();

		requiresTransaction = true;
		return result;
	}

	public override ResultSetMapping AppendUpdateOperation(StringBuilder commandStringBuilder, IReadOnlyModificationCommand command, int commandPosition, out bool requiresTransaction)
	{
		var name = command.TableName;
		var operations = command.ColumnModifications;
		var writeOperations = operations.Where(o => o.IsWrite).ToList();
		var readOperations = operations.Where(o => o.IsRead).ToList();
		var conditionOperations = operations.Where(o => o.IsCondition).ToList();
		var anyRead = readOperations.Any();
		AppendUpdateCommandHeader(commandStringBuilder, name, null, writeOperations);
		AppendWhereClause(commandStringBuilder, conditionOperations);
		commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();

		//if (anyRead)
		//{
		//	var keyOperations = operations.Where(o => o.IsKey).ToList();

		//	return AppendSelectAffectedCommand(commandStringBuilder, name, null, readOperations, keyOperations, commandPosition);
		//}
		//		commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
		requiresTransaction = true;
		return ResultSetMapping.NoResults;
	}

	public override ResultSetMapping AppendDeleteOperation(StringBuilder commandStringBuilder, IReadOnlyModificationCommand command, int commandPosition, out bool requiresTransaction)
	{
		var name = command.TableName;
		var operations = command.ColumnModifications;
		var conditionOperations = operations.Where(o => o.IsCondition).ToList();
		var inputOperations = GenerateParameters(conditionOperations);
		AppendDeleteCommandHeader(commandStringBuilder, name, null);
		AppendWhereClause(commandStringBuilder, conditionOperations);
		commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
		requiresTransaction = true;
		return ResultSetMapping.LastInResultSet;
	}

	// workaround for GenerateBlockParameterName
	protected override void AppendUpdateCommandHeader(StringBuilder commandStringBuilder, string name, string schema, IReadOnlyList<IColumnModification> operations)
	{
		commandStringBuilder.Append("UPDATE ");
		SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, name, schema);
		commandStringBuilder.Append(" SET ")
			.AppendJoin(
				operations,
				SqlGenerationHelper,
				(sb, o, helper) =>
				{
					helper.DelimitIdentifier(sb, o.ColumnName);
					sb.Append(" = ");
					if (!o.UseCurrentValueParameter)
					{
						AppendSqlLiteral(sb, o.Value, o.Property);
					}
					else
					{
						((IIBSqlGenerationHelper)helper).GenerateBlockParameterName(sb, o.ParameterName);
					}
				});
	}

	// workaround for GenerateBlockParameterName
	protected override void AppendWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification, bool useOriginalValue)
	{
		SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, columnModification.ColumnName);
		if ((useOriginalValue ? columnModification.OriginalValue : columnModification.Value) == null)
		{
			commandStringBuilder.Append(" IS NULL");
			return;
		}

		commandStringBuilder.Append(" = ");
		if (!columnModification.UseCurrentValueParameter && !columnModification.UseOriginalValueParameter)
		{
			AppendSqlLiteral(commandStringBuilder, columnModification.Value, columnModification.Property);
		}
		else
		{
			((IIBSqlGenerationHelper)SqlGenerationHelper).GenerateBlockParameterName(commandStringBuilder, useOriginalValue ? columnModification.OriginalParameterName : columnModification.ParameterName);
		}
	}

	string GetColumnType(IColumnModification column)
	{
		return Dependencies.TypeMappingSource.GetMapping(column.Property).StoreType;
	}

	IEnumerable<(string name, string type)> GenerateParameters(IEnumerable<IColumnModification> columns)
	{
		foreach (var item in columns)
		{
			var type = GetColumnType(item);
			if (item.UseCurrentValueParameter)
			{
				yield return (item.ParameterName, type);
			}
			if (item.UseOriginalValueParameter)
			{
				yield return (item.OriginalParameterName, type);
			}
		}
	}

	public override string GenerateNextSequenceValueOperation(string name, string schema)
	{
		var builder = new StringBuilder();
		builder.Append("SELECT GEN_ID(");
		builder.Append(SqlGenerationHelper.DelimitIdentifier(name));
		builder.Append(", 1) FROM RDB$DATABASE");
		return builder.ToString();
	}

	/*override*/
	void AppendSqlLiteral(StringBuilder commandStringBuilder, object value, IProperty property)
	{
		var mapping = property != null
			? Dependencies.TypeMappingSource.FindMapping(property)
			: null;
		mapping ??= Dependencies.TypeMappingSource.GetMappingForValue(value);
		commandStringBuilder.Append(mapping.GenerateProviderValueSqlLiteral(value));
	}
}