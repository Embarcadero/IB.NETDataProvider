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
using System.Linq;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class UdfDbFunctionIBTests : UdfDbFunctionTestBase<UdfDbFunctionIBTests.IB>
{
	public UdfDbFunctionIBTests(IB fixture)
		: base(fixture)
	{ }

	[NotSupportedOnInterBaseFact]
	public override void QF_CrossApply_Correlated_Select_Anonymous()
	{
		base.QF_CrossApply_Correlated_Select_Anonymous();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_OuterApply_Correlated_Select_QF()
	{
		base.QF_OuterApply_Correlated_Select_QF();
	}

	[NotSupportedOnInterBaseFact]
	public override void Udf_with_argument_being_comparison_of_nullable_columns()
	{
		base.Udf_with_argument_being_comparison_of_nullable_columns();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_Select_Correlated_Subquery_In_Anonymous_MultipleCollections()
	{
		base.QF_Select_Correlated_Subquery_In_Anonymous_MultipleCollections();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_CrossApply_Correlated_Select_Result()
	{
		base.QF_CrossApply_Correlated_Select_Result();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_Select_Correlated_Subquery_In_Anonymous()
	{
		base.QF_Select_Correlated_Subquery_In_Anonymous();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_Correlated_Func_Call_With_Navigation()
	{
		base.QF_Correlated_Func_Call_With_Navigation();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_Select_Correlated_Direct_With_Function_Query_Parameter_Correlated_In_Anonymous()
	{
		base.QF_Select_Correlated_Direct_With_Function_Query_Parameter_Correlated_In_Anonymous();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_OuterApply_Correlated_Select_Entity()
	{
		base.QF_OuterApply_Correlated_Select_Entity();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_Correlated_Nested_Func_Call()
	{
		base.QF_Correlated_Nested_Func_Call();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_OuterApply_Correlated_Select_Anonymous()
	{
		base.QF_OuterApply_Correlated_Select_Anonymous();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_Select_Correlated_Subquery_In_Anonymous_Nested_With_QF()
	{
		base.QF_Select_Correlated_Subquery_In_Anonymous_Nested_With_QF();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_Correlated_Select_In_Anonymous()
	{
		base.QF_Correlated_Select_In_Anonymous();
	}

	[NotSupportedOnInterBaseFact]
	public override void QF_CrossApply_Correlated_Select_QF_Type()
	{
		base.QF_CrossApply_Correlated_Select_QF_Type();
	}

	[NotSupportedOnInterBaseFact]
	public override void Udf_with_argument_being_comparison_to_null_parameter()
	{
		base.Udf_with_argument_being_comparison_to_null_parameter();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_CrossJoin_Not_Correlated()
	{
		base.QF_CrossJoin_Not_Correlated();
	}

	[DoesNotHaveTheDataFact]
	public override void DbSet_mapped_to_function()
	{
		base.DbSet_mapped_to_function();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_LeftJoin_Select_Result()
	{
		base.QF_LeftJoin_Select_Result();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_Join()
	{
		base.QF_Join();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_LeftJoin_Select_Anonymous()
	{
		base.QF_LeftJoin_Select_Anonymous();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_Stand_Alone_Parameter()
	{
		base.QF_Stand_Alone_Parameter();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_Stand_Alone()
	{
		base.QF_Stand_Alone();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_Nested()
	{
		base.QF_Nested();
	}

	[DoesNotHaveTheDataFact]
	public override void QF_CrossJoin_Parameter()
	{
		base.QF_CrossJoin_Parameter();
	}

	[NotSupportedOnInterBaseFact]
	public override void TVF_with_argument_being_a_subquery_with_navigation_in_projection_groupby_aggregate()
	{
		base.TVF_with_argument_being_a_subquery_with_navigation_in_projection_groupby_aggregate();
	}

	protected class IBUDFSqlContext : UDFSqlContext
	{
		public IBUDFSqlContext(DbContextOptions options)
		: base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			var isDateMethodInfo = typeof(UDFSqlContext).GetMethod(nameof(IsDateStatic));
			modelBuilder.HasDbFunction(isDateMethodInfo)
				.HasTranslation(args => new SqlFunctionExpression(null, "IsDate", args, true, new[] { true }, isDateMethodInfo.ReturnType, null));
			var isDateMethodInfo2 = typeof(UDFSqlContext).GetMethod(nameof(IsDateInstance));
			modelBuilder.HasDbFunction(isDateMethodInfo2)
				.HasTranslation(args => new SqlFunctionExpression(null, "IsDate", args, true, new[] { true }, isDateMethodInfo2.ReturnType, null));

			var methodInfo = typeof(UDFSqlContext).GetMethod(nameof(MyCustomLengthStatic));
			modelBuilder.HasDbFunction(methodInfo)
				.HasTranslation(args => new SqlFunctionExpression("ef_length", args, true, new[] { true }, methodInfo.ReturnType, null));
			var methodInfo2 = typeof(UDFSqlContext).GetMethod(nameof(MyCustomLengthInstance));
			modelBuilder.HasDbFunction(methodInfo2)
				.HasTranslation(args => new SqlFunctionExpression("ef_length", args, true, new[] { true }, methodInfo2.ReturnType, null));
			var methodInfo3 = typeof(UDFSqlContext).GetMethod(nameof(StringLength));
			modelBuilder.HasDbFunction(methodInfo3)
				.HasTranslation(args => new SqlFunctionExpression("ef_length", args, true, new[] { true }, methodInfo3.ReturnType, null));

			modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetCustomerWithMostOrdersAfterDateStatic)))
				.HasName("GetCustWithMostOrdersAfterDate");
			modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetCustomerWithMostOrdersAfterDateInstance)))
				.HasName("GetCustWithMostOrdersAfterDate");

			modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(IdentityString)))
				.HasSchema(null);

			ModelHelpers.SetStringLengths(modelBuilder);
			ModelHelpers.SetPrimaryKeyGeneration(modelBuilder);
		}
	}


	public class IB : UdfFixtureBase
	{
		protected override string StoreName { get; } = nameof(UdfDbFunctionIBTests);
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;
		protected override Type ContextType { get; } = typeof(IBUDFSqlContext);

		protected override void Seed(DbContext context)
		{
			base.Seed(context);
			context.Database.ExecuteSqlRaw(
					@"create procedure ""GetTopTwoSellingProducts""
                                                    returns
                                                    (
                                                        ""ProductId"" int,
                                                        ""AmountSold"" int
                                                    )
                                                    as
                                                    begin
                                                        for select ""ProductId"", sum(""Quantity"") as ""TotalSold""
                                                        from ""LineItem""
                                                        group by ""ProductId""
                                                        order by ""TotalSold"" desc rows 2															
                                                        into :""ProductId"", :""AmountSold"" do
                                                        begin
                                                            suspend;
                                                        end
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create procedure ""GetOrdersWithMultipleProducts""(customerId int)
                                                    returns
                                                    (
                                                        ""OrderId"" int,
                                                        ""CustomerId"" int,
                                                        ""OrderDate"" timestamp
                                                    )
                                                    as
                                                    begin
                                                        for select o.""Id"", :customerId, ""OrderDate""
                                                        from ""Orders"" o
                                                        join ""LineItem"" li on o.""Id"" = li.""OrderId""
                                                        where o.""CustomerId"" = :customerId
                                                        group by o.""Id"", ""OrderDate""
                                                        having count(""ProductId"") > 1
                                                        into :""OrderId"", :""CustomerId"", :""OrderDate"" do
                                                        begin
                                                            suspend;
                                                        end
                                                    end");

			context.SaveChanges();
		}
	}
}