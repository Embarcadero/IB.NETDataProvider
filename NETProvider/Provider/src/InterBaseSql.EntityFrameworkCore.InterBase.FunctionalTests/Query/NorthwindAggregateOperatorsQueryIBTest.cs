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
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NorthwindAggregateOperatorsQueryIBTest : NorthwindAggregateOperatorsQueryRelationalTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
{
	public NorthwindAggregateOperatorsQueryIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
		: base(fixture)
	{ }

	protected override bool CanExecuteQueryString => false;

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Multiple_collection_navigation_with_FirstOrDefault_chained(bool async)
	{
		return base.Multiple_collection_navigation_with_FirstOrDefault_chained(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Contains_with_local_enumerable_inline(bool async)
	{
		await Assert.ThrowsAsync<InvalidOperationException>(
			async () =>
				await base.Contains_with_local_enumerable_inline(async));
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Contains_with_local_enumerable_inline_closure_mix(bool async)
	{
		await Assert.ThrowsAsync<InvalidOperationException>(
			async () =>
				await base.Contains_with_local_enumerable_inline_closure_mix(async));
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Contains_with_local_anonymous_type_array_closure(bool async)
	{
		return AssertTranslationFailed(() => base.Contains_with_local_anonymous_type_array_closure(async));
	}

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Contains_with_local_tuple_array_closure(bool async)
	{
		return AssertTranslationFailed(() => base.Contains_with_local_tuple_array_closure(async));
	}

	[Theory(Skip = "Different math on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Average_over_max_subquery_is_client_eval(bool async)
	{
		return base.Average_over_max_subquery_is_client_eval(async);
	}

	[Theory(Skip = "Different math on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Average_over_nested_subquery_is_client_eval(bool async)
	{
		return base.Average_over_nested_subquery_is_client_eval(async);
	}

	[Theory(Skip = "Different math on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Sum_with_division_on_decimal(bool async)
	{
		return base.Sum_with_division_on_decimal(async);
	}

	[Theory(Skip = "Different math on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Contains_inside_Average_without_GroupBy(bool async)
	{
		return base.Contains_inside_Average_without_GroupBy(async);
	}
}
