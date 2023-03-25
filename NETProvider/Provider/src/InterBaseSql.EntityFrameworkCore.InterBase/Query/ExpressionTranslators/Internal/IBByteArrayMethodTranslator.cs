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

public class IBByteArrayMethodTranslator : IMethodCallTranslator
{
	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBByteArrayMethodTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (method.IsGenericMethod
			&& method.GetGenericMethodDefinition().Equals(EnumerableMethods.Contains)
			&& arguments[0].Type == typeof(byte[]))
		{
			var value = arguments[1] is SqlConstantExpression constantValue
				? _ibSqlExpressionFactory.Function("ASCII_CHAR", new[] { _ibSqlExpressionFactory.Constant((byte)constantValue.Value) }, false, new[] { false }, typeof(string))
				: _ibSqlExpressionFactory.Function("ASCII_CHAR", new[] { _ibSqlExpressionFactory.Convert(_ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]), typeof(byte)) }, true, new[] { true }, typeof(string));

			return _ibSqlExpressionFactory.GreaterThan(
				_ibSqlExpressionFactory.Function(
					"POSITION",
					new[] { value, _ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]) },
					true,
					new[] { true, true },
					typeof(int)),
				_ibSqlExpressionFactory.Constant(0));
		}
		return null;
	}
}