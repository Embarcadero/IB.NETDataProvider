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
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NorthwindFunctionsQueryIBTest : NorthwindFunctionsQueryRelationalTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
{
	public NorthwindFunctionsQueryIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
		: base(fixture)
	{ }

	protected override bool CanExecuteQueryString => false;

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToBoolean(bool async)
	{
		return base.Convert_ToBoolean(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToByte(bool async)
	{
		return base.Convert_ToByte(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToDecimal(bool async)
	{
		return base.Convert_ToDecimal(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToDouble(bool async)
	{
		return base.Convert_ToDouble(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToInt16(bool async)
	{
		return base.Convert_ToInt16(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToInt32(bool async)
	{
		return base.Convert_ToInt32(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToInt64(bool async)
	{
		return base.Convert_ToInt64(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Convert_ToString(bool async)
	{
		return base.Convert_ToString(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Trim_with_char_array_argument_in_predicate(bool async)
	{
		return base.Trim_with_char_array_argument_in_predicate(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task TrimEnd_with_char_array_argument_in_predicate(bool async)
	{
		return base.TrimEnd_with_char_array_argument_in_predicate(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task TrimStart_with_char_array_argument_in_predicate(bool async)
	{
		return base.TrimStart_with_char_array_argument_in_predicate(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_math_degrees(bool async)
	{
		return base.Where_math_degrees(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_math_radians(bool async)
	{
		return base.Where_mathf_radians(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_degrees(bool async)
	{
		return base.Where_math_degrees(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_radians(bool async)
	{
		return base.Where_mathf_radians(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Regex_IsMatch_MethodCall(bool async)
	{
		return base.Regex_IsMatch_MethodCall(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Regex_IsMatch_MethodCall_constant_input(bool async)
	{
		return base.Regex_IsMatch_MethodCall_constant_input(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_abs1(bool async)
	{
		return base.Where_mathf_abs1(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_acos(bool async)
	{
		return base.Where_mathf_acos(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_asin(bool async)
	{
		return base.Where_mathf_asin(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_atan(bool async)
	{
		return base.Where_mathf_atan(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_atan2(bool async)
	{
		return base.Where_mathf_atan2(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_ceiling1(bool async)
	{
		return base.Where_mathf_ceiling1(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_cos(bool async)
	{
		return base.Where_mathf_cos(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_exp(bool async)
	{
		return base.Where_mathf_exp(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_floor(bool async)
	{
		return base.Where_mathf_floor(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_log(bool async)
	{
		return base.Where_mathf_log(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_log_new_base(bool async)
	{
		return base.Where_mathf_log_new_base(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_log10(bool async)
	{
		return base.Where_mathf_log10(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_power(bool async)
	{
		return base.Where_mathf_power(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_round2(bool async)
	{
		return base.Where_mathf_round2(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_sign(bool async)
	{
		return base.Where_mathf_sign(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_sin(bool async)
	{
		return base.Where_mathf_sin(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_sqrt(bool async)
	{
		return base.Where_mathf_sqrt(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_square(bool async)
	{
		return base.Where_mathf_square(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_tan(bool async)
	{
		return base.Where_mathf_tan(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Where_mathf_truncate(bool async)
	{
		return base.Where_mathf_truncate(async);
	}

	[NotSupportedByProviderTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Datetime_subtraction_TotalDays(bool async)
	{
		return base.Datetime_subtraction_TotalDays(async);
	}
}
