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
	public class IBTrimExpression : SqlFunctionExpression, IEquatable<IBTrimExpression>
	{
		public virtual string Where { get; }
		public virtual SqlExpression WhatExpression { get; }
		public virtual SqlExpression ValueExpression { get; }

		public IBTrimExpression(string where, SqlExpression whatExpression, SqlExpression valueExpression, RelationalTypeMapping typeMapping)
			: base(default, default, default, default, default, default, typeof(string), typeMapping)
		{
			Where = where;
			WhatExpression = whatExpression;
			ValueExpression = valueExpression;
		}

		protected override Expression Accept(ExpressionVisitor visitor)
			=> visitor is IBQuerySqlGenerator fbQuerySqlGenerator
				? fbQuerySqlGenerator.VisitTrim(this)
				: base.Accept(visitor);

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var newValueExpression = (SqlExpression)visitor.Visit(ValueExpression);

			return newValueExpression != ValueExpression
				? new IBTrimExpression(Where, WhatExpression, newValueExpression, TypeMapping)
				: this;
		}

		public override void Print(ExpressionPrinter expressionPrinter)
		{
			expressionPrinter.Append("TRIM(");
			expressionPrinter.Append(Where);
			if (WhatExpression != null)
			{
				expressionPrinter.Append(" ");
				expressionPrinter.Visit(WhatExpression);
			}
			expressionPrinter.Append(" FROM ");
			expressionPrinter.Visit(ValueExpression);
			expressionPrinter.Append(")");
		}

		public override bool Equals(object obj)
		{
			return obj != null
				&& (ReferenceEquals(this, obj)
					|| obj is IBTrimExpression fbTrimExpression
					&& Equals(fbTrimExpression));
		}

		public bool Equals(IBTrimExpression other)
		{
			return base.Equals(other)
			   && Where.Equals(other.Where)
			   && WhatExpression.Equals(other.WhatExpression)
			   && ValueExpression.Equals(other.ValueExpression);
		}

		public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Where, WhatExpression, ValueExpression);
	}
}
