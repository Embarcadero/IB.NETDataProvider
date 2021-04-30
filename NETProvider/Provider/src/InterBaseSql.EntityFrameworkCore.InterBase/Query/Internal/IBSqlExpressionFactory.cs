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

using InterBaseSql.EntityFrameworkCore.InterBase.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal
{
	public class IBSqlExpressionFactory : SqlExpressionFactory
	{
		public IBSqlExpressionFactory(SqlExpressionFactoryDependencies dependencies)
			: base(dependencies)
		{ }

		public IBSubstringExpression Substring(SqlExpression valueExpression, SqlExpression fromExpression, SqlExpression forExpression)
			=> (IBSubstringExpression)ApplyDefaultTypeMapping(new IBSubstringExpression(valueExpression, fromExpression, forExpression, null));

		public IBExtractExpression Extract(string part, SqlExpression valueExpression)
			=> (IBExtractExpression)ApplyDefaultTypeMapping(new IBExtractExpression(part, valueExpression, null));

		public IBDateTimeDateMemberExpression DateTimeDateMember(SqlExpression valueExpression)
			=> (IBDateTimeDateMemberExpression)ApplyDefaultTypeMapping(new IBDateTimeDateMemberExpression(valueExpression, null));

		public IBTrimExpression Trim(string where, SqlExpression whatExpression, SqlExpression valueExpression)
			=> (IBTrimExpression)ApplyDefaultTypeMapping(new IBTrimExpression(where, whatExpression, valueExpression, null));

		public override SqlExpression ApplyTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
			=> sqlExpression == null || sqlExpression.TypeMapping != null
				? sqlExpression
				: sqlExpression switch
				{
					IBSubstringExpression e => ApplyTypeMappingOnSubstring(e),
					IBExtractExpression e => ApplyTypeMappingOnExtract(e),
					IBDateTimeDateMemberExpression e => ApplyTypeMappingOnDateTimeDateMember(e),
					IBTrimExpression e => ApplyTypeMappingOnTrim(e),
					_ => base.ApplyTypeMapping(sqlExpression, typeMapping)
				};

		SqlExpression ApplyTypeMappingOnSubstring(IBSubstringExpression expression)
		{
			return new IBSubstringExpression(
				ApplyDefaultTypeMapping(expression.ValueExpression),
				ApplyDefaultTypeMapping(expression.FromExpression),
				ApplyDefaultTypeMapping(expression.ForExpression),
				expression.TypeMapping ?? FindMapping(expression.Type));
		}

		SqlExpression ApplyTypeMappingOnExtract(IBExtractExpression expression)
		{
			return new IBExtractExpression(
				expression.Part,
				ApplyDefaultTypeMapping(expression.ValueExpression),
				expression.TypeMapping ?? FindMapping(expression.Type));
		}

		SqlExpression ApplyTypeMappingOnDateTimeDateMember(IBDateTimeDateMemberExpression expression)
		{
			return new IBDateTimeDateMemberExpression(
				ApplyDefaultTypeMapping(expression.ValueExpression),
				expression.TypeMapping ?? FindMapping(expression.Type));
		}

		SqlExpression ApplyTypeMappingOnTrim(IBTrimExpression expression)
		{
			return new IBTrimExpression(
				expression.Where,
				ApplyDefaultTypeMapping(expression.WhatExpression),
				ApplyDefaultTypeMapping(expression.ValueExpression),
				expression.TypeMapping ?? FindMapping(expression.Type));
		}
	}
}
