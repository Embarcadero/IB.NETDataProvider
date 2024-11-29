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
using Xunit;
using Xunit.Abstractions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class TPCInheritanceQueryHiLoIBTest : TPCInheritanceQueryIBTestBase<TPCInheritanceQueryHiLoIBFixture>
{
	public TPCInheritanceQueryHiLoIBTest(TPCInheritanceQueryHiLoIBFixture fixture, ITestOutputHelper testOutputHelper)
		: base(fixture, testOutputHelper)
	{ }

	[NotSupportedNULLInUnionByInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Can_filter_all_animals(bool async)
	{
		await base.Can_filter_all_animals(async);
	}

	[NotSupportedNULLInUnionByInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Can_include_animals(bool async)
	{
		await base.Can_include_animals(async);
	}

	[NotSupportedNULLInUnionByInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task Can_include_prey(bool async)
	{
		await base.Can_include_prey(async);
	}

	[NotSupportedNULLInUnionByInterBaseFact]
	public override void Can_insert_update_delete()
	{
		base.Can_insert_update_delete();
	}
}
