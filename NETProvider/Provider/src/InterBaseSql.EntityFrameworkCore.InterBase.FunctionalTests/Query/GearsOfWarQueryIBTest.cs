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

using System.Linq;
using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class GearsOfWarQueryIBTest : GearsOfWarQueryRelationalTestBase<GearsOfWarQueryIBFixture>
{
	public GearsOfWarQueryIBTest(GearsOfWarQueryIBFixture fixture)
		: base(fixture)
	{ }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_DateAdd_AddDays(bool isAsync)
	{
		return base.DateTimeOffset_DateAdd_AddDays(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_DateAdd_AddHours(bool isAsync)
	{
		return base.DateTimeOffset_DateAdd_AddHours(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_DateAdd_AddMilliseconds(bool isAsync)
	{
		return base.DateTimeOffset_DateAdd_AddMilliseconds(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_DateAdd_AddMinutes(bool isAsync)
	{
		return base.DateTimeOffset_DateAdd_AddMinutes(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_DateAdd_AddMonths(bool isAsync)
	{
		return base.DateTimeOffset_DateAdd_AddMonths(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_DateAdd_AddSeconds(bool isAsync)
	{
		return base.DateTimeOffset_DateAdd_AddSeconds(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_DateAdd_AddYears(bool isAsync)
	{
		return base.DateTimeOffset_DateAdd_AddYears(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_date_component(bool isAsync)
	{
		return base.Where_datetimeoffset_date_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_dayofyear_component(bool isAsync)
	{
		return base.Where_datetimeoffset_dayofyear_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_day_component(bool isAsync)
	{
		return base.Where_datetimeoffset_day_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_hour_component(bool isAsync)
	{
		return base.Where_datetimeoffset_hour_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_millisecond_component(bool isAsync)
	{
		return base.Where_datetimeoffset_millisecond_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_minute_component(bool isAsync)
	{
		return base.Where_datetimeoffset_minute_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_month_component(bool isAsync)
	{
		return base.Where_datetimeoffset_month_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_now(bool isAsync)
	{
		return base.Where_datetimeoffset_now(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_second_component(bool isAsync)
	{
		return base.Where_datetimeoffset_second_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_utcnow(bool isAsync)
	{
		return base.Where_datetimeoffset_utcnow(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_year_component(bool isAsync)
	{
		return base.Where_datetimeoffset_year_component(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool isAsync)
	{
		return base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool isAsync)
	{
		return base.Correlated_collections_inner_subquery_selector_references_outer_qsre(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool isAsync)
	{
		return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool isAsync)
	{
		return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool isAsync)
	{
		return base.DateTimeOffset_Contains_Less_than_Greater_than(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool isAsync)
	{
		return base.Outer_parameter_in_group_join_with_DefaultIfEmpty(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Outer_parameter_in_join_key(bool isAsync)
	{
		return base.Outer_parameter_in_join_key(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Outer_parameter_in_join_key_inner_and_outer(bool isAsync)
	{
		return base.Outer_parameter_in_join_key_inner_and_outer(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_collection_navigation_nested_with_take_composite_key(bool isAsync)
	{
		return base.Project_collection_navigation_nested_with_take_composite_key(isAsync);
	}

	[Theory(Skip = "Different implicit ordering on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool isAsync)
	{
		return base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(isAsync);
	}

	[Theory(Skip = "Different implicit ordering on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool isAsync)
	{
		return base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(isAsync);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(bool async)
	{
		return base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(bool async)
	{
		return base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(bool async)
	{
		return base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(bool async)
	{
		return base.SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(bool async)
	{
		return base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Array_access_on_byte_array(bool async)
	{
		return base.Array_access_on_byte_array(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_after_distinct_3_levels(bool async)
	{
		return base.Correlated_collection_after_distinct_3_levels(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_via_SelectMany_with_Distinct_missing_indentifying_columns_in_projection(bool async)
	{
		return base.Correlated_collection_via_SelectMany_with_Distinct_missing_indentifying_columns_in_projection(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task ToString_boolean_property_non_nullable(bool async)
	{
		return AssertQuery(async, ss => ss.Set<Weapon>().Select(w => w.IsAutomatic.ToString()), elementAsserter: (lhs, rhs) => { Assert.True(lhs.Equals(rhs, System.StringComparison.InvariantCultureIgnoreCase)); });
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task ToString_boolean_property_nullable(bool async)
	{
		return AssertQuery(async, ss => ss.Set<LocustHorde>().Select(lh => lh.Eradicated.ToString()), elementAsserter: (lhs, rhs) => { Assert.True(lhs.Equals(rhs, System.StringComparison.InvariantCultureIgnoreCase)); });
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_inner_collection_references_element_two_levels_up(bool async)
	{
		return base.Correlated_collection_with_inner_collection_references_element_two_levels_up(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(bool async)
	{
		return base.Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(bool async)
	{
		return base.Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(bool async)
	{
		return base.Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_distinct_projecting_identifier_column(bool async)
	{
		return base.Correlated_collection_with_distinct_projecting_identifier_column(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_distinct_not_projecting_identifier_column(bool async)
	{
		return base.Correlated_collection_with_distinct_not_projecting_identifier_column(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_with_Distinct(bool async)
	{
		return base.Correlated_collections_with_Distinct(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task First_on_byte_array(bool async)
	{
		return base.First_on_byte_array(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_subtract_TimeOnly(bool async)
	{
		return base.Where_TimeOnly_subtract_TimeOnly(async);
	}

	[Theory(Skip = "NETProvider#1008")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_IsBetween(bool async)
	{
		return base.Where_TimeOnly_IsBetween(async);
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_AddMinutes(bool async)
	{
		return base.Where_TimeOnly_AddMinutes(async);
	}

	[Theory(Skip = "NETProvider#1009")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_Add_TimeSpan(bool async)
	{
		return base.Where_TimeOnly_Add_TimeSpan(async);
	}
}
