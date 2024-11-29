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
using Xunit;
using Xunit.Abstractions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class IncompleteMappingInheritanceQueryIBTest : TPHInheritanceQueryTestBase<IncompleteMappingInheritanceQueryIBFixture>
{
	public IncompleteMappingInheritanceQueryIBTest(IncompleteMappingInheritanceQueryIBFixture fixture, ITestOutputHelper testOutputHelper)
		: base(fixture, testOutputHelper)
	{ }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task GetType_in_hierarchy_in_abstract_base_type(bool async)
	{
		await base.GetType_in_hierarchy_in_abstract_base_type(async);
	}

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override async Task GetType_in_hierarchy_in_intermediate_type(bool async)
	{
		await base.GetType_in_hierarchy_in_intermediate_type(async);
	}

}
