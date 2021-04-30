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
using System.Collections.Generic;
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal
{
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

		public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
		{
			if (method.Equals(TrimWithoutArgsMethod))
			{
				return _ibSqlExpressionFactory.Trim("'BOTH'", null, instance);
			}
			if (method.Equals(TrimWithCharArgMethod))
			{
				return _ibSqlExpressionFactory.Trim("'BOTH'", arguments[0], instance);
			}
			if (method.Equals(TrimEndWithoutArgsMethod))
			{
				return _ibSqlExpressionFactory.Trim("'TRAILING'", null, instance);
			}
			if (method.Equals(TrimEndWithCharArgMethod))
			{
				return _ibSqlExpressionFactory.Trim("'TRAILING'", arguments[0], instance);
			}
			if (method.Equals(TrimStartWithoutArgsMethod))
			{
				return _ibSqlExpressionFactory.Trim("'LEADING'", null, instance);
			}
			if (method.Equals(TrimStartWithCharArgMethod))
			{
				return _ibSqlExpressionFactory.Trim("'LEADING'", arguments[0], instance);
			}
			return null;
		}
	}
}
