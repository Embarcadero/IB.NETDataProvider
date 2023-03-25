/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
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
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal;

public class IBDateTimeDateComponentTranslator : IMemberTranslator
{
	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBDateTimeDateComponentTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (member.DeclaringType == typeof(DateTime) && member.Name == nameof(DateTime.Date))
		{
			return _ibSqlExpressionFactory.SpacedFunction(
				"CAST",
				new[] { instance, _ibSqlExpressionFactory.Fragment("AS"), _ibSqlExpressionFactory.Fragment("DATE") },
				true,
				new[] { true, false, false },
				typeof(DateTime));
		}
		return null;
	}
}
