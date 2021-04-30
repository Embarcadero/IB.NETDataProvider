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

using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query
{
	public class GearsOfWarQueryIBTest : GearsOfWarQueryTestBase<GearsOfWarQueryIBFixture>
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
	}
}
