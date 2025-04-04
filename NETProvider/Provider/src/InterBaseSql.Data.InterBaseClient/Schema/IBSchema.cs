﻿/*
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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.Services;

namespace InterBaseSql.Data.Schema;

internal abstract class IBSchema
{
	#region Abstract Methods

	protected abstract StringBuilder GetCommandText(string[] restrictions);

	#endregion

	#region Methods

	public DataTable GetSchema(IBConnection connection, string collectionName, string[] restrictions)
	{
		Dialect = connection.DBSQLDialect;
		var dataTable = new DataTable(collectionName);
		var command = BuildCommand(connection, collectionName, ParseRestrictions(restrictions));
		try
		{
			using (var adapter = new IBDataAdapter(command))
			{
				try
				{
					adapter.Fill(dataTable);
				}
				catch (Exception ex)
				{
					throw IBException.Create(ex);
				}
			}
		}
		finally
		{
			command.Dispose();
		}
		TrimStringFields(dataTable);
		ProcessResult(dataTable);
		return dataTable;
	}
	public async Task<DataTable> GetSchemaAsync(IBConnection connection, string collectionName, string[] restrictions, CancellationToken cancellationToken = default)
	{
		Dialect = connection.DBSQLDialect;
		var dataTable = new DataTable(collectionName);
		var command = BuildCommand(connection, collectionName, ParseRestrictions(restrictions));
		try
		{
			using (var adapter = new IBDataAdapter(command))
			{
				try
				{
					adapter.Fill(dataTable);
				}
				catch (Exception ex)
				{
					throw IBException.Create(ex);
				}
			}
		}
		finally
		{
#if NET48 || NETSTANDARD2_0
			command.Dispose();
			await Task.CompletedTask.ConfigureAwait(false);
#else
			await command.DisposeAsync().ConfigureAwait(false);
#endif
		}
		TrimStringFields(dataTable);
		ProcessResult(dataTable);
		return dataTable;
	}

	#endregion

	#region Protected Methods

	protected IBCommand BuildCommand(IBConnection connection, string collectionName, string[] restrictions)
	{
		SetMajorVersionNumber(connection);
		var filter = string.Format("CollectionName='{0}'", collectionName);
		var builder = GetCommandText(restrictions);
		var restriction = connection.GetSchema(DbMetaDataCollectionNames.Restrictions).Select(filter);
		var transaction = connection.InnerConnection.ActiveTransaction;
		var command = new IBCommand(builder.ToString(), connection, transaction);
		Dialect = connection.DBSQLDialect;

		if (restrictions != null && restrictions.Length > 0)
		{
			var index = 0;

			for (var i = 0; i < restrictions.Length; i++)
			{
				var rname = restriction[i]["RestrictionName"].ToString();
				if (restrictions[i] != null)
				{
					// Catalog, Schema and TableType are no real restrictions
					if (!rname.EndsWith("Catalog") && !rname.EndsWith("Schema") && rname != "TableType")
					{
						var pname = string.Format("@p{0}", index++);

						command.Parameters.Add(pname, IBDbType.VarChar, 255).Value = restrictions[i];
					}
				}
			}
		}

		return command;
	}

	protected virtual void ProcessResult(DataTable schema)
	{ }

	protected virtual string[] ParseRestrictions(string[] restrictions)
	{
		return restrictions;
	}

	#endregion

	#region Private Methods
	/// <summary>
	/// Determines the major version number from the Serverversion on the inner connection.
	/// </summary>
	/// <param name="connection">an open connection, which is used to determine the version number of the connected database server</param>
	private void SetMajorVersionNumber(IBConnection connection)
	{
		var serverVersion = IBServerProperties.ParseServerVersion(connection.ServerVersion);
		MajorVersionNumber = serverVersion.Major;
	}
	#endregion

	#region Private Static Methods

	private static void TrimStringFields(DataTable schema)
	{
		schema.BeginLoadData();

		foreach (DataRow row in schema.Rows)
		{
			for (var i = 0; i < schema.Columns.Count; i++)
			{
				if (!row.IsNull(schema.Columns[i]) &&
					schema.Columns[i].DataType == typeof(System.String))
				{
					row[schema.Columns[i]] = row[schema.Columns[i]].ToString().Trim();
				}
			}
		}

		schema.EndLoadData();
		schema.AcceptChanges();
	}

	#endregion

	#region Properties
	/// <summary>
	/// The major version of the connected InterBase server
	/// </summary>
	protected int MajorVersionNumber { get; private set; }
	protected short Dialect { get; set; }
	#endregion
}