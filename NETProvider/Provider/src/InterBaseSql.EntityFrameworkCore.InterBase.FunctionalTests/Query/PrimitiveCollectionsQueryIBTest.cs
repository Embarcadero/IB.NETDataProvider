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

using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class PrimitiveCollectionsQueryIBTest : PrimitiveCollectionsQueryRelationalTestBase<PrimitiveCollectionsQueryIBTest.PrimitiveCollectionsQueryIBFixture>
{
	public PrimitiveCollectionsQueryIBTest(PrimitiveCollectionsQueryIBFixture fixture)
		: base(fixture)
	{ }

	[ConditionalFact]
	public virtual async Task Json_representation_of_bool_array()
	{
		await using var context = CreateContext();

		Assert.Equal(
			"[true,false]",
			await context.Database.SqlQuery<string>($@"SELECT ""Bools"" AS ""Value"" FROM ""PrimitiveCollectionsEntity"" WHERE ""Id"" = 1")
				.SingleAsync());
	}

	[NotSupportedOnInterBaseFact]
	public override void Parameter_collection_in_subquery_and_Convert_as_compiled_query()
	{
		base.Parameter_collection_in_subquery_and_Convert_as_compiled_query();
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_in_subquery_Union_another_parameter_collection_as_compiled_query(bool async)
	{
		return base.Parameter_collection_in_subquery_Union_another_parameter_collection_as_compiled_query(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Column_collection_of_strings_contains_null(bool async)
	{
		return base.Column_collection_of_strings_contains_null(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Inline_collection_Count_with_one_value(bool async)
	{
		return base.Inline_collection_Count_with_one_value(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Inline_collection_Count_with_two_values(bool async)
    {
        return base.Inline_collection_Count_with_two_values(async);
    }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Inline_collection_Count_with_three_values(bool async)
    {
        return base.Inline_collection_Count_with_three_values(async);
    }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_Count(bool async)
	{
		return base.Parameter_collection_Count(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_of_ints_Contains_int(bool async)
	{
		return base.Parameter_collection_of_ints_Contains_int(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_of_nullable_ints_Contains_int(bool async)
	{
		return base.Parameter_collection_of_nullable_ints_Contains_int(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_of_nullable_ints_Contains_nullable_int(bool async)
	{
		return base.Parameter_collection_of_nullable_ints_Contains_nullable_int(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_of_strings_Contains_string(bool async)
	{
		return base.Parameter_collection_of_strings_Contains_string(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_of_strings_Contains_nullable_string(bool async)
	{
		return base.Parameter_collection_of_strings_Contains_nullable_string(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_of_DateTimes_Contains(bool async)
	{
		return base.Parameter_collection_of_DateTimes_Contains(async);
	}

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Parameter_collection_of_bools_Contains(bool async)
    {
        return base.Parameter_collection_of_bools_Contains(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Parameter_collection_of_enums_Contains(bool async)
    {
        return base.Parameter_collection_of_enums_Contains(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Parameter_collection_null_Contains(bool async)
    {
        return base.Parameter_collection_null_Contains(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_of_ints_Contains(bool async)
    {
        return base.Column_collection_of_ints_Contains(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_of_nullable_ints_Contains(bool async)
    {
        return base.Column_collection_of_nullable_ints_Contains(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_of_nullable_ints_Contains_null(bool async)
    {
        return base.Column_collection_of_nullable_ints_Contains_null(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_of_nullable_strings_contains_null(bool async)
    {
        return base.Column_collection_of_nullable_strings_contains_null(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_of_bools_Contains(bool async)
    {
        return base.Column_collection_of_bools_Contains(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Count_method(bool async)
    {
        return base.Column_collection_Count_method(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Length(bool async)
    {
        return base.Column_collection_Length(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_index_int(bool async)
    {
        return base.Column_collection_index_int(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_index_string(bool async)
    {
        return base.Column_collection_index_string(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_index_datetime(bool async)
    {
        return base.Column_collection_index_datetime(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_index_beyond_end(bool async)
    {
        return base.Column_collection_index_beyond_end(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Nullable_reference_column_collection_index_equals_nullable_column(bool async)
    {
        return base.Nullable_reference_column_collection_index_equals_nullable_column(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Non_nullable_reference_column_collection_index_equals_nullable_column(bool async)
    {
        return base.Non_nullable_reference_column_collection_index_equals_nullable_column(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Inline_collection_index_Column(bool async)
    {
        return base.Inline_collection_index_Column(async);
    }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_index_Column_equal_Column(bool async)
	{
		return base.Parameter_collection_index_Column_equal_Column(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_index_Column_equal_constant(bool async)
	{
		return base.Parameter_collection_index_Column_equal_constant(async);
	}

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_ElementAt(bool async)
    {
        return base.Column_collection_ElementAt(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Skip(bool async)
    {
        return base.Column_collection_Skip(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Take(bool async)
    {
        return base.Column_collection_Take(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Skip_Take(bool async)
    {
        return base.Column_collection_Skip_Take(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_OrderByDescending_ElementAt(bool async)
    {
        return base.Column_collection_OrderByDescending_ElementAt(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Any(bool async)
    {
        return base.Column_collection_Any(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Distinct(bool async)
    {
        return base.Column_collection_Distinct(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Join_parameter_collection(bool async)
    {
        return base.Column_collection_Join_parameter_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Inline_collection_Join_ordered_column_collection(bool async)
    {
        return base.Inline_collection_Join_ordered_column_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Parameter_collection_Concat_column_collection(bool async)
    {
        return base.Parameter_collection_Concat_column_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Union_parameter_collection(bool async)
    {
        return base.Column_collection_Union_parameter_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_Intersect_inline_collection(bool async)
    {
        return base.Column_collection_Intersect_inline_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Inline_collection_Except_column_collection(bool async)
    {
        return base.Inline_collection_Except_column_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_in_subquery_Union_column_collection_as_compiled_query(bool async)
	{
		return base.Parameter_collection_in_subquery_Union_column_collection_as_compiled_query(async);
	}

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Parameter_collection_in_subquery_Union_column_collection(bool async)
    {
        return base.Parameter_collection_in_subquery_Union_column_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Parameter_collection_in_subquery_Union_column_collection_nested(bool async)
    {
        return base.Parameter_collection_in_subquery_Union_column_collection_nested(async);
    }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Parameter_collection_in_subquery_Count_as_compiled_query(bool async)
	{
		return base.Parameter_collection_in_subquery_Count_as_compiled_query(async);
	}

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Column_collection_in_subquery_Union_parameter_collection(bool async)
    {
        return base.Column_collection_in_subquery_Union_parameter_collection(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_collection_of_ints_ordered(bool async)
    {
        return base.Project_collection_of_ints_ordered(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_collection_of_datetimes_filtered(bool async)
    {
        return base.Project_collection_of_datetimes_filtered(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_collection_of_nullable_ints_with_paging(bool async)
    {
        return base.Project_collection_of_nullable_ints_with_paging(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_collection_of_nullable_ints_with_paging2(bool async)
    {
        return base.Project_collection_of_nullable_ints_with_paging2(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_collection_of_nullable_ints_with_paging3(bool async)
    {
        return base.Project_collection_of_nullable_ints_with_paging3(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_collection_of_ints_with_distinct(bool async)
    {
        return base.Project_collection_of_ints_with_distinct(async);
    }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Project_empty_collection_of_nullables_and_collection_only_containing_nulls(bool async)
	{
		return base.Project_empty_collection_of_nullables_and_collection_only_containing_nulls(async);
	}

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_multiple_collections(bool async)
    {
        return base.Project_multiple_collections(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Project_primitive_collections_element(bool async)
    {
        return base.Project_primitive_collections_element(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Nested_contains_with_Lists_and_no_inferred_type_mapping(bool async)
    {
        return base.Nested_contains_with_Lists_and_no_inferred_type_mapping(async);
    }

	[NotSupportedOnInterBaseTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Nested_contains_with_arrays_and_no_inferred_type_mapping(bool async)
    {
        return base.Nested_contains_with_arrays_and_no_inferred_type_mapping(async);
    }

	PrimitiveCollectionsContext CreateContext()
	{
		return Fixture.CreateContext();
	}

	public class PrimitiveCollectionsQueryIBFixture : PrimitiveCollectionsQueryFixtureBase
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;
	}
}
