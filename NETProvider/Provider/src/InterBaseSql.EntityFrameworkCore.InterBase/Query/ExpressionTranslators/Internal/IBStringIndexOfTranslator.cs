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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal;

public class IBStringIndexOfTranslator : IMethodCallTranslator
{
	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBStringIndexOfTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (method.DeclaringType == typeof(string) && method.Name == nameof(string.IndexOf))
		{
			var args = new List<SqlExpression>();
			args.Add(_ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]));
			args.Add(instance);
			foreach (var a in arguments.Skip(1))
			{
				args.Add(_ibSqlExpressionFactory.ApplyDefaultTypeMapping(a));
			}
			return _ibSqlExpressionFactory.Subtract(
				_ibSqlExpressionFactory.Function("EF_POSITION", args, true, Enumerable.Repeat(true, args.Count), typeof(int)),
				_ibSqlExpressionFactory.Constant(1));
		}
		return null;
	}
}
