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
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class FunkyDataQueryIBTest : FunkyDataQueryTestBase<FunkyDataQueryIBTest.FunkyDataQueryIBFixture>
{
	public FunkyDataQueryIBTest(FunkyDataQueryIBFixture fixture)
		: base(fixture)
	{ }

	[Theory]
	[MemberData(nameof(IsAsyncData))]
	public override Task String_contains_on_argument_with_wildcard_column(bool async)
	{
		return base.String_contains_on_argument_with_wildcard_column(async);
	}

	public class FunkyDataQueryIBFixture : FunkyDataQueryFixtureBase
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SetStringLengths(modelBuilder);
		}
	}
}