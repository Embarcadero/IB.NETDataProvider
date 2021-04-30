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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query
{
	public class ChangeTrackingIBTest : ChangeTrackingTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
	{
		public ChangeTrackingIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
			: base(fixture)
		{ }

		protected override NorthwindContext CreateNoTrackingContext()
			=> new NorthwindRelationalContext(
				new DbContextOptionsBuilder(Fixture.CreateOptions())
					.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options);
	}
}
