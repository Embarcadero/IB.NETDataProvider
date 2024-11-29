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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Expressions.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;

public class IBQuerySqlGenerator : QuerySqlGenerator
{
	readonly IIBOptions _ibOptions;

	public IBQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, IIBOptions ibOptions)
		: base(dependencies)
	{
		_ibOptions = ibOptions;
	}

	protected override Expression VisitCase(CaseExpression caseExpression)
	{
		Sql.Append("CASE");

		if (caseExpression.Operand != null)
		{
			Sql.Append(" ");
			Visit(caseExpression.Operand);
		}

		using (Sql.Indent())
		{
			foreach (var whenClause in caseExpression.WhenClauses)
			{
				Sql.AppendLine()
				   .Append("WHEN ");
				Visit(whenClause.Test);
				if ((whenClause.Test is ColumnExpression) && (whenClause.Test.TypeMapping.DbType == System.Data.DbType.Boolean))
					Sql.Append(" = TRUE ");
				Sql.Append(" THEN ");
				Visit(whenClause.Result);
			}

			if (caseExpression.ElseResult != null)
			{
				Sql.AppendLine()
				   .Append("ELSE ");
				Visit(caseExpression.ElseResult);
			}
		}

		Sql.AppendLine()
		   .Append("END");
		return caseExpression;
	}
	protected override Expression VisitCrossJoin(CrossJoinExpression crossJoinExpression)
	{
		Sql.Append(", ");
		Visit(crossJoinExpression.Table);

		return crossJoinExpression;
	}

	protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
	{
		if (sqlUnaryExpression.OperatorType == ExpressionType.Not && sqlUnaryExpression.TypeMapping.ClrType != typeof(bool))
		{
			Sql.Append("EF_BITNOT(");
			Visit(sqlUnaryExpression.Operand);
			Sql.Append(")");
			return sqlUnaryExpression;
		}
		else
		{
			return base.VisitSqlUnary(sqlUnaryExpression);
		}
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
		else if (sqlBinaryExpression.OperatorType == ExpressionType.And)
		{
			if (sqlBinaryExpression.TypeMapping.ClrType == typeof(bool))
			{
				Sql.Append("IIF(EF_BITAND(");
				BooleanToIntegralAndVisit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				BooleanToIntegralAndVisit(sqlBinaryExpression.Right);
				Sql.Append(") = 0, FALSE, TRUE)");
			}
			else
			{
				Sql.Append("EF_BITAND(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
			}
			return sqlBinaryExpression;
		}
		else if (sqlBinaryExpression.OperatorType == ExpressionType.Or)
		{
			if (sqlBinaryExpression.TypeMapping.ClrType == typeof(bool))
			{
				Sql.Append("IIF(EF_BITOR(");
				BooleanToIntegralAndVisit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				BooleanToIntegralAndVisit(sqlBinaryExpression.Right);
				Sql.Append(") = 0, FALSE, TRUE)");
			}
			else
			{
				Sql.Append("EF_BITOR(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
			}
			return sqlBinaryExpression;
		}
		else if (sqlBinaryExpression.OperatorType == ExpressionType.ExclusiveOr)
		{
			if (sqlBinaryExpression.TypeMapping.ClrType == typeof(bool))
			{
				Sql.Append("IIF(EF_BITXOR(");
				BooleanToIntegralAndVisit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				BooleanToIntegralAndVisit(sqlBinaryExpression.Right);
				Sql.Append(") = 0, FALSE, TRUE)");
			}
			else
			{
				Sql.Append("EF_BITXOR(");
				Visit(sqlBinaryExpression.Left);
				Sql.Append(", ");
				Visit(sqlBinaryExpression.Right);
				Sql.Append(")");
			}
			return sqlBinaryExpression;
		}
		else if (sqlBinaryExpression.OperatorType == ExpressionType.LeftShift)
		{
			Sql.Append("EF_BITSHL(");
			Visit(sqlBinaryExpression.Left);
			Sql.Append(", ");
			Visit(sqlBinaryExpression.Right);
			Sql.Append(")");
			return sqlBinaryExpression;
		}
		else if (sqlBinaryExpression.OperatorType == ExpressionType.RightShift)
		{
			Sql.Append("EF_BITSHR(");
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

		void BooleanToIntegralAndVisit(SqlExpression expression)
		{
			Sql.Append("CASE ");
			Visit(expression);
			Sql.Append("WHEN TRUE THEN 1 ELSE 0 END");
		}
	}

	protected override Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
	{
		var shouldExplicitParameterTypes = _ibOptions.ExplicitParameterTypes;
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
				var isUnicode = IBTypeMappingSource.IsUnicode(sqlParameterExpression.TypeMapping);
				Sql.Append(((IIBSqlGenerationHelper)Dependencies.SqlGenerationHelper).StringParameterQueryType(isUnicode));
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
		var shouldExplicitStringLiteralTypes = _ibOptions.ExplicitStringLiteralTypes && sqlConstantExpression.Type == typeof(string);
		if (shouldExplicitStringLiteralTypes)
		{
			Sql.Append("CAST(");
		}
		base.VisitSqlConstant(sqlConstantExpression);
		if (shouldExplicitStringLiteralTypes)
		{
			Sql.Append(" AS ");
			Sql.Append(((IIBSqlGenerationHelper)Dependencies.SqlGenerationHelper).StringLiteralQueryType(sqlConstantExpression.Value as string));
			Sql.Append(")");
		}
		return sqlConstantExpression;
	}

	protected override void GenerateEmptyProjection(SelectExpression selectExpression)
	{
		base.GenerateEmptyProjection(selectExpression);
		if (selectExpression.Alias != null)
		{
			Sql.Append(" AS empty");
		}
	}

	protected override void GenerateTop(SelectExpression selectExpression)
	{
		// handled by GenerateLimitOffset
	}

	protected override void GenerateLimitOffset(SelectExpression selectExpression)
	{
		if (selectExpression.Limit != null && selectExpression.Offset != null)
		{
			Sql.AppendLine();
			Sql.Append("ROWS (");
			Visit(selectExpression.Offset);
			Sql.Append(" + 1) TO (");
			Visit(selectExpression.Offset);
			Sql.Append(" + ");
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
			Sql.Append(" + 1) TO (");
			Sql.Append(long.MaxValue.ToString(CultureInfo.InvariantCulture));
			Sql.Append(")");
		}
	}

	protected override string GetOperator(SqlBinaryExpression binaryExpression)
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
		return base.GetOperator(binaryExpression);
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

	protected override void GeneratePseudoFromClause()
	{
		Sql.Append(" FROM RDB$DATABASE");
	}

	protected override Expression VisitOrdering(OrderingExpression orderingExpression)
	{
		if (orderingExpression.Expression is SqlConstantExpression
			|| orderingExpression.Expression is SqlParameterExpression
			|| (orderingExpression.Expression is SqlFragmentExpression sqlFragment && sqlFragment.Sql.Equals("(SELECT 1)", StringComparison.Ordinal)))
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

	protected override Expression VisitTableValuedFunction(TableValuedFunctionExpression tableValuedFunctionExpression)
	{
		Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableValuedFunctionExpression.StoreFunction.Name));
		if (tableValuedFunctionExpression.Arguments.Any())
		{
			Sql.Append("(");
			GenerateList(tableValuedFunctionExpression.Arguments, e => Visit(e));
			Sql.Append(")");
		}
		Sql.Append(AliasSeparator);
		Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableValuedFunctionExpression.Alias));
		return tableValuedFunctionExpression;
	}

	protected override Expression VisitExtension(Expression extensionExpression)
	{
		return extensionExpression switch
		{
			IBSpacedFunctionExpression spacedFunctionExpression => VisitSpacedFunction(spacedFunctionExpression),
			_ => base.VisitExtension(extensionExpression),
		};
	}

	public virtual Expression VisitSpacedFunction(IBSpacedFunctionExpression spacedFunctionExpression)
	{
		Sql.Append(spacedFunctionExpression.Name);
		Sql.Append("(");
		for (var i = 0; i < spacedFunctionExpression.Arguments.Count; i++)
		{
			Visit(spacedFunctionExpression.Arguments[i]);
			if (i < spacedFunctionExpression.Arguments.Count - 1)
			{
				Sql.Append(" ");
			}
		}
		Sql.Append(")");
		return spacedFunctionExpression;
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