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

namespace InterBaseSql.Data.Schema;

internal class IBColumnPrivileges : IBSchema
{
	#region Protected Methods

	protected override StringBuilder GetCommandText(string[] restrictions)
	{
		var sql = new StringBuilder();
		var where = new StringBuilder();

		sql.Append(
			@"SELECT
					null AS TABLE_CATALOG,
					null AS TABLE_SCHEMA,
					rdb$relation_name AS TABLE_NAME,
					rdb$field_name AS COLUMN_NAME,
					rdb$user AS GRANTEE,
					rdb$grantor AS GRANTOR,
					rdb$privilege AS PRIVILEGE,
					rdb$grant_option AS WITH_GRANT
				FROM rdb$user_privileges");

		where.Append("rdb$object_type = 0");

		if (restrictions != null)
		{
			var index = 0;

			/* TABLE_CATALOG */
			if (restrictions.Length >= 1 && restrictions[0] != null)
			{
			}

			/* TABLE_SCHEMA */
			if (restrictions.Length >= 2 && restrictions[1] != null)
			{
			}

			/* TABLE_NAME */
			if (restrictions.Length >= 3 && restrictions[2] != null)
			{
				where.AppendFormat(" AND rdb$relation_name = @p{0}", index++);
			}

			/* COLUMN_NAME */
			if (restrictions.Length >= 4 && restrictions[3] != null)
			{
				where.AppendFormat(" AND rdb$field_name = @p{0}", index++);
			}

			/* GRANTOR */
			if (restrictions.Length >= 6 && restrictions[5] != null)
			{
				where.AppendFormat(" AND rdb$grantor = @p{0}", index++);
			}

			/* GRANTEE */
			if (restrictions.Length >= 5 && restrictions[4] != null)
			{
				where.AppendFormat(" AND rdb$user = @p{0}", index++);
			}
		}

		if (where.Length > 0)
		{
			sql.AppendFormat(" WHERE {0} ", where.ToString());
		}

		sql.Append(" ORDER BY rdb$relation_name");

		return sql;
	}

	#endregion
}
