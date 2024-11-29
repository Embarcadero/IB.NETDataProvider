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

using System;
using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NorthwindGroupByQueryIBTest : NorthwindGroupByQueryTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
{
	public NorthwindGroupByQueryIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
		: base(fixture)
	{ }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Select_uncorrelated_collection_with_groupby_works(bool async)
	{
		return base.Select_uncorrelated_collection_with_groupby_works(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Select_uncorrelated_collection_with_groupby_multiple_collections_work(bool async)
	{
		return base.Select_uncorrelated_collection_with_groupby_multiple_collections_work(async);
	}

	[Theory(Skip = "efcore#19027")]
	[MemberData(nameof(IsAsyncData))]
	public override Task GroupBy_scalar_subquery(bool async)
	{
		return base.GroupBy_scalar_subquery(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task AsEnumerable_in_subquery_for_GroupBy(bool async)
	{
		return base.AsEnumerable_in_subquery_for_GroupBy(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Complex_query_with_group_by_in_subquery5(bool async)
	{
		return base.Complex_query_with_group_by_in_subquery5(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Complex_query_with_groupBy_in_subquery1(bool async)
	{
		return base.Complex_query_with_groupBy_in_subquery1(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Complex_query_with_groupBy_in_subquery2(bool async)
	{
		return base.Complex_query_with_groupBy_in_subquery2(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Complex_query_with_groupBy_in_subquery3(bool async)
	{
		return base.Complex_query_with_groupBy_in_subquery3(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Complex_query_with_groupBy_in_subquery4(bool async)
	{
		return base.Complex_query_with_groupBy_in_subquery4(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Select_nested_collection_with_groupby(bool async)
	{
		return base.Select_nested_collection_with_groupby(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Select_uncorrelated_collection_with_groupby_when_outer_is_distinct(bool async)
	{
		return base.Select_uncorrelated_collection_with_groupby_when_outer_is_distinct(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Select_correlated_collection_after_GroupBy_aggregate_when_identifier_changes_to_complex(bool async)
	{
		var message = (await Assert.ThrowsAsync<InvalidOperationException>(
			() => base.Select_correlated_collection_after_GroupBy_aggregate_when_identifier_changes_to_complex(async))).Message;

		Assert.Equal(RelationalStrings.InsufficientInformationToIdentifyElementOfCollectionJoin, message);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task GroupBy_aggregate_from_multiple_query_in_same_projection(bool async)
	{
		return base.GroupBy_aggregate_from_multiple_query_in_same_projection(async);
	}
}
