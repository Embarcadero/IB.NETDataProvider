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

using System.Linq;
using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.NullSemanticsModel;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NullSemanticsQueryIBTest : NullSemanticsQueryTestBase<NullSemanticsQueryIBFixture>
{
	public NullSemanticsQueryIBTest(NullSemanticsQueryIBFixture fixture)
		: base(fixture)
	{ }

	public override async Task Bool_equal_nullable_bool_compared_to_null(bool async)
	{
		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => true == (e.NullableBoolA == null)));

		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => false == (e.NullableBoolA != null)));
	}
	public override async Task Bool_equal_nullable_bool_HasValue(bool async)
	{
		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => true == e.NullableBoolA.HasValue));

		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => false == e.NullableBoolA.HasValue));

		// Not valid for IB
		//await AssertQuery(
		//	async,
		//	ss => ss.Set<NullSemanticsEntity1>().Where(e => e.BoolB == e.NullableBoolA.HasValue));
	}
	public override async Task Bool_logical_operation_with_nullable_bool_HasValue(bool async)
	{
		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => true || e.NullableBoolA.HasValue));

		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => false && e.NullableBoolA.HasValue),
			assertEmpty: true);

		// IB not supported
		//await AssertQuery(
		//	async,
		//	ss => ss.Set<NullSemanticsEntity1>().Where(e => e.BoolB | e.NullableBoolA.HasValue));
	}

	public override async Task Bool_not_equal_nullable_bool_HasValue(bool async)
	{
		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => true != e.NullableBoolA.HasValue));

		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => false != e.NullableBoolA.HasValue));

		//await AssertQuery(
		//	async,
		//	ss => ss.Set<NullSemanticsEntity1>().Where(e => e.BoolB != e.NullableBoolA.HasValue));
	}

	public override async Task Bool_not_equal_nullable_bool_compared_to_null(bool async)
	{
		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => true != (e.NullableBoolA == null)));

		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => false != (e.NullableBoolA != null)));
	}

	public override async Task Bool_not_equal_nullable_int_HasValue(bool async)
	{
		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => true != e.NullableIntA.HasValue));

		await AssertQuery(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => false != e.NullableIntA.HasValue));

		//await AssertQuery(
		//	async,
		//	ss => ss.Set<NullSemanticsEntity1>().Where(e => e.BoolB != e.NullableIntA.HasValue));
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Compare_complex_equal_equal_equal(bool async)
	{
		await base.Compare_complex_equal_equal_equal(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Compare_complex_equal_not_equal_equal(bool async)
	{
		await base.Compare_complex_equal_not_equal_equal(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Compare_complex_not_equal_equal_equal(bool async)
	{
		await base.Compare_complex_not_equal_equal_equal(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Compare_complex_not_equal_equal_not_equal(bool async)
	{
		await base.Compare_complex_not_equal_equal_not_equal(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Compare_complex_not_equal_not_equal_equal(bool async)
	{
		await base.Compare_complex_equal_equal_equal(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Compare_complex_not_equal_not_equal_not_equal(bool async)
	{
		await base.Compare_complex_not_equal_not_equal_not_equal(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Comparison_compared_to_null_check_on_bool(bool async)
	{
		await base.Comparison_compared_to_null_check_on_bool(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Empty_subquery_with_contains_returns_false(bool async)
	{
		await base.Empty_subquery_with_contains_returns_false(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional(bool async)
	{
		await base.Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_multiple(bool async)
	{
		await base.Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_multiple(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_negative(bool async)
	{
		await base.Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_negative(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_with_setup(bool async)
	{
		await base.Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_with_setup(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_nested(bool async)
	{
		await base.Is_null_on_column_followed_by_OrElse_optimizes_nullability_conditional_nested(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Multiple_contains_calls_get_combined_into_one_for_relational_null_semantics(bool async)
	{
		await base.Multiple_contains_calls_get_combined_into_one_for_relational_null_semantics(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Multiple_negated_contains_calls_get_combined_into_one_for_relational_null_semantics(bool async)
	{
		await base.Multiple_negated_contains_calls_get_combined_into_one_for_relational_null_semantics(async);
	}

	[NotSupportedOrderByInterBaseTheoryAttribute]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Null_comparison_in_order_by_with_relational_nulls(bool async)
	{
		await base.Null_comparison_in_order_by_with_relational_nulls(async);
	}

	[NotSupportedOrderByInterBaseTheoryAttribute]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Null_comparison_in_selector_with_relational_nulls(bool async)
	{
		await base.Null_comparison_in_selector_with_relational_nulls(async);
	}

	//We can't client-evaluate Like (for the expected results).
	// However, since the test data has no LIKE wildcards, it effectively functions like equality - except that 'null like null' returns
	// false instead of true. So we have this "lite" implementation which doesn't support wildcards.
	private bool LikeLite(string s, string pattern)
		=> s == pattern && s is not null && pattern is not null;

	public override async Task Like_with_escape_char(bool async)
	{
		await AssertQueryScalar(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => EF.Functions.Like(e.StringA, e.StringB, "\\")).Select(e => e.Id),
			ss => ss.Set<NullSemanticsEntity1>().Where(e => LikeLite(e.StringA, e.StringB)).Select(e => e.Id));

		await AssertQueryScalar(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => !EF.Functions.Like(e.StringA, e.StringB, "\\")).Select(e => e.Id),
			ss => ss.Set<NullSemanticsEntity1>().Where(e => !LikeLite(e.StringA, e.StringB)).Select(e => e.Id));

		//await AssertQueryScalar(
		//	async,
		//	ss => ss.Set<NullSemanticsEntity1>().Where(e => EF.Functions.Like(e.StringA, e.StringB, null)).Select(e => e.Id),
		//	ss => ss.Set<NullSemanticsEntity1>().Where(e => false).Select(e => e.Id),
		//	assertEmpty: true);

		await AssertQueryScalar(
			async,
			ss => ss.Set<NullSemanticsEntity1>().Where(e => !EF.Functions.Like(e.StringA, e.StringB, null)).Select(e => e.Id),
			ss => ss.Set<NullSemanticsEntity1>().Where(e => true).Select(e => e.Id));
	}


	protected override NullSemanticsContext CreateContext(bool useRelationalNulls = false)
	{
		var options = new DbContextOptionsBuilder(Fixture.CreateOptions());
		if (useRelationalNulls)
		{
			new IBDbContextOptionsBuilder(options).UseRelationalNulls();
		}
		var context = new NullSemanticsContext(options.Options);
		context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		return context;
	}
}