﻿/*
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

using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class ComplexNavigationsCollectionsSplitSharedTypeQueryIBTest : ComplexNavigationsCollectionsSplitSharedTypeQueryRelationalTestBase<ComplexNavigationsSharedTypeQueryIBFixture>
{
	public ComplexNavigationsCollectionsSplitSharedTypeQueryIBTest(ComplexNavigationsSharedTypeQueryIBFixture fixture)
		: base(fixture)
	{ }

	[Theory(Skip = "Should fail, but not failing.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task SelectMany_with_navigation_and_Distinct_projecting_columns_including_join_key(bool async)
	{
		return base.SelectMany_with_navigation_and_Distinct_projecting_columns_including_join_key(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Complex_query_with_let_collection_projection_FirstOrDefault(bool async)
	{
		return base.Complex_query_with_let_collection_projection_FirstOrDefault(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Complex_query_with_let_collection_projection_FirstOrDefault_with_ToList_on_inner_and_outer(bool async)
	{
		return base.Complex_query_with_let_collection_projection_FirstOrDefault_with_ToList_on_inner_and_outer(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(bool async)
	{
		return base.Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(bool async)
	{
		return base.Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(bool async)
	{
		return base.Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Filtered_include_Take_with_another_Take_on_top_level(bool async)
	{
		return base.Filtered_include_Take_with_another_Take_on_top_level(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_FirstOrDefault_on_top_level(bool async)
	{
		return base.Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_FirstOrDefault_on_top_level(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_unordered_Take_on_top_level(bool async)
	{
		return base.Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_unordered_Take_on_top_level(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Projecting_collection_after_optional_reference_correlated_with_parent(bool async)
	{
		return base.Projecting_collection_after_optional_reference_correlated_with_parent(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Projecting_collection_with_group_by_after_optional_reference_correlated_with_parent(bool async)
	{
		return base.Projecting_collection_with_group_by_after_optional_reference_correlated_with_parent(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task SelectMany_with_predicate_and_DefaultIfEmpty_projecting_root_collection_element_and_another_collection(bool async)
	{
		return base.SelectMany_with_predicate_and_DefaultIfEmpty_projecting_root_collection_element_and_another_collection(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Skip_Take_Distinct_on_grouping_element(bool async)
	{
		return base.Skip_Take_Distinct_on_grouping_element(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Skip_Take_on_grouping_element_inside_collection_projection(bool async)
	{
		return base.Skip_Take_on_grouping_element_inside_collection_projection(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Skip_Take_on_grouping_element_with_reference_include(bool async)
	{
		return base.Skip_Take_on_grouping_element_with_reference_include(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Skip_Take_Select_collection_Skip_Take(bool async)
	{
		return base.Skip_Take_Select_collection_Skip_Take(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Take_Select_collection_Take(bool async)
	{
		return base.Take_Select_collection_Take(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Filtered_include_Skip_Take_with_another_Skip_Take_on_top_level(bool async)
	{
		return base.Filtered_include_Skip_Take_with_another_Skip_Take_on_top_level(async);
	}
}
