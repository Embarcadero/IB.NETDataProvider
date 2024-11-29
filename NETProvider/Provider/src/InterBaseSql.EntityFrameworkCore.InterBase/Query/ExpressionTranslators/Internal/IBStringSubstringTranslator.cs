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

using System.Collections.Generic;
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal;

public class IBStringSubstringTranslator : IMethodCallTranslator
{
	static readonly MethodInfo SubstringOnlyStartMethod = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int) });
	static readonly MethodInfo SubstringStartAndLengthMethod = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });

	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBStringSubstringTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!(method.Equals(SubstringOnlyStartMethod) || method.Equals(SubstringStartAndLengthMethod)))
			return null;

		var fromExpression = _ibSqlExpressionFactory.ApplyDefaultTypeMapping(_ibSqlExpressionFactory.Add(arguments[0], _ibSqlExpressionFactory.Constant(1)));
		var forExpression = arguments.Count == 2 ? _ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]) : null;
		var substringArguments = forExpression != null
			? new[] { instance, _ibSqlExpressionFactory.Fragment(", "), fromExpression, _ibSqlExpressionFactory.Fragment(", "), forExpression }
			: new[] { instance, _ibSqlExpressionFactory.Fragment(", "), fromExpression, _ibSqlExpressionFactory.Fragment(", -1") };
		var nullability = forExpression != null
			? new[] { true, false, true, false, true }
			: new[] { true, false, true };
		return _ibSqlExpressionFactory.SpacedFunction(
			"EF_SUBSTR",
			substringArguments,
			true,
			nullability,
			typeof(string));

		//var fromExpression = _ibSqlExpressionFactory.Add(arguments[0], _ibSqlExpressionFactory.Constant(1));
		//var forExpression = arguments.Count == 2 ? arguments[1] : null;
		//return _ibSqlExpressionFactory.Substring(instance, fromExpression, forExpression);
	}
}