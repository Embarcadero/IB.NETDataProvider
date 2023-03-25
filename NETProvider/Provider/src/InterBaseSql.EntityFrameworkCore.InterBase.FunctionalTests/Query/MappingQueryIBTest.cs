﻿/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
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
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class MappingQueryIBTest : MappingQueryTestBase<MappingQueryIBTest.MappingQueryFbFixture>
{
	public MappingQueryIBTest(MappingQueryFbFixture fixture)
		: base(fixture)
	{ }

	[DoesNotHaveTheDataFact]
	public override void All_customers()
	{
		base.All_customers();
	}

	[DoesNotHaveTheDataFact]
	public override void All_employees()
	{
		base.All_employees();
	}

	[DoesNotHaveTheDataFact]
	public override void All_orders()
	{
		base.All_orders();
	}

	[DoesNotHaveTheDataFact]
	public override void Project_nullable_enum()
	{
		base.Project_nullable_enum();
	}

	public class MappingQueryFbFixture : MappingQueryFixtureBase
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

		protected override string DatabaseSchema { get; } = null;

		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);

			modelBuilder.Entity<MappedCustomer>(
				e =>
				{
					e.Property(c => c.CompanyName2).Metadata.SetColumnName("CompanyName");
					e.Metadata.SetTableName("Customers");
				});
		}
	}
}
