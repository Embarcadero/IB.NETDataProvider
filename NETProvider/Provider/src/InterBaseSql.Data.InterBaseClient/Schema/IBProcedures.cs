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

internal class IBProcedures : IBSchema
{
	#region Protected Methods

	protected override StringBuilder GetCommandText(string[] restrictions)
	{
		var sql = new StringBuilder();
		var where = new StringBuilder();

		sql.Append(
			@"SELECT
					null AS PROCEDURE_CATALOG,
					null AS PROCEDURE_SCHEMA,
					rdb$procedure_name AS PROCEDURE_NAME,
					rdb$procedure_inputs AS INPUTS,
					rdb$procedure_outputs AS OUTPUTS,
					rdb$system_flag AS IS_SYSTEM_PROCEDURE,
					rdb$procedure_source AS SOURCE,
					rdb$description AS DESCRIPTION
				FROM rdb$procedures");

		if (restrictions != null)
		{
			var index = 0;

			/* PROCEDURE_CATALOG */
			if (restrictions.Length >= 1 && restrictions[0] != null)
			{
			}

			/* PROCEDURE_SCHEMA */
			if (restrictions.Length >= 2 && restrictions[1] != null)
			{
			}

			/* PROCEDURE_NAME */
			if (restrictions.Length >= 3 && restrictions[2] != null)
			{
				where.AppendFormat("rdb$procedure_name = @p{0}", index++);
			}
		}

		if (where.Length > 0)
		{
			sql.AppendFormat(" WHERE {0} ", where.ToString());
		}

		sql.Append(" ORDER BY rdb$procedure_name");

		return sql;
	}

	protected override void ProcessResult(DataTable schema)
	{
		schema.BeginLoadData();
		if (IBDBXLegacyTypes.IncludeLegacySchemaType)
		{
			schema.Columns.Add("ProcedureType", typeof(string));
		}

		foreach (DataRow row in schema.Rows)
		{
			if (row["INPUTS"] == DBNull.Value)
			{
				row["INPUTS"] = 0;
			}
			if (row["OUTPUTS"] == DBNull.Value)
			{
				row["OUTPUTS"] = 0;
			}
			if (row["IS_SYSTEM_PROCEDURE"] == DBNull.Value ||
				Convert.ToInt32(row["IS_SYSTEM_PROCEDURE"], CultureInfo.InvariantCulture) == 0)
			{
				row["IS_SYSTEM_PROCEDURE"] = false;
			}
			else
			{
				row["IS_SYSTEM_PROCEDURE"] = true;
			}
			if (IBDBXLegacyTypes.IncludeLegacySchemaType)
			{
				row["ProcedureType"] = "PROCEDURE";
			}
		}

		schema.EndLoadData();
		schema.AcceptChanges();
	}

	#endregion
}
