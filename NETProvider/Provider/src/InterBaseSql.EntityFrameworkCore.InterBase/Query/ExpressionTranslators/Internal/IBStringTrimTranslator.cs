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

using System;
using System.Collections.Generic;
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal;

public class IBStringTrimTranslator : IMethodCallTranslator
{
	static readonly MethodInfo TrimWithoutArgsMethod = typeof(string).GetRuntimeMethod(nameof(string.Trim), new Type[] { });
	static readonly MethodInfo TrimWithCharArgMethod = typeof(string).GetRuntimeMethod(nameof(string.Trim), new[] { typeof(char) });
	static readonly MethodInfo TrimEndWithoutArgsMethod = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), new Type[] { });
	static readonly MethodInfo TrimEndWithCharArgMethod = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), new[] { typeof(char) });
	static readonly MethodInfo TrimStartWithoutArgsMethod = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), new Type[] { });
	static readonly MethodInfo TrimStartWithCharArgMethod = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), new[] { typeof(char) });

	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBStringTrimTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!TryGetTrimDefinition(instance, method, arguments, out var trimArguments, out var nullability))
		{
			return null;
		}
		return _ibSqlExpressionFactory.SpacedFunction(
			"EF_TRIM",
			trimArguments,
			true,
			nullability,
			typeof(string));
	}

	bool TryGetTrimDefinition(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, out IEnumerable<SqlExpression> trimArguments, out IEnumerable<bool> nullability)
	{
		if (method.Equals(TrimWithoutArgsMethod))
		{
			trimArguments = new[] { _ibSqlExpressionFactory.Fragment("'BOTH'"), _ibSqlExpressionFactory.Fragment(", "), instance };
			nullability = new[] { false, false, true };
			return true;
		}
		if (method.Equals(TrimWithCharArgMethod))
		{
			trimArguments = new[] { _ibSqlExpressionFactory.Fragment("'BOTH'"), _ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]), _ibSqlExpressionFactory.Fragment(", "), instance };
			nullability = new[] { false, true, false, true };
			return true;
		}
		if (method.Equals(TrimEndWithoutArgsMethod))
		{
			trimArguments = new[] { _ibSqlExpressionFactory.Fragment("'TRAILING'"), _ibSqlExpressionFactory.Fragment(", "), instance };
			nullability = new[] { false, false, true };
			return true;
		}
		if (method.Equals(TrimEndWithCharArgMethod))
		{
			trimArguments = new[] { _ibSqlExpressionFactory.Fragment("'TRAILING'"), _ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]), _ibSqlExpressionFactory.Fragment(", "), instance };
			nullability = new[] { false, true, false, true };
			return true;
		}
		if (method.Equals(TrimStartWithoutArgsMethod))
		{
			trimArguments = new[] { _ibSqlExpressionFactory.Fragment("'LEADING'"), _ibSqlExpressionFactory.Fragment(", "), instance };
			nullability = new[] { false, false, true };
			return true;
		}
		if (method.Equals(TrimStartWithCharArgMethod))
		{
			trimArguments = new[] { _ibSqlExpressionFactory.Fragment("'LEADING'"), _ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]), _ibSqlExpressionFactory.Fragment(", "), instance };
			nullability = new[] { false, true, false, true };
			return true;
		}
		trimArguments = default;
		nullability = default;
		return false;
	}
}