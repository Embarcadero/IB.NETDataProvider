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

using System.Linq.Expressions;
using System.Collections.Generic;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;

public class IBSqlExpressionFactory : SqlExpressionFactory
{
	public IBSqlExpressionFactory(SqlExpressionFactoryDependencies dependencies)
		: base(dependencies)
	{ }

	public IBSpacedFunctionExpression SpacedFunction(string name, IEnumerable<SqlExpression> arguments, bool nullable, IEnumerable<bool> argumentsPropagateNullability, Type type, RelationalTypeMapping typeMapping = null)
		=> (IBSpacedFunctionExpression)ApplyDefaultTypeMapping(new IBSpacedFunctionExpression(name, arguments, nullable, argumentsPropagateNullability, type, typeMapping));

	public override SqlExpression ApplyTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
		=> sqlExpression == null || sqlExpression.TypeMapping != null
			? sqlExpression
			: sqlExpression switch
			{
				IBSpacedFunctionExpression e => e.ApplyTypeMapping(typeMapping),
				_ => base.ApplyTypeMapping(sqlExpression, typeMapping)
			};
}