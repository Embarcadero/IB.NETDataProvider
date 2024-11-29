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

using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceRelationshipsModel;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class TPCRelationshipsQueryIBTest : TPCRelationshipsQueryTestBase<TPCRelationshipsQueryIBTest.TPCRelationshipsQueryIBFixture>
{
	public TPCRelationshipsQueryIBTest(TPCRelationshipsQueryIBFixture fixture)
		: base(fixture)
	{ }

	public class TPCRelationshipsQueryIBFixture : TPCRelationshipsQueryRelationalFixture
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;
		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SetStringLengths(modelBuilder);
			ModelHelpers.SetPrimaryKeyGeneration(modelBuilder);
		}
		protected override void Seed(InheritanceRelationshipsContext context)
		{
			ModelHelpers.DisableUniqueKeys(context);
			base.Seed(context);
		}
	}
}
