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

public class IBStringContainsTranslator : IMethodCallTranslator
{
	static readonly MethodInfo MethodInfo = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBStringContainsTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!method.Equals(MethodInfo))
			return null;

		var patternExpression = _ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]);
		var positionExpression = _ibSqlExpressionFactory.GreaterThan(
			_ibSqlExpressionFactory.Function(
				"EF_POSITION",
				new[] { patternExpression, instance, _ibSqlExpressionFactory.Constant(0) },
				true,
				new[] { true, true },
				typeof(int)),
			_ibSqlExpressionFactory.Constant(0));
		var matchingExpression = patternExpression is SqlConstantExpression sqlConstantExpression
			? ((string)sqlConstantExpression.Value)?.Length == 0
				? (SqlExpression)_ibSqlExpressionFactory.Constant(true)
				: (SqlExpression)positionExpression
			: _ibSqlExpressionFactory.OrElse(
				positionExpression,
				_ibSqlExpressionFactory.Equal(
					_ibSqlExpressionFactory.Function("EF_LENGTH", new[] { patternExpression }, true, new[] { true }, typeof(int)),
					_ibSqlExpressionFactory.Constant(0)));
		return _ibSqlExpressionFactory.AndAlso(matchingExpression, _ibSqlExpressionFactory.AndAlso(_ibSqlExpressionFactory.IsNotNull(instance), _ibSqlExpressionFactory.IsNotNull(patternExpression)));
	}
}