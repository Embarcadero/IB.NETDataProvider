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

using System.Linq;
using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class TPCGearsOfWarQueryIBTest : TPCGearsOfWarQueryRelationalTestBase<TPCGearsOfWarQueryIBFixture>
{
	public TPCGearsOfWarQueryIBTest(TPCGearsOfWarQueryIBFixture fixture)
		: base(fixture)
	{ }

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task ToString_boolean_property_non_nullable(bool async)
	{
		return AssertQuery(
			async,
			ss => ss.Set<Weapon>().Select(w => w.IsAutomatic.ToString()), elementAsserter: (lhs, rhs) => { Assert.True(lhs.Equals(rhs, System.StringComparison.OrdinalIgnoreCase)); });
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task ToString_boolean_property_nullable(bool async)
	{
		return AssertQuery(
			async,
			ss => ss.Set<LocustHorde>().Select(lh => lh.Eradicated.ToString()), elementAsserter: (lhs, rhs) => { Assert.True(lhs.Equals(rhs, System.StringComparison.OrdinalIgnoreCase)); });
	}

	[Theory(Skip = "Different implicit ordering on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Group_by_on_StartsWith_with_null_parameter_as_argument(bool async)
	{
		return base.Group_by_on_StartsWith_with_null_parameter_as_argument(async);
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

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_distinct_not_projecting_identifier_column(bool async)
	{
		return base.Correlated_collection_with_distinct_not_projecting_identifier_column(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_distinct_projecting_identifier_column(bool async)
	{
		return base.Correlated_collection_with_distinct_projecting_identifier_column(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(bool async)
	{
		return base.Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(bool async)
	{
		return base.Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(bool async)
	{
		return base.Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(bool async)
	{
		return base.Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collection_with_inner_collection_references_element_two_levels_up(bool async)
	{
		return base.Correlated_collection_with_inner_collection_references_element_two_levels_up(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool async)
	{
		return base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool async)
	{
		return base.Correlated_collections_inner_subquery_selector_references_outer_qsre(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool async)
	{
		return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool async)
	{
		return base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Correlated_collections_with_Distinct(bool async)
	{
		return base.Correlated_collections_with_Distinct(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool async)
	{
		return base.DateTimeOffset_Contains_Less_than_Greater_than(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_Date_returns_datetime(bool async)
	{
		return base.DateTimeOffset_Date_returns_datetime(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffsetNow_minus_timespan(bool async)
	{
		return base.DateTimeOffsetNow_minus_timespan(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task First_on_byte_array(bool async)
	{
		return base.First_on_byte_array(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool async)
	{
		return base.Outer_parameter_in_group_join_with_DefaultIfEmpty(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Outer_parameter_in_join_key(bool async)
	{
		return base.Outer_parameter_in_join_key(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Outer_parameter_in_join_key_inner_and_outer(bool async)
	{
		return base.Outer_parameter_in_join_key_inner_and_outer(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(bool async)
	{
		return base.SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(async);
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
	public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(bool async)
	{
		return base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(bool async)
	{
		return base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Subquery_inside_Take_argument(bool async)
	{
		return base.Subquery_inside_Take_argument(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_AddDays(bool async)
	{
		return base.Where_DateOnly_AddDays(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_AddMonths(bool async)
	{
		return base.Where_DateOnly_AddMonths(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_AddYears(bool async)
	{
		return base.Where_DateOnly_AddYears(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_Day(bool async)
	{
		return base.Where_DateOnly_Day(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_DayOfWeek(bool async)
	{
		return base.Where_DateOnly_DayOfWeek(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_DayOfYear(bool async)
	{
		return base.Where_DateOnly_DayOfYear(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_Month(bool async)
	{
		return base.Where_DateOnly_Month(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_DateOnly_Year(bool async)
	{
		return base.Where_DateOnly_Year(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_date_component(bool async)
	{
		return base.Where_datetimeoffset_date_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_day_component(bool async)
	{
		return base.Where_datetimeoffset_day_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_dayofyear_component(bool async)
	{
		return base.Where_datetimeoffset_dayofyear_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_hour_component(bool async)
	{
		return base.Where_datetimeoffset_hour_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_millisecond_component(bool async)
	{
		return base.Where_datetimeoffset_millisecond_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_minute_component(bool async)
	{
		return base.Where_datetimeoffset_minute_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_month_component(bool async)
	{
		return base.Where_datetimeoffset_month_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_now(bool async)
	{
		return base.Where_datetimeoffset_now(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_second_component(bool async)
	{
		return base.Where_datetimeoffset_second_component(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_utcnow(bool async)
	{
		return base.Where_datetimeoffset_utcnow(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_datetimeoffset_year_component(bool async)
	{
		return base.Where_datetimeoffset_year_component(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_Add_TimeSpan(bool async)
	{
		return base.Where_TimeOnly_Add_TimeSpan(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_AddHours(bool async)
	{
		return base.Where_TimeOnly_AddHours(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_AddMinutes(bool async)
	{
		return base.Where_TimeOnly_AddMinutes(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_Hour(bool async)
	{
		return base.Where_TimeOnly_Hour(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_IsBetween(bool async)
	{
		return base.Where_TimeOnly_IsBetween(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_Millisecond(bool async)
	{
		return base.Where_TimeOnly_Millisecond(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_Minute(bool async)
	{
		return base.Where_TimeOnly_Minute(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_Second(bool async)
	{
		return base.Where_TimeOnly_Second(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_TimeOnly_subtract_TimeOnly(bool async)
	{
		return base.Where_TimeOnly_subtract_TimeOnly(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_to_unix_time_milliseconds(bool async)
	{
		return base.DateTimeOffset_to_unix_time_milliseconds(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task DateTimeOffset_to_unix_time_seconds(bool async)
	{
		return base.DateTimeOffset_to_unix_time_seconds(async);
	}

	[Theory(Skip = "Different implicit ordering on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool async)
	{
		return base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(async);
	}

	[Theory(Skip = "Different implicit ordering on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool async)
	{
		return base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(async);
	}
}
