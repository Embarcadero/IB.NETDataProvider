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
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class QueryFilterFuncletizationIBTest : QueryFilterFuncletizationTestBase<QueryFilterFuncletizationIBTest.QueryFilterFuncletizationIBFixture>
{
	public QueryFilterFuncletizationIBTest(QueryFilterFuncletizationIBFixture fixture)
		: base(fixture)
	{ }

	[Fact]
	public override void DbContext_complex_expression_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_complex_expression_is_parameterized();
	}

	[Fact]
	public override void DbContext_field_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_field_is_parameterized();
	}

	[Fact]
	public override void DbContext_list_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_list_is_parameterized();
	}

	[Fact]
	public override void DbContext_method_call_chain_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_method_call_chain_is_parameterized();
	}

	[Fact]
	public override void DbContext_method_call_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_method_call_is_parameterized();
	}

	[Fact]
	public override void DbContext_property_based_filter_does_not_short_circuit()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_property_based_filter_does_not_short_circuit();
	}

	[Fact]
	public override void DbContext_property_chain_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_property_chain_is_parameterized();
	}

	[Fact]
	public override void DbContext_property_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_property_is_parameterized();
	}

	[Fact]
	public override void DbContext_property_method_call_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_property_method_call_is_parameterized();
	}

	[Fact]
	public override void DbContext_property_parameter_does_not_clash_with_closure_parameter_name()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.DbContext_property_parameter_does_not_clash_with_closure_parameter_name();
	}

	[Fact]
	public override void EntityTypeConfiguration_DbContext_field_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.EntityTypeConfiguration_DbContext_field_is_parameterized();
	}

	[Fact]
	public override void EntityTypeConfiguration_DbContext_method_call_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.EntityTypeConfiguration_DbContext_method_call_is_parameterized();
	}

	[Fact]
	public override void EntityTypeConfiguration_DbContext_property_chain_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.EntityTypeConfiguration_DbContext_property_chain_is_parameterized();
	}

	[Fact]
	public override void EntityTypeConfiguration_DbContext_property_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.EntityTypeConfiguration_DbContext_property_is_parameterized();
	}

	[Fact]
	public override void Extension_method_DbContext_field_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Extension_method_DbContext_field_is_parameterized();
	}

	[Fact]
	public override void Extension_method_DbContext_property_chain_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Extension_method_DbContext_property_chain_is_parameterized();
	}

	[Fact]
	public override void Local_method_DbContext_field_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Local_method_DbContext_field_is_parameterized();
	}

	[Fact]
	public override void Local_static_method_DbContext_property_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Local_static_method_DbContext_property_is_parameterized();
	}

	[Fact]
	public override void Local_variable_from_OnModelCreating_can_throw_exception()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Local_variable_from_OnModelCreating_can_throw_exception();
	}

	[Fact]
	public override void Local_variable_from_OnModelCreating_is_inlined()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Local_variable_from_OnModelCreating_is_inlined();
	}

	[Fact]
	public override void Method_parameter_is_inlined()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Method_parameter_is_inlined();
	}

	[Fact]
	public override void Remote_method_DbContext_property_method_call_is_parameterized()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Remote_method_DbContext_property_method_call_is_parameterized();
	}

	[Fact]
	public override void Static_member_from_dbContext_is_inlined()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Static_member_from_dbContext_is_inlined();
	}

	[Fact]
	public override void Static_member_from_non_dbContext_is_inlined()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Static_member_from_non_dbContext_is_inlined();
	}

	[Fact]
	public override void Using_Context_set_method_in_filter_works()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Using_Context_set_method_in_filter_works();
	}

	[Fact]
	public override void Using_DbSet_in_filter_works()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Using_DbSet_in_filter_works();
	}

	[Fact]
	public override void Using_multiple_context_in_filter_parametrize_only_current_context()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Using_multiple_context_in_filter_parametrize_only_current_context();
	}

	[Fact]
	public override void Using_multiple_entities_with_filters_reuses_parameters()
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return;
		base.Using_multiple_entities_with_filters_reuses_parameters();
	}

	public class QueryFilterFuncletizationIBFixture : QueryFilterFuncletizationRelationalFixture
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SimpleTableNames(modelBuilder);
			ModelHelpers.SetStringLengths(modelBuilder);
			ModelHelpers.SetPrimaryKeyGeneration(modelBuilder, IBValueGenerationStrategy.IdentityColumn);
		}
	}
}