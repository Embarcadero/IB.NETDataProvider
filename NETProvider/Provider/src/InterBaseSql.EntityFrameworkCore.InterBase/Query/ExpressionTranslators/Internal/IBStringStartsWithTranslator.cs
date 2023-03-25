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

public class FbStringStartsWithTranslator : IMethodCallTranslator
{
	static readonly MethodInfo StartsWithMethod = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });
	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public FbStringStartsWithTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!method.Equals(StartsWithMethod))
			return null;

		var patternExpression = _ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]);
		var patternConstantExpression = patternExpression as SqlConstantExpression;
		var likePatternExpression = patternConstantExpression != null
			? (SqlExpression)_ibSqlExpressionFactory.Constant(((string)patternConstantExpression.Value) + "%")
			: (SqlExpression)_ibSqlExpressionFactory.Add(patternExpression, _ibSqlExpressionFactory.Constant("%"));
		var startsWithExpression = _ibSqlExpressionFactory.AndAlso(
			_ibSqlExpressionFactory.Like(
				instance,
				likePatternExpression),
			_ibSqlExpressionFactory.Equal(
				_ibSqlExpressionFactory.ApplyDefaultTypeMapping(_ibSqlExpressionFactory.Function(
					"LEFT",
					new[] {
							instance,
							_ibSqlExpressionFactory.Function(
								"CHAR_LENGTH",
								new[] { patternExpression },
								true,
								new[] { true },
								typeof(int)) },
					true,
					new[] { true, true },
					instance.Type)),
				patternExpression));
		return patternConstantExpression != null
			? (string)patternConstantExpression.Value == string.Empty
				? _ibSqlExpressionFactory.Constant(true)
				: startsWithExpression
			: _ibSqlExpressionFactory.OrElse(
				startsWithExpression,
				_ibSqlExpressionFactory.Equal(
					patternExpression,
					_ibSqlExpressionFactory.Constant(string.Empty)));
	}
}