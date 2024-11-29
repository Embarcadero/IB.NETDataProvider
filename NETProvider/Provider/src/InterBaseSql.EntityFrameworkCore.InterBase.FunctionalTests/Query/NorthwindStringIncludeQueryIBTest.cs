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
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NorthwindStringIncludeQueryIBTest : NorthwindStringIncludeQueryTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
{
	public NorthwindStringIncludeQueryIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
		: base(fixture)
	{ }

	[Theory(Skip = "Different implicit ordering on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Repro9735(bool async)
	{
		return base.Repro9735(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_collection_OrderBy_empty_list_contains(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Include_collection_OrderBy_empty_list_contains(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_collection_OrderBy_empty_list_does_not_contains(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Include_collection_OrderBy_empty_list_does_not_contains(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_collection_OrderBy_list_contains(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Include_collection_OrderBy_list_contains(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_duplicate_collection_result_operator(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Include_duplicate_collection_result_operator(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_duplicate_collection_result_operator2(bool async)
	{
		var ibTestStore = (IBTestStore)Fixture.TestStore;
		if (ibTestStore.ServerLessThan4())
			return Task.CompletedTask;
		return base.Include_duplicate_collection_result_operator2(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_collection_with_cross_apply_with_filter(bool async)
	{
		return base.Include_collection_with_cross_apply_with_filter(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_collection_with_outer_apply_with_filter_non_equality(bool async)
	{
		return base.Include_collection_with_outer_apply_with_filter_non_equality(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Include_collection_with_outer_apply_with_filter(bool async)
	{
		return base.Include_collection_with_outer_apply_with_filter(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Include_collection_with_last_no_orderby(bool async)
	{
		Assert.Equal(
			RelationalStrings.LastUsedWithoutOrderBy(nameof(Enumerable.Last)),
			(await Assert.ThrowsAsync<InvalidOperationException>(
				() => base.Include_collection_with_last_no_orderby(async))).Message);
	}

	[LongExecutionTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Join_Include_reference_GroupBy_Select(bool async)
	{
		return base.Join_Include_reference_GroupBy_Select(async);
	}

	[LongExecutionTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task SelectMany_Include_collection_GroupBy_Select(bool async)
	{
		return base.SelectMany_Include_collection_GroupBy_Select(async);
	}

	[LongExecutionTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task SelectMany_Include_reference_GroupBy_Select(bool async)
	{
		return base.SelectMany_Include_reference_GroupBy_Select(async);
	}
}
