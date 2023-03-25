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
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

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

	[Fact(Skip = "efcore#24228")]
	public override void Nullable_navigation_property_access_preserves_schema_for_sql_function()
	{
		base.Nullable_navigation_property_access_preserves_schema_for_sql_function();
	}

	[Fact(Skip = "efcore#24228")]
	public override void Compare_function_without_null_propagation_to_null()
	{
		base.Compare_function_without_null_propagation_to_null();
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
				.HasTranslation(args => new SqlFunctionExpression("char_length", args, true, new[] { true }, methodInfo.ReturnType, null));
			var methodInfo2 = typeof(UDFSqlContext).GetMethod(nameof(MyCustomLengthInstance));
			modelBuilder.HasDbFunction(methodInfo2)
				.HasTranslation(args => new SqlFunctionExpression("char_length", args, true, new[] { true }, methodInfo2.ReturnType, null));
			var methodInfo3 = typeof(UDFSqlContext).GetMethod(nameof(StringLength));
			modelBuilder.HasDbFunction(methodInfo3)
				.HasTranslation(args => new SqlFunctionExpression("char_length", args, true, new[] { true }, methodInfo3.ReturnType, null));

			modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetCustomerWithMostOrdersAfterDateStatic)))
				.HasName("GetCustWithMostOrdersAfterDate");
			modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetCustomerWithMostOrdersAfterDateInstance)))
				.HasName("GetCustWithMostOrdersAfterDate");

			modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(IdentityString)))
				.HasSchema(null);

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
				@"create function ""CustomerOrderCount"" (customerId int)
                                                    returns int
                                                    as
                                                    begin
                                                        return (select count(""Id"") from ""Orders"" where ""CustomerId"" = :customerId);
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create function ""StarValue"" (starCount int, val varchar(1000))
                                                    returns varchar(1000)
                                                    as
                                                    begin
                                                        return rpad('', :starCount, '*') || :val;
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create function ""DollarValue"" (starCount int, val varchar(1000))
                                                    returns varchar(1000)
                                                    as
                                                    begin
                                                        return rpad('', :starCount, '$') || :val;
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create function ""GetReportingPeriodStartDate"" (period int)
                                                    returns timestamp
                                                    as
                                                    begin
                                                        return cast('1998-01-01' as timestamp);
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create function ""GetCustWithMostOrdersAfterDate"" (searchDate Date)
                                                    returns int
                                                    as
                                                    begin
                                                        return (select first 1 ""CustomerId""
                                                                from ""Orders""
                                                                where ""OrderDate"" > :searchDate
                                                                group by ""CustomerId""
                                                                order by count(""Id"") desc);
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create function ""IsTopCustomer"" (customerId int)
                                                    returns boolean
                                                    as
                                                    begin
                                                        if (:customerId = 1) then
                                                            return true;

                                                        return false;
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create function ""IdentityString"" (customerName varchar(1000))
                                                    returns varchar(1000)
                                                    as
                                                    begin
                                                        return :customerName;
                                                    end");

			context.Database.ExecuteSqlRaw(
				@"create function ""IsDate"" (val varchar(1000))
                                                    returns boolean
                                                    as
                                                    declare dummy date;
                                                    begin
                                                        begin
                                                            begin
                                                                dummy = cast(val as date);
                                                            end
                                                            when any do
                                                            begin
                                                                return false;
                                                            end
                                                        end
                                                        return true;
                                                    end");

			context.SaveChanges();
		}
	}
}
