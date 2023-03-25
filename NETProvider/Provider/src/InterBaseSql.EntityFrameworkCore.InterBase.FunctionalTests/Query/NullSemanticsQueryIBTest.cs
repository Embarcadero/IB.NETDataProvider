/*
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

using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.NullSemanticsModel;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NullSemanticsQueryIBTest : NullSemanticsQueryTestBase<NullSemanticsQueryIBFixture>
{
	public NullSemanticsQueryIBTest(NullSemanticsQueryIBFixture fixture)
		: base(fixture)
	{ }

	protected override NullSemanticsContext CreateContext(bool useRelationalNulls = false)
	{
		var options = new DbContextOptionsBuilder(Fixture.CreateOptions());
		if (useRelationalNulls)
		{
			new IBDbContextOptionsBuilder(options).UseRelationalNulls();
		}
		var context = new NullSemanticsContext(options.Options);
		context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		return context;
	}
}
