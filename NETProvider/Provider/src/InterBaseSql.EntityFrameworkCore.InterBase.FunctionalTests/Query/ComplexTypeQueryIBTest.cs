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
using System.Linq;
using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.ComplexTypeModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class ComplexTypeQueryIBTest : ComplexTypeQueryRelationalTestBase<ComplexTypeQueryIBTest.ComplexTypeQueryIBFixture>
{
	public ComplexTypeQueryIBTest(ComplexTypeQueryIBFixture fixture)
		: base(fixture)
	{ }

	#region InterBase can not handle subqueries with parameters in the Rows section
	public override Task Filter_on_property_inside_complex_type_after_subquery(bool async)
		=> AssertQuery(
			async,
			ss => ss.Set<Customer>()
				.OrderBy(c => c.Id)
				//.Skip(1)
				//.Distinct()
				.Where(c => c.ShippingAddress.ZipCode == 07728));

	public override Task Filter_on_property_inside_nested_complex_type_after_subquery(bool async)
		=> AssertQuery(
			async,
			ss => ss.Set<Customer>()
				.OrderBy(c => c.Id)
				//.Skip(1)
				//.Distinct()
				.Where(c => c.ShippingAddress.Country.Code == "DE"));

	public override Task Filter_on_property_inside_struct_complex_type_after_subquery(bool async)
		=> AssertQuery(
			async,
			ss => ss.Set<ValuedCustomer>()
				.OrderBy(c => c.Id)
				//.Skip(1)
				//.Distinct()
				.Where(c => c.ShippingAddress.ZipCode == 07728));

	public override Task Filter_on_property_inside_nested_struct_complex_type_after_subquery(bool async)
		=> AssertQuery(
			async,
			ss => ss.Set<ValuedCustomer>()
				.OrderBy(c => c.Id)
				//.Skip(1)
				//.Distinct()
				.Where(c => c.ShippingAddress.Country.Code == "DE"));

	public override Task Load_struct_complex_type_after_subquery_on_entity_type(bool async)
		 => AssertQuery(
			 async,
			 ss => ss.Set<ValuedCustomer>()
				 .OrderBy(c => c.Id), null, null, true);
	//.Skip(1)
	//.Distinct());

	public override Task Load_complex_type_after_subquery_on_entity_type(bool async)
		 => AssertQuery(
			 async,
			 ss => ss.Set<Customer>()
				 .OrderBy(c => c.Id), null, null, true);
				 //.Skip(1)
				 //.Distinct());
	#endregion

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_complex_type_via_optional_navigation(bool async)
	{
		return Assert.ThrowsAsync<InvalidOperationException>(() => base.Project_complex_type_via_optional_navigation(async));
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_struct_complex_type_via_optional_navigation(bool async)
	{
		return Assert.ThrowsAsync<InvalidOperationException>(() => base.Project_struct_complex_type_via_optional_navigation(async));
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Union_property_in_complex_type(bool async)
	{
		return AssertQuery(
			async,
			ss => ss.Set<Customer>().Select(c => c.ShippingAddress.AddressLine1)
				.Union(ss.Set<Customer>().Select(c => c.BillingAddress.AddressLine1)));
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_entity_with_nested_complex_type_twice_with_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_entity_with_nested_complex_type_twice_with_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_entity_with_nested_complex_type_twice_with_double_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_entity_with_nested_complex_type_twice_with_double_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_entity_with_struct_nested_complex_type_twice_with_double_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_entity_with_struct_nested_complex_type_twice_with_double_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_entity_with_struct_nested_complex_type_twice_with_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_entity_with_struct_nested_complex_type_twice_with_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_nested_complex_type_twice_with_double_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_nested_complex_type_twice_with_double_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_nested_complex_type_twice_with_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_nested_complex_type_twice_with_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_struct_nested_complex_type_twice_with_double_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_struct_nested_complex_type_twice_with_double_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_same_struct_nested_complex_type_twice_with_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Project_same_struct_nested_complex_type_twice_with_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Union_of_same_entity_with_nested_complex_type_projected_twice_with_double_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Union_of_same_entity_with_nested_complex_type_projected_twice_with_double_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Union_of_same_entity_with_nested_complex_type_projected_twice_with_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Union_of_same_entity_with_nested_complex_type_projected_twice_with_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Union_of_same_nested_complex_type_projected_twice_with_double_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Union_of_same_nested_complex_type_projected_twice_with_double_pushdown(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Union_of_same_nested_complex_type_projected_twice_with_pushdown(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Union_of_same_nested_complex_type_projected_twice_with_pushdown(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Same_entity_with_complex_type_projected_twice_with_pushdown_as_part_of_another_projection(bool async)
	{
		return base.Same_entity_with_complex_type_projected_twice_with_pushdown_as_part_of_another_projection(async);
	}

	public class ComplexTypeQueryIBFixture : ComplexTypeQueryRelationalFixtureBase
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SetStringLengths(modelBuilder);
		}
	}
}
