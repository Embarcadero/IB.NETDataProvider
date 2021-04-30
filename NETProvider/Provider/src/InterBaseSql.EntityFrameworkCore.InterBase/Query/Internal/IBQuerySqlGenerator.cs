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
using System.Linq;
using System.Linq.Expressions;
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Expressions.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal
{
	public class IBQuerySqlGenerator : QuerySqlGenerator
	{
		readonly IIBOptions _ibOptions;
		bool GeneratingLimits = false;

		public IBQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, IIBOptions ibOptions)
			: base(dependencies)
		{
			_ibOptions = ibOptions;
		}

		protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
		{
			if (sqlBinaryExpression.OperatorType == ExpressionType.Modulo)
			{
				Sql.Append("EF_MOD(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
				return sqlBinaryExpression;
			}
			else if (sqlBinaryExpression.OperatorType == ExpressionType.And && sqlBinaryExpression.TypeMapping.ClrType != typeof(bool))
			{
				Sql.Append("EF_BINAND(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
				return sqlBinaryExpression;
			}
			else if (sqlBinaryExpression.OperatorType == ExpressionType.Or && sqlBinaryExpression.TypeMapping.ClrType != typeof(bool))
			{
				Sql.Append("EF_BINOR(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
				return sqlBinaryExpression;
			}
			else if (sqlBinaryExpression.OperatorType == ExpressionType.ExclusiveOr)
			{
				Sql.Append("EF_BINXOR(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
				return sqlBinaryExpression;
			}
			else if (sqlBinaryExpression.OperatorType == ExpressionType.LeftShift)
			{
				Sql.Append("EF_BINSHL(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
				return sqlBinaryExpression;
			}
			else if (sqlBinaryExpression.OperatorType == ExpressionType.RightShift)
			{
				Sql.Append("EF_BINSHR(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
				return sqlBinaryExpression;
			}
			else
			{
				return base.VisitSqlBinary(sqlBinaryExpression);
			}
		}

		protected override Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
		{
			var shouldExplicitParameterTypes = _ibOptions.ExplicitParameterTypes && (GeneratingLimits != true);
			if (shouldExplicitParameterTypes)
			{
				Sql.Append("CAST(");
			}
			base.VisitSqlParameter(sqlParameterExpression);
			if (shouldExplicitParameterTypes)
			{
				Sql.Append(" AS ");
				if (sqlParameterExpression.Type == typeof(string))
				{
					Sql.Append((Dependencies.SqlGenerationHelper as IIBSqlGenerationHelper).StringParameterQueryType());
				}
				else
				{
					Sql.Append(sqlParameterExpression.TypeMapping.StoreType);
				}
				Sql.Append(")");
			}
			return sqlParameterExpression;
		}

		protected override Expression VisitSqlConstant(SqlConstantExpression sqlConstantExpression)
		{
			var shouldExplicitStringLiteralTypes = (_ibOptions.ExplicitStringLiteralTypes && sqlConstantExpression.Type == typeof(string)) && (GeneratingLimits != true);
			if (shouldExplicitStringLiteralTypes)
			{
				Sql.Append("CAST(");
			}
			base.VisitSqlConstant(sqlConstantExpression);
			if (shouldExplicitStringLiteralTypes)
			{
				Sql.Append(" AS ");
				Sql.Append((Dependencies.SqlGenerationHelper as IIBSqlGenerationHelper).StringLiteralQueryType(sqlConstantExpression.Value as string));
				Sql.Append(")");
			}
			return sqlConstantExpression;
		}

		protected override void GenerateTop(SelectExpression selectExpression)
		{
			// handled by GenerateLimitOffset
		}

		protected override void GenerateLimitOffset(SelectExpression selectExpression)
		{
			GeneratingLimits = true;
//			var p = GetCommand(selectExpression).Parameters;
			if (selectExpression.Limit != null && selectExpression.Offset != null)
			{
				Sql.AppendLine();
				Sql.Append("ROWS (");
				Visit(selectExpression.Offset);
				Sql.Append(") TO (");
//				Sql.Append(" + 1) TO (");
//				Visit(selectExpression.Offset);
//				Sql.Append(" + ");
				Visit(selectExpression.Limit);
				Sql.Append(")");
			}
			else if (selectExpression.Limit != null && selectExpression.Offset == null)
			{
				Sql.AppendLine();
				Sql.Append("ROWS (");
				Visit(selectExpression.Limit);
				Sql.Append(")");
			}
			else if (selectExpression.Limit == null && selectExpression.Offset != null)
			{
				Sql.AppendLine();
				Sql.Append("ROWS (");
				Visit(selectExpression.Offset);
				Sql.Append(") TO (");
				Sql.Append(int.MaxValue);
				Sql.Append(")");
			}
			GeneratingLimits = false;
		}

		protected override string GenerateOperator(SqlBinaryExpression binaryExpression)
		{
			if (binaryExpression.OperatorType == ExpressionType.Add && binaryExpression.TypeMapping.ClrType == typeof(string))
			{
				return " || ";
			}
			else if (binaryExpression.OperatorType == ExpressionType.AndAlso || binaryExpression.OperatorType == ExpressionType.And)
			{
				return " AND ";
			}
			else if (binaryExpression.OperatorType == ExpressionType.OrElse || binaryExpression.OperatorType == ExpressionType.Or)
			{
				return " OR ";
			}
			return base.GenerateOperator(binaryExpression);
		}

		// https://github.com/aspnet/EntityFrameworkCore/issues/19031
		protected override void GenerateOrderings(SelectExpression selectExpression)
		{
			if (selectExpression.Orderings.Any())
			{
				var orderings = selectExpression.Orderings.ToList();

				if (selectExpression.Limit == null
					&& selectExpression.Offset == null)
				{
					orderings.RemoveAll(oe => oe.Expression is SqlConstantExpression || oe.Expression is SqlParameterExpression);
				}

				if (orderings.Count > 0)
				{
					Sql.AppendLine()
						.Append("ORDER BY ");

					GenerateList(orderings, e => Visit(e));
				}
			}
		}

		protected /*override*/ void GeneratePseudoFromClause()
		{
			Sql.Append(" FROM RDB$DATABASE");
		}

		// GeneratePseudoFromClause workaround
		protected override Expression VisitSelect(SelectExpression selectExpression)
		{
			if (selectExpression.Alias != null)
			{
				Sql.AppendLine("(");
				Sql.IncrementIndent();
			}

			Sql.Append("SELECT ");

			if (selectExpression.IsDistinct)
			{
				Sql.Append("DISTINCT ");
			}

			GenerateTop(selectExpression);

			if (selectExpression.Projection.Any())
			{
				GenerateList(selectExpression.Projection, e => Visit(e));
			}
			else
			{
				Sql.Append("1");
			}

			if (selectExpression.Tables.Any())
			{
				Sql.AppendLine().Append("FROM ");

				GenerateList(selectExpression.Tables, e => Visit(e), sql => sql.AppendLine());
			}
			else
			{
				GeneratePseudoFromClause();
			}

			if (selectExpression.Predicate != null)
			{
				Sql.AppendLine().Append("WHERE ");

				Visit(selectExpression.Predicate);
			}

			if (selectExpression.GroupBy.Count > 0)
			{
				Sql.AppendLine().Append("GROUP BY ");

				GenerateList(selectExpression.GroupBy, e => Visit(e));
			}

			if (selectExpression.Having != null)
			{
				Sql.AppendLine().Append("HAVING ");

				Visit(selectExpression.Having);
			}

			GenerateOrderings(selectExpression);
			GenerateLimitOffset(selectExpression);

			if (selectExpression.Alias != null)
			{
				Sql.DecrementIndent();

				Sql.AppendLine()
					.Append(")" + AliasSeparator + Dependencies.SqlGenerationHelper.DelimitIdentifier(selectExpression.Alias));
			}

			return selectExpression;
		}

		protected override Expression VisitOrdering(OrderingExpression orderingExpression)
		{
			if (orderingExpression.Expression is SqlConstantExpression
				|| orderingExpression.Expression is SqlParameterExpression)
			{
				Sql.Append("(SELECT 1");
				GeneratePseudoFromClause();
				Sql.Append(")");
			}
			else
			{
				Visit(orderingExpression.Expression);
			}

			if (!orderingExpression.IsAscending)
			{
				Sql.Append(" DESC");
			}

			return orderingExpression;
		}

		public virtual Expression VisitSubstring(IBSubstringExpression substringExpression)
		{
			Sql.Append("EF_SUBSTR(");
			Visit(substringExpression.ValueExpression);
			Sql.Append(", ");
			Visit(substringExpression.FromExpression);
			if (substringExpression.ForExpression != null)
			{
				Sql.Append(", ");
				Visit(substringExpression.ForExpression);
			}
			Sql.Append(")");
			return substringExpression;
		}

		public virtual Expression VisitExtract(IBExtractExpression extractExpression)
		{
			Sql.Append("EXTRACT(");
			Sql.Append(extractExpression.Part);
			Sql.Append(" FROM ");
			Visit(extractExpression.ValueExpression);
			Sql.Append(")");
			return extractExpression;
		}

		public virtual Expression VisitDateTimeDateMember(IBDateTimeDateMemberExpression dateTimeDateMemberExpression)
		{
			Sql.Append("CAST(");
			Visit(dateTimeDateMemberExpression.ValueExpression);
			Sql.Append(" AS DATE)");
			return dateTimeDateMemberExpression;
		}

		public virtual Expression VisitTrim(IBTrimExpression trimExpression)
		{
			Sql.Append("EF_TRIM(");
			Sql.Append(trimExpression.Where);
			if (trimExpression.WhatExpression != null)
			{
				Sql.Append(" ");
				Visit(trimExpression.WhatExpression);
			}
			Sql.Append(", ");
			Visit(trimExpression.ValueExpression);
			Sql.Append(")");
			return trimExpression;
		}

		void GenerateList<T>(IReadOnlyList<T> items, Action<T> generationAction, Action<IRelationalCommandBuilder> joinAction = null)
		{
			joinAction ??= (isb => isb.Append(", "));

			for (var i = 0; i < items.Count; i++)
			{
				if (i > 0)
				{
					joinAction(Sql);
				}

				generationAction(items[i]);
			}
		}
	}
}
