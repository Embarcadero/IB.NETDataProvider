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
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal
{
	public class IBStartsWithOptimizedTranslator : IMethodCallTranslator
	{
		static readonly MethodInfo StartsWithMethod = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

		readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

		public IBStartsWithOptimizedTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
		{
			_ibSqlExpressionFactory = ibSqlExpressionFactory;
		}

		public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
		{
			if (!method.Equals(StartsWithMethod))
				return null;

			var patternExpression = arguments[0];

			var startsWithExpression = _ibSqlExpressionFactory.AndAlso(
				_ibSqlExpressionFactory.Like(
					instance,
					_ibSqlExpressionFactory.Add(patternExpression, _ibSqlExpressionFactory.Constant("%"))),
				_ibSqlExpressionFactory.Equal(
					_ibSqlExpressionFactory.Function(
						"LEFT",
						new[] {
							instance,
							_ibSqlExpressionFactory.Function(
								"EF_LENGTH",
								new[] { patternExpression },
								typeof(int)) },
						instance.Type),
					patternExpression));
			return patternExpression is SqlConstantExpression sqlConstantExpression
				? (string)sqlConstantExpression.Value == string.Empty
					? (SqlExpression)_ibSqlExpressionFactory.Constant(true)
					: startsWithExpression
				: _ibSqlExpressionFactory.OrElse(
					startsWithExpression,
					_ibSqlExpressionFactory.Equal(
						patternExpression,
						_ibSqlExpressionFactory.Constant(string.Empty)));
		}
	}
}
