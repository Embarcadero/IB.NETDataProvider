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
using System.Data;
using System.Data.Common;
using System.Linq;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Logging;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Scaffolding.Internal;

public class IBDatabaseModelFactory : DatabaseModelFactory
{
	public int MajorVersionNumber { get; private set; }

	public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
	{
		using (var connection = new IBConnection(connectionString))
		{
			return Create(connection, options);
		}
	}

	public override DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
	{
		var databaseModel = new DatabaseModel();

		var connectionStartedOpen = connection.State == ConnectionState.Open;
		if (!connectionStartedOpen)
		{
			connection.Open();
		}

		var serverVersion = IBServerProperties.ParseServerVersion(connection.ServerVersion);
		MajorVersionNumber = serverVersion.Major;

		try
		{
			databaseModel.DatabaseName = connection.Database;
			databaseModel.DefaultSchema = GetDefaultSchema(connection);

			var schemaList = new List<string>();
			var tableList = options.Tables.ToList();
			var tableFilter = GenerateTableFilter(tableList, schemaList);

			var tables = GetTables(connection, tableFilter);
			foreach (var table in tables)
			{
				table.Database = databaseModel;
				if (tableFilter.Invoke(table))
				{
					databaseModel.Tables.Add(table);
				}
			}

			return databaseModel;
		}
		finally
		{
			if (!connectionStartedOpen)
			{
				connection.Close();
			}
		}
	}

	private static string GetDefaultSchema(DbConnection connection)
	{
		return null;
	}

	private static Func<DatabaseTable, bool> GenerateTableFilter(IReadOnlyList<string> tables, IReadOnlyList<string> schemas)
	{
		return tables.Any() ? x => tables.Contains(x.Name) : _ => true;
	}

	private const string GetTablesQuery =
		@"SELECT
                r.RDB$RELATION_NAME,
                r.RDB$DESCRIPTION,
                r.RDB$RELATION_TYPE
              FROM
               RDB$RELATIONS r
             WHERE
              COALESCE(r.RDB$SYSTEM_FLAG, 0) <> 1
             ORDER BY
              r.RDB$RELATION_NAME";

	private IEnumerable<DatabaseTable> GetTables(DbConnection connection, Func<DatabaseTable, bool> filter)
	{
		using (var command = connection.CreateCommand())
		{
			var tables = new List<DatabaseTable>();
			command.CommandText = GetTablesQuery;
			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					var name = reader.GetString(0).Trim();
					var comment = reader.GetString(1);
					var type = reader.GetString(2).Trim();

					var table = type == "VIEW"
						? new DatabaseView()
						: new DatabaseTable();

					table.Schema = null;
					table.Name = name;
					table.Comment = string.IsNullOrEmpty(comment) ? null : comment;

					tables.Add(table);
				}
			}

			GetColumns(connection, tables, filter);
			GetPrimaryKeys(connection, tables);
			GetIndexes(connection, tables, filter);
			GetConstraints(connection, tables);

