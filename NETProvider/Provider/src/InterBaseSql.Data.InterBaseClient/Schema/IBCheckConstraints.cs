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
using System.Text;
using System.Globalization;

namespace InterBaseSql.Data.Schema;

internal class IBCheckConstraints : IBSchema
{
	#region Protected Methods

	protected override StringBuilder GetCommandText(string[] restrictions)
	{
		var sql = new StringBuilder();
		var where = new StringBuilder();

		sql.Append(
			@"SELECT
					null AS CONSTRAINT_CATALOG,
					null AS CONSTRAINT_SCHEMA,
					chk.rdb$constraint_name AS CONSTRAINT_NAME,
					trig.rdb$trigger_source AS CHECK_CLAUSULE,
				    trig.rdb$description AS DESCRIPTION
				FROM rdb$check_constraints chk
				    INNER JOIN rdb$triggers trig ON chk.rdb$trigger_name = trig.rdb$trigger_name");

		if (restrictions != null)
		{
			/* CONSTRAINT_CATALOG */
			if (restrictions.Length >= 1 && restrictions[0] != null)
			{
			}

			/* CONSTRAINT_SCHEMA */
			if (restrictions.Length >= 2 && restrictions[1] != null)
			{
			}

			/* CONSTRAINT_NAME */
			if (restrictions.Length >= 3 && restrictions[2] != null)
			{
				where.Append("chk.rdb$constraint_name = @p0");
			}
		}

		if (where.Length > 0)
		{
			sql.AppendFormat(" WHERE {0}", where.ToString());
		}

		sql.Append(" ORDER BY chk.rdb$constraint_name");

		return sql;
	}

	#endregion
}
