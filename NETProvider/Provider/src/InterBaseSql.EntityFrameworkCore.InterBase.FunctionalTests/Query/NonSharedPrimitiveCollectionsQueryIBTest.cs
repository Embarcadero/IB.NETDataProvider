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
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NonSharedPrimitiveCollectionsQueryIBTest : NonSharedPrimitiveCollectionsQueryRelationalTestBase
{
	[NotSupportedOnInterBaseFact]
	public override Task Array_of_string()
	{
		return base.Array_of_string();
	}

	[NotSupportedOnInterBaseFact]
	public override Task Array_of_int()
	{
		return base.Array_of_int();
	}

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_long()
    {
        return base.Array_of_long();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_short()
    {
        return base.Array_of_short();
    }

	[NotSupportedOnInterBaseFact]
	public override Task Array_of_byte()
	{
		return base.Array_of_byte();
	}

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_double()
    {
        return base.Array_of_double();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_float()
    {
        return base.Array_of_float();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_decimal()
    {
        return base.Array_of_decimal();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_DateTime()
    {
        return base.Array_of_DateTime();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_DateTime_with_milliseconds()
    {
        return base.Array_of_DateTime_with_milliseconds();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_DateTime_with_microseconds()
    {
        return base.Array_of_DateTime_with_microseconds();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_DateOnly()
    {
        return base.Array_of_DateOnly();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_TimeOnly()
    {
        return base.Array_of_TimeOnly();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_TimeOnly_with_milliseconds()
    {
        return base.Array_of_TimeOnly_with_milliseconds();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_TimeOnly_with_microseconds()
    {
        return base.Array_of_TimeOnly_with_microseconds();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_DateTimeOffset()
    {
        return base.Array_of_DateTimeOffset();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_bool()
    {
        return base.Array_of_bool();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_Guid()
    {
        return base.Array_of_Guid();
    }

	[NotSupportedOnInterBaseFact]
    public override Task Array_of_byte_array()
    {
        return base.Array_of_byte_array();
    }

	[NotSupportedOnInterBaseFact]
	public override Task Array_of_enum()
	{
		return base.Array_of_enum();
	}

	[NotSupportedOnInterBaseFact]
	public override Task Array_of_array_is_not_supported()
	{
		return base.Array_of_array_is_not_supported();
	}

	[NotSupportedOnInterBaseFact]
    public override Task Multidimensional_array_is_not_supported()
    {
        return base.Multidimensional_array_is_not_supported();
    }

	[NotSupportedOnInterBaseFact]
	public override Task Column_with_custom_converter()
	{
		return base.Column_with_custom_converter();
	}

	[NotSupportedOnInterBaseFact]
	public override Task Parameter_with_inferred_value_converter()
	{
		return base.Parameter_with_inferred_value_converter();
	}

	[NotSupportedOnInterBaseFact]
	public override Task Constant_with_inferred_value_converter()
	{
		return base.Constant_with_inferred_value_converter();
	}

	[NotSupportedOnInterBaseFact]
	public override Task Inline_collection_in_query_filter()
	{
		return base.Inline_collection_in_query_filter();
	}

	[NotSupportedOnInterBaseFact]
    public override Task Column_collection_inside_json_owned_entity()
    {
        return base.Column_collection_inside_json_owned_entity();
    }

	protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;
}
