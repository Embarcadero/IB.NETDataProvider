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

using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NullKeysIBTest : NullKeysTestBase<NullKeysIBTest.NullKeysIBFixture>
{
	public NullKeysIBTest(NullKeysIBFixture fixture)
		: base(fixture)
	{ }

	public class NullKeysIBFixture : NullKeysFixtureBase
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;
		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SetStringLengths(modelBuilder);
			ModelHelpers.SetPrimaryKeyGeneration(modelBuilder, IBValueGenerationStrategy.None);
		}
		protected override void Seed(PoolableDbContext context)
		{
			context.Database.ExecuteSql($"drop index \"IX_WithStringFk_SelfFk\"");
			context.Add(
				new WithStringKey { Id = "Stereo" });
			context.Add(
				new WithStringKey { Id = "Fire" });
			context.Add(
				new WithStringKey { Id = "Empire" });

			context.Add(
				new WithStringFk { Id = "Me", Fk = "Fire" });
			context.Add(
				new WithStringFk { Id = "By" });
			context.Add(
				new WithStringFk { Id = "Rodrigue", Fk = "Stereo" });
			context.Add(
				new WithStringFk
				{
					Id = "Wendy",
					Fk = "Stereo",
					SelfFk = "Rodrigue"
				});
			context.Add(
				new WithStringFk { Id = "And", SelfFk = "By" });
			context.Add(
				new WithStringFk { Id = "George", Fk = "Empire" });

			context.Add(
				new WithIntKey { Id = 1 });
			context.Add(
				new WithIntKey { Id = 2 });
			context.Add(
				new WithIntKey { Id = 3 });

			context.Add(
				new WithNullableIntFk { Id = 1 });
			context.Add(
				new WithNullableIntFk { Id = 2, Fk = 1 });
			context.Add(
				new WithNullableIntFk { Id = 3 });
			context.Add(
				new WithNullableIntFk { Id = 4, Fk = 2 });
			context.Add(
				new WithNullableIntFk { Id = 5 });
			context.Add(
				new WithNullableIntFk { Id = 6 });

			context.Add(
				new WithNullableIntKey { Id = 1 });
			context.Add(
				new WithNullableIntKey { Id = 2 });
			context.Add(
				new WithNullableIntKey { Id = 3 });

			context.Add(
				new WithIntFk { Id = 1, Fk = 1 });
			context.Add(
				new WithIntFk { Id = 2, Fk = 1 });
			context.Add(
				new WithIntFk { Id = 3, Fk = 3 });

			context.Add(
				new WithAllNullableIntKey { Id = 1 });
			context.Add(
				new WithAllNullableIntKey { Id = 2 });
			context.Add(
				new WithAllNullableIntKey { Id = 3 });

			context.Add(
				new WithAllNullableIntFk { Id = 1 });
			context.Add(
				new WithAllNullableIntFk { Id = 2, Fk = 1 });
			context.Add(
				new WithAllNullableIntFk { Id = 3 });
			context.Add(
				new WithAllNullableIntFk { Id = 4, Fk = 2 });
			context.Add(
				new WithAllNullableIntFk { Id = 5 });
			context.Add(
				new WithAllNullableIntFk { Id = 6 });

			context.SaveChanges();
		}
	}
}