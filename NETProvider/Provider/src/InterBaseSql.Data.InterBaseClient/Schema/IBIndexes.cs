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
using System.Globalization;
using System.Text;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Schema;

internal class IBIndexes : IBSchema
{
	#region Protected Methods

	protected override StringBuilder GetCommandText(string[] restrictions)
	{
		var sql = new StringBuilder();
		var where = new StringBuilder();

		if (!IBDBXLegacyTypes.IncludeLegacySchemaType)
			sql.Append(
				@"SELECT
						null AS TABLE_CATALOG,
						null AS TABLE_SCHEMA,
						idx.rdb$relation_name AS TABLE_NAME,
						idx.rdb$index_name AS INDEX_NAME,
						idx.rdb$index_inactive AS IS_INACTIVE,
						idx.rdb$unique_flag AS IS_UNIQUE,
						(SELECT COUNT(*) FROM rdb$relation_constraints rel
						WHERE rel.rdb$constraint_type = 'PRIMARY KEY' AND rel.rdb$index_name = idx.rdb$index_name AND rel.rdb$relation_name = idx.rdb$relation_name) as PRIMARY_KEY,
						(SELECT COUNT(*) FROM rdb$relation_constraints rel
						WHERE rel.rdb$constraint_type = 'UNIQUE' AND rel.rdb$index_name = idx.rdb$index_name AND rel.rdb$relation_name = idx.rdb$relation_name) as UNIQUE_KEY,
						idx.rdb$system_flag AS IS_SYSTEM_INDEX,
						idx.rdb$index_type AS INDEX_TYPE,
						idx.rdb$description AS DESCRIPTION
				FROM rdb$indices idx");
		else
			sql.Append(
				@"SELECT
						  null AS TABLE_CATALOG,
						  null AS TABLE_SCHEMA,
						  idx.rdb$relation_name AS TABLE_NAME,
						  idx.rdb$index_name AS INDEX_NAME,
						  idx.rdb$index_inactive AS IS_INACTIVE,
						  idx.rdb$unique_flag AS IS_UNIQUE,
							(SELECT COUNT(*) FROM rdb$relation_constraints rel
							WHERE rel.rdb$constraint_type = 'PRIMARY KEY' AND rel.rdb$index_name = idx.rdb$index_name AND rel.rdb$relation_name = idx.rdb$relation_name) as PRIMARY_KEY,
						  (SELECT COUNT(*) FROM rdb$relation_constraints rel
						  WHERE rel.rdb$constraint_type = 'UNIQUE' AND rel.rdb$index_name = idx.rdb$index_name AND rel.rdb$relation_name = idx.rdb$relation_name) as UNIQUE_KEY,
						  idx.rdb$system_flag AS IS_SYSTEM_INDEX,
						  idx.rdb$index_type AS INDEX_TYPE,
						  idx.rdb$description AS DESCRIPTION ,
						  (select rdb$constraint_name from rdb$relation_constraints where rdb$relation_name = idx.rdb$relation_name and rdb$index_name = idx.rdb$index_name) as LEG_CONSTRAINT_NAME
						FROM rdb$indices idx");


		if (restrictions != null)
		{
			var index = 0;

			/* TABLE_CATALOG */
			if (restrictions.Length >= 1 && restrictions[0] != null)
			{
			}

			/* TABLE_SCHEMA	*/
			if (restrictions.Length >= 2 && restrictions[1] != null)
			{
			}

			/* TABLE_NAME */
			if (restrictions.Length >= 3 && restrictions[2] != null)
			{
				where.AppendFormat("idx.rdb$relation_name = @p{0}", index++);
			}

			/* INDEX_NAME */
			if (restrictions.Length >= 4 && restrictions[3] != null)
			{
				if (where.Length > 0)
				{
					where.Append(" AND ");
				}

				where.AppendFormat("idx.rdb$index_name = @p{0}", index++);
			}
		}

		if (where.Length > 0)
		{
			sql.AppendFormat(" WHERE {0} ", where.ToString());
		}

		sql.Append(" ORDER BY idx.rdb$relation_name, idx.rdb$index_name");

		return sql;
	}

	protected override void ProcessResult(DataTable schema)
	{
		schema.BeginLoadData();
		schema.Columns.Add("IS_PRIMARY", typeof(bool));
		if (IBDBXLegacyTypes.IncludeLegacySchemaType)
		{
			schema.Columns.Add("IsAscending", typeof(bool));
			schema.Columns.Add("IsUnique", typeof(bool));
		}

		foreach (DataRow row in schema.Rows)
		{
			row["IS_UNIQUE"] = !(row["IS_UNIQUE"] == DBNull.Value || Convert.ToInt32(row["IS_UNIQUE"], CultureInfo.InvariantCulture) == 0);

			row["IS_PRIMARY"] = !(row["PRIMARY_KEY"] == DBNull.Value || Convert.ToInt32(row["PRIMARY_KEY"], CultureInfo.InvariantCulture) == 0);

			row["IS_INACTIVE"] = !(row["IS_INACTIVE"] == DBNull.Value || Convert.ToInt32(row["IS_INACTIVE"], CultureInfo.InvariantCulture) == 0);

			row["IS_SYSTEM_INDEX"] = !(row["IS_SYSTEM_INDEX"] == DBNull.Value || Convert.ToInt32(row["IS_SYSTEM_INDEX"], CultureInfo.InvariantCulture) == 0);
			if (IBDBXLegacyTypes.IncludeLegacySchemaType)
			{
				row["IsAscending"] = !(row["INDEX_TYPE"] == DBNull.Value || Convert.ToInt32(row["INDEX_TYPE"], CultureInfo.InvariantCulture) != 0);
				row["IsUnique"] = !(row["IS_UNIQUE"] == DBNull.Value || Convert.ToInt32(row["IS_UNIQUE"], CultureInfo.InvariantCulture) == 0);
			}
		}

		schema.EndLoadData();
		schema.AcceptChanges();

		schema.Columns.Remove("PRIMARY_KEY");
	}

	#endregion
}
