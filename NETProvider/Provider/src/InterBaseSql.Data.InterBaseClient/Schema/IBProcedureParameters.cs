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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Schema;

internal class IBProcedureParameters : IBSchema
{
	#region Protected Methods
	static Dictionary<string, string> ColName = new Dictionary<string,string>();
	static Dictionary<string, string> ColName_legacy = new Dictionary<string, string>();

	protected override StringBuilder GetCommandText(string[] restrictions)
	{
		var sql = new StringBuilder();
		var where = new StringBuilder();

		sql.Append(
			@"SELECT
					null AS PROCEDURE_CATALOG,
					null AS PROCEDURE_SCHEMA,
					pp.rdb$procedure_name AS PROCEDURE_NAME,
					pp.rdb$parameter_name AS PARAMETER_NAME,
					null AS PARAMETER_DATA_TYPE,
					fld.rdb$field_sub_type AS PARAMETER_SUB_TYPE,
					pp.rdb$parameter_number AS ORDINAL_POSITION,
					CAST(pp.rdb$parameter_type AS integer) AS PARAMETER_DIRECTION,
					CAST(fld.rdb$field_length AS integer) AS PARAMETER_SIZE,
					CAST(fld.rdb$field_precision AS integer) AS NUMERIC_PRECISION,
					CAST(fld.rdb$field_scale AS integer) AS NUMERIC_SCALE,
					CAST(fld.rdb$character_length AS integer) AS CHARACTER_MAX_LENGTH,
					CAST(fld.rdb$field_length AS integer) AS CHARACTER_OCTET_LENGTH,
					fld.rdb$null_flag AS COLUMN_NULLABLE,
					null AS CHARACTER_SET_CATALOG,
					null AS CHARACTER_SET_SCHEMA,
					cs.rdb$character_set_name AS CHARACTER_SET_NAME,
					null AS COLLATION_CATALOG,
					null AS COLLATION_SCHEMA,
					coll.rdb$collation_name AS COLLATION_NAME,
					null AS COLLATION_CATALOG,
					null AS COLLATION_SCHEMA,
					pp.rdb$description AS DESCRIPTION,
					fld.rdb$field_type AS FIELD_TYPE
				FROM rdb$procedure_parameters pp
					LEFT JOIN rdb$fields fld ON pp.rdb$field_source = fld.rdb$field_name
					LEFT JOIN rdb$character_sets cs ON cs.rdb$character_set_id = fld.rdb$character_set_id
					LEFT JOIN rdb$collations coll ON (coll.rdb$collation_id = fld.rdb$collation_id AND coll.rdb$character_set_id = fld.rdb$character_set_id)");

		if (restrictions != null)
		{
			var index = 0;

			/* PROCEDURE_CATALOG */
			if (restrictions.Length >= 1 && restrictions[0] != null)
			{
			}

			/* PROCEDURE_SCHEMA	*/
			if (restrictions.Length >= 2 && restrictions[1] != null)
			{
			}

			/* PROCEDURE_NAME */
			if (restrictions.Length >= 3 && restrictions[2] != null)
			{
				where.AppendFormat("pp.rdb$procedure_name = @p{0}", index++);
			}

			/* PARAMETER_NAME */
			if (restrictions.Length >= 4 && restrictions[3] != null)
			{
				if (where.Length > 0)
				{
					where.Append(" AND ");
				}

				where.AppendFormat("pp.rdb$parameter_name = @p{0}", index++);
			}
		}

		if (where.Length > 0)
		{
			sql.AppendFormat(" WHERE {0} ", where.ToString());
		}

		sql.Append(" ORDER BY pp.rdb$procedure_name, pp.rdb$parameter_type, pp.rdb$parameter_number");

		return sql;
	}

	protected override void ProcessResult(DataTable schema)
	{
		schema.BeginLoadData();
		schema.Columns.Add("IS_NULLABLE", typeof(bool));
		if (IBDBXLegacyTypes.IncludeLegacySchemaType)
		{
			schema.Columns.Add("DbxDataType", typeof(int));
			schema.Columns.Add("IsFixedLength", typeof(bool) );
			schema.Columns.Add("IsUnicode", typeof(bool));
			schema.Columns.Add("IsLong", typeof(bool));
			schema.Columns.Add("IsUnsigned", typeof(bool));
			schema.Columns.Add("ParameterMode", typeof(string));
		}

		foreach (DataRow row in schema.Rows)
		{
			var blrType = Convert.ToInt32(row["FIELD_TYPE"], CultureInfo.InvariantCulture);

			var subType = 0;
			if (row["PARAMETER_SUB_TYPE"] != DBNull.Value)
			{
				subType = Convert.ToInt32(row["PARAMETER_SUB_TYPE"], CultureInfo.InvariantCulture);
			}

			var scale = 0;
			if (row["NUMERIC_SCALE"] != DBNull.Value)
			{
				scale = Convert.ToInt32(row["NUMERIC_SCALE"], CultureInfo.InvariantCulture);
			}

			row["IS_NULLABLE"] = (row["COLUMN_NULLABLE"] == DBNull.Value);

			var dbType = (IBDbType)TypeHelper.GetDbDataTypeFromBlrType(blrType, subType, scale);
			row["PARAMETER_DATA_TYPE"] = TypeHelper.GetDataTypeName((DbDataType)dbType).ToLowerInvariant();

			if (dbType == IBDbType.Char || dbType == IBDbType.VarChar)
			{
				row["PARAMETER_SIZE"] = row["CHARACTER_MAX_LENGTH"];
			}
			else
			{
				row["CHARACTER_OCTET_LENGTH"] = 0;
			}

			if (dbType == IBDbType.Binary || dbType == IBDbType.Text)
			{
				row["PARAMETER_SIZE"] = Int32.MaxValue;
			}

			if (row["NUMERIC_PRECISION"] == DBNull.Value)
			{
				row["NUMERIC_PRECISION"] = 0;
			}

			if ((dbType == IBDbType.Decimal || dbType == IBDbType.Numeric) &&
				(row["NUMERIC_PRECISION"] == DBNull.Value || Convert.ToInt32(row["NUMERIC_PRECISION"]) == 0))
			{
				row["NUMERIC_PRECISION"] = row["PARAMETER_SIZE"];
			}

			row["NUMERIC_SCALE"] = (-1) * scale;

			var direction = Convert.ToInt32(row["PARAMETER_DIRECTION"], CultureInfo.InvariantCulture);
			switch (direction)
			{
				case 0:
					row["PARAMETER_DIRECTION"] = ParameterDirection.Input;
					break;

				case 1:
					row["PARAMETER_DIRECTION"] = ParameterDirection.Output;
					break;
			}
			if (IBDBXLegacyTypes.IncludeLegacySchemaType)
			{
				switch (direction)
				{
					case 0:
						row["ParameterMode"] = "IN";
						break;

					case 1:
						row["ParameterMode"] = "OUT";
						break;
				}
				row["DbxDataType"] = IBDBXLegacyTypes.GetLegacyType(Dialect, IBDBXLegacyTypes.GetLegacyProviderType(dbType, subType, scale)); ;
				row["IsFixedLength"] = IBDBXLegacyTypes.FixedLength.Contains(dbType);
				row["IsUnicode"] = false;
				row["IsLong"] = false;
				row["IsUnsigned"] = IBDBXLegacyTypes.IsLong.Contains(dbType);
				row["ORDINAL_POSITION"] = (short)row["ORDINAL_POSITION"] + 1;
			}
		}

		schema.EndLoadData();
		schema.AcceptChanges();

		// Remove not more needed columns
		schema.Columns.Remove("COLUMN_NULLABLE");
		schema.Columns.Remove("FIELD_TYPE");
		schema.Columns.Remove("CHARACTER_MAX_LENGTH");
	}

	#endregion
}