			return tables;
		}
	}

	// InterBase doesn't accept the Fb SQL continue to manage the older way
	private string GetStoreType(int field_type, int size, int sub_type, int precision, int scale)
	{
		string result = "";
		var absscale = Math.Abs(scale);
		switch (field_type)
		{
			case 7:
				switch (sub_type)
				{
					case 0 : return "SMALLINT";
					case 1 : return $"NUMERIC({precision}, {Math.Abs(scale)})";
					case 2 : return $"DECIMAL({precision}, {Math.Abs(scale)})";
					default : return "?";
				}
			case 8:
				switch (sub_type)
				{
					case 0: return "INTEGER";
					case 1: return $"NUMERIC({precision}, {Math.Abs(scale)})";
					case 2: return $"DECIMAL({precision}, {Math.Abs(scale)})";
					default: return "?";
				}
			case 9:
				result = "QUAD";
				break;
			case 10:
				result = "FLOAT";
				break;
			case 12:
				result = "DATE";
				break;
			case 13:
				result = "TIME";
				break;
			case 14:
				result = "CHAR(" + size.ToString() + ")";
				break;
			case 16:
				switch (sub_type)
				{
					case 0: return "NUMERIC(18, 0)";
					case 1: return $"NUMERIC({precision}, {Math.Abs(scale)})";
					case 2: return $"DECIMAL({precision}, {Math.Abs(scale)})";
					default: return "?";
				}
			case 17:
				result = "BOOLEAN";
				break;
			case 27:
				result = "DOUBLE PRECISION";
				break;
			case 35:
				result = "TIMESTAMP";
				break;
			case 37:
				result = "VARCHAR(" + size.ToString() + ")";
				break;
			case 40:
				result = "CSTRING(" + size.ToString() + ")";
				break;
			case 45:
				result = "BLOB_ID";
				break;
			case 261:
				result = "BLOB SUB_TYPE ";
				switch (sub_type)
				{
					case 0:
						result += "BINARY";
						break;
					case 1:
						result += "TEXT";
						break;
					default:
						result += sub_type.ToString();
						break;
				}
				break;
			default:
				result = "";
				break;
		}
		return result;
	}

	private const string GetColumnsQuery =
	   @"SELECT RF.RDB$FIELD_NAME as COLUMN_NAME,
       COALESCE(RF.RDB$DEFAULT_SOURCE, F.RDB$DEFAULT_SOURCE) as COLUMN_DEFAULT,
       COALESCE(RF.RDB$NULL_FLAG, F.RDB$NULL_FLAG, 0)  as NOT_NULL,
        F.rdb$description as COLUMN_COMMENT, 0 as AUTO_GENERATED,
        ch.RDB$CHARACTER_SET_NAME as CHARACTER_SET_NAME,
        Coalesce(F.RDB$FIELD_TYPE, 0) FIELD_TYPE, 
        case Coalesce(F.RDB$FIELD_TYPE, 0)
          when 14 then cast((F.RDB$FIELD_LENGTH / CH.RDB$BYTES_PER_CHARACTER) as smallint)
          when 37 then cast((F.RDB$FIELD_LENGTH / CH.RDB$BYTES_PER_CHARACTER) as smallint)
          when 40 then cast((F.RDB$FIELD_LENGTH / CH.RDB$BYTES_PER_CHARACTER) as smallint)
          else 0
        end as FIELD_SIZE, coalesce(F.RDB$FIELD_SUB_TYPE, 0) as FIELD_SUB_TYPE,
        CASE WHEN f.rdb$computed_blr is null THEN false ELSE true END  COMPUTED_SOURCE,
        RDB$FIELD_SCALE FIELD_SCALE, COALESCE({1}, 0) FIELD_PRECISION
  FROM RDB$RELATION_FIELDS RF JOIN  RDB$FIELDS F ON
       F.RDB$FIELD_NAME = RF.RDB$FIELD_SOURCE LEFT OUTER JOIN RDB$CHARACTER_SETS CH ON
       CH.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID
         WHERE RF.RDB$RELATION_NAME = '{0}'
               AND COALESCE(RF.RDB$SYSTEM_FLAG, 0) = 0
             ORDER BY
              RF.RDB$FIELD_POSITION";

	private void GetColumns(DbConnection connection, IReadOnlyList<DatabaseTable> tables, Func<DatabaseTable, bool> tableFilter)
	{
		foreach (var table in tables)
		{
			using (var command = connection.CreateCommand())
			{
				var precisionField = ((IBConnection)connection).DBSQLDialect == 3 ? "RDB$FIELD_PRECISION" : "0";
				command.CommandText = string.Format(GetColumnsQuery, table.Name, precisionField);
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var name = reader["COLUMN_NAME"].ToString().Trim();
						var defaultValue = reader["COLUMN_DEFAULT"].ToString().Trim();
						var nullable = !Convert.ToBoolean(reader["NOT_NULL"]);
						var storeType = GetStoreType(Convert.ToInt32(reader["FIELD_TYPE"]),
													 Convert.ToInt32(reader["FIELD_SIZE"]),
													 Convert.ToInt32(reader["FIELD_SUB_TYPE"]),
													 Convert.ToInt32(reader["FIELD_PRECISION"]),
													 Convert.ToInt32(reader["FIELD_SCALE"]));
						var charset = reader["CHARACTER_SET_NAME"].ToString().Trim();
						var comment = reader["COLUMN_COMMENT"].ToString().Trim();

						var is_computed = Convert.ToBoolean(reader["COMPUTED_SOURCE"]);

						var valueGenerated = ValueGenerated.Never;

						if (is_computed)

						{
							valueGenerated = ValueGenerated.OnAddOrUpdate;
						}

						var column = new DatabaseColumn
						{
							Table = table,
							Name = name,
							StoreType = storeType,
							IsNullable = nullable,
							DefaultValueSql = CreateDefaultValueString(defaultValue),
							ValueGenerated = valueGenerated,
							Comment = string.IsNullOrEmpty(comment) ? null : comment,
						};

						table.Columns.Add(column);
					}
				}
			}
		}
	}

	private string CreateDefaultValueString(string defaultValue)
	{
		if (defaultValue == null)
		{
			return null;
		}
		if (defaultValue.StartsWith("default "))
		{
			return defaultValue.Remove(0, 8);
		}
		else
		{
			return null;
		}

	}

	private const string GetPrimaryQuery =
	@"SELECT
           i.rdb$index_name as INDEX_NAME,
           sg.rdb$field_name as FIELD_NAME
          FROM
           RDB$INDICES i
           LEFT JOIN rdb$index_segments sg on i.rdb$index_name = sg.rdb$index_name
           LEFT JOIN rdb$relation_constraints rc on rc.rdb$index_name = I.rdb$index_name
          WHERE
           rc.rdb$constraint_type = 'PRIMARY KEY'
           AND i.rdb$relation_name = '{0}'
          ORDER BY sg.RDB$FIELD_POSITION";

	private void GetPrimaryKeys(DbConnection connection, IReadOnlyList<DatabaseTable> tables)
	{
		foreach (var x in tables)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = string.Format(GetPrimaryQuery, x.Name);

				using (var reader = command.ExecuteReader())
				{
					DatabasePrimaryKey index = null;
					while (reader.Read())
					{
						if (index == null)
						{
							index = new DatabasePrimaryKey
							{
								Table = x,
								Name = reader.GetString(0).Trim()
							};
						}
						index.Columns.Add(x.Columns.Single(y => y.Name == reader.GetString(1).Trim()));
						x.PrimaryKey = index;
					}
				}
			}
		}
	}

	// InterBase does not have List() internal function so keep it this way
	private const string GetIndexesQuery =
		@"SELECT DISTINCT I.rdb$index_name as INDEX_NAME,
                     COALESCE(I.rdb$unique_flag, 0) as IS_UNIQUE,
                     Coalesce(I.rdb$index_type, 0) as IS_DESC
                FROM RDB$INDICES i LEFT JOIN rdb$index_segments sg on
                     i.rdb$index_name = sg.rdb$index_name LEFT JOIN rdb$relation_constraints rc on 
		      	     rc.rdb$index_name = I.rdb$index_name
               WHERE i.rdb$relation_name = '{0}' and
                     rc.rdb$constraint_type is null
               ORDER BY INDEX_NAME, IS_UNIQUE";

	// rdb$index_name maps to INDEX_NAME from above
	private const string GetIndexFields =
		@"SELECT rc.RDB$FIELD_NAME as COLUMNS
                FROM rdb$INDEX_SEGMENTS rc
               WHERE rc.rdb$index_name = '{0}'";

	/// <remarks>
	/// Primary keys are handled as in <see cref="GetConstraints"/>, not here
	/// </remarks>
	private void GetIndexes(DbConnection connection, IReadOnlyList<DatabaseTable> tables, Func<DatabaseTable, bool> tableFilter)
	{
		foreach (var table in tables)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = string.Format(GetIndexesQuery, table.Name);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var index = new DatabaseIndex
						{
							Table = table,
							Name = reader.GetString(0).Trim(),
							IsUnique = reader.GetBoolean(1),
						};
						using (var cCommand = connection.CreateCommand())
						{
							cCommand.CommandText = string.Format(GetIndexFields, index.Name);
							using (var fReader = cCommand.ExecuteReader())
							{
								while (fReader.Read())
								{
									index.Columns.Add(table.Columns.Single(y => y.Name == fReader.GetString(0).Trim()));
								}
							}
						}

						if (reader.GetBoolean(2))
						{
							var isDescending = new bool[index.Columns.Count];
							isDescending.AsSpan().Fill(true);
							index.IsDescending = isDescending;
						}

						table.Indexes.Add(index);
					}
				}
			}
		}
	}

	private const string GetConstraintsQuery =
		@"SELECT drs.rdb$constraint_name as CONSTRAINT_NAME, 
                 drs.RDB$RELATION_NAME as TABLE_NAME,
                 mrc.rdb$relation_name AS REFERENCED_TABLE_NAME, 
                 rc.RDB$DELETE_RULE as DELETE_RULE,
                 mrc.rdb$index_name mrc_index_name, drs.rdb$index_name drs_index_name,
                 drs.rdb$constraint_type CONSTRAINT_TYPE
            FROM rdb$relation_constraints drs left JOIN rdb$ref_constraints rc ON
                 drs.rdb$constraint_name = rc.rdb$constraint_name left JOIN rdb$relation_constraints mrc ON
                 rc.rdb$const_name_uq = mrc.rdb$constraint_name
           WHERE drs.rdb$constraint_type = 'FOREIGN KEY' AND
                 drs.RDB$RELATION_NAME = '{0}' ";

	// mi maps to mrc_index_name and di maps to drs_index_name
	private const string GetIndexSegments =
		@"select di.rdb$field_name || '|' || mi.rdb$field_name as PAIRED_COLUMNS
                from rdb$index_segments di join rdb$index_segments mi on
                     mi.RDB$FIELD_POSITION = di.RDB$FIELD_POSITION
               where (mi.rdb$index_name = '{0}' or mi.rdb$index_name is null) and
                     di.rdb$index_name = '{1}'";

	private void GetConstraints(DbConnection connection, IReadOnlyList<DatabaseTable> tables)
	{
		foreach (var table in tables)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = string.Format(GetConstraintsQuery, table.Name);
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var referencedTableName = reader.GetString(2).Trim();
						var referencedTable = tables.First(t => t.Name == referencedTableName);
						var fkInfo = new DatabaseForeignKey { Name = reader.GetString(0).Trim(), OnDelete = ConvertToReferentialAction(reader.GetString(3).Trim()), Table = table, PrincipalTable = referencedTable };

						using (var cCommand = connection.CreateCommand())
						{
							cCommand.CommandText = string.Format(GetIndexSegments, reader.GetString(4).Trim(), reader.GetString(5).Trim());
							using (var fReader = cCommand.ExecuteReader())
							{
								while (fReader.Read())
								{
									fkInfo.Columns.Add(table.Columns.Single(y =>
											string.Equals(y.Name, fReader.GetString(0).Split('|')[0].Trim(), StringComparison.OrdinalIgnoreCase)));
									fkInfo.PrincipalColumns.Add(fkInfo.PrincipalTable.Columns.Single(y =>
											string.Equals(y.Name, fReader.GetString(0).Split('|')[1].Trim(), StringComparison.OrdinalIgnoreCase)));
								}
							}
						}
						table.ForeignKeys.Add(fkInfo);
					}
				}
			}
		}
	}

	private static ReferentialAction? ConvertToReferentialAction(string onDeleteAction)
	{
		return onDeleteAction.ToUpperInvariant() switch
		{
			"RESTRICT" => ReferentialAction.Restrict,
			"CASCADE" => ReferentialAction.Cascade,
			"SET NULL" => ReferentialAction.SetNull,
			"NO ACTION" => ReferentialAction.NoAction,
			_ => null,
		};
	}
}