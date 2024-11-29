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
using System.Linq;
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal;

public class IBStringIndexOfTranslator : IMethodCallTranslator
{
	static readonly MethodInfo IndexOfMethodInfo
	   = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), new[] { typeof(string) });

	static readonly MethodInfo IndexOfMethodInfoWithStartingPosition
		= typeof(string).GetRuntimeMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(int) });

	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBStringIndexOfTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (method.Equals(IndexOfMethodInfo))
		{
			var args = new List<SqlExpression>();
			args.Add(_ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]));
			args.Add(instance);
			args.Add(_ibSqlExpressionFactory.Constant(0, RelationalTypeMapping.NullMapping));
			return _ibSqlExpressionFactory.Subtract(
				_ibSqlExpressionFactory.Function("EF_POSITION", args, true, Enumerable.Repeat(true, args.Count), typeof(int)),
				_ibSqlExpressionFactory.Constant(1));
		}
		if (method.Equals(IndexOfMethodInfoWithStartingPosition))
		{
			var args = new List<SqlExpression>();
			args.Add(_ibSqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]));
			args.Add(instance);
			args.Add(_ibSqlExpressionFactory.Add(arguments[1], _ibSqlExpressionFactory.Constant(1)));
			return _ibSqlExpressionFactory.Subtract(
				_ibSqlExpressionFactory.Function("EF_POSITION", args, true, Enumerable.Repeat(true, args.Count), typeof(int)),
				_ibSqlExpressionFactory.Constant(1));
		}
		return null;
	}
}