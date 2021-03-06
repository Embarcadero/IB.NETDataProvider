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
using System.Linq.Expressions;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.Expressions.Internal
{
	public class IBSubstringExpression : SqlFunctionExpression, IEquatable<IBSubstringExpression>
	{
		public virtual SqlExpression ValueExpression { get; }
		public virtual SqlExpression FromExpression { get; }
		public virtual SqlExpression ForExpression { get; }

		public IBSubstringExpression(SqlExpression valueExpression, SqlExpression fromExpression, SqlExpression forExpression, RelationalTypeMapping typeMapping)
			: base(default, default, default, default, default, default, typeof(string), typeMapping)
		{
			ValueExpression = valueExpression;
			FromExpression = fromExpression;
			ForExpression = forExpression;
		}

		protected override Expression Accept(ExpressionVisitor visitor)
			=> visitor is IBQuerySqlGenerator fbQuerySqlGenerator
				? fbQuerySqlGenerator.VisitSubstring(this)
				: base.Accept(visitor);

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var newValueExpression = (SqlExpression)visitor.Visit(ValueExpression);
			var newFromExpression = (SqlExpression)visitor.Visit(FromExpression);
			var newForExpression = (SqlExpression)visitor.Visit(ForExpression);

			return newValueExpression != ValueExpression || newFromExpression != FromExpression || newForExpression != ForExpression
				? new IBSubstringExpression(newValueExpression, newFromExpression, newForExpression, TypeMapping)
				: this;
		}

		public override void Print(ExpressionPrinter expressionPrinter)
		{
			expressionPrinter.Append("SUBSTRING(");
			expressionPrinter.Visit(ValueExpression);
			expressionPrinter.Append(" FROM ");
			expressionPrinter.Visit(FromExpression);
			if (ForExpression != null)
			{
				expressionPrinter.Append(" FOR ");
				expressionPrinter.Visit(ForExpression);
			}
			expressionPrinter.Append(")");
		}

		public override bool Equals(object obj)
		{
			return obj != null
				&& (ReferenceEquals(this, obj)
					|| obj is IBSubstringExpression fbSubstringExpression
					&& Equals(fbSubstringExpression));
		}

		public bool Equals(IBSubstringExpression other)
		{
			return base.Equals(other)
			   && ValueExpression.Equals(other.ValueExpression)
			   && FromExpression.Equals(other.FromExpression)
			   && (ForExpression == null ? other.ForExpression == null : ForExpression.Equals(other.ForExpression));
		}

		public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), ValueExpression, FromExpression, ForExpression);
	}
}
