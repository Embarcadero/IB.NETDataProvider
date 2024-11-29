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
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class Ef6GroupByIBTest : Ef6GroupByTestBase<Ef6GroupByIBTest.Ef6GroupByIBFixture>
{
	public Ef6GroupByIBTest(Ef6GroupByIBFixture fixture)
		: base(fixture)
	{ }

	[NotSupportedOnInterBaseTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Group_Join_from_LINQ_101(bool async)
	{
		return base.Group_Join_from_LINQ_101(async);
	}

	[Theory(Skip = "Different math on InterBase.")]
	[MemberData(nameof(IsAsyncData))]
	public override Task Average_Grouped_from_LINQ_101(bool async)
	{
		return base.Average_Grouped_from_LINQ_101(async);
	}

	public class Ef6GroupByIBFixture : Ef6GroupByFixtureBase
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SetStringLengths(modelBuilder);
			var newbuilder = new DbContextOptionsBuilder<ArubaContext>();
		}
	}
}
