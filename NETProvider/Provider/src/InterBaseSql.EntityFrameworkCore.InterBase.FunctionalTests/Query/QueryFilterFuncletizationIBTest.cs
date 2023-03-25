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

using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;
using Xunit.Abstractions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class QueryFilterFuncletizationIBTest : QueryFilterFuncletizationTestBase<QueryFilterFuncletizationIBTest.QueryFilterFuncletizationIBFixture>
{
	public QueryFilterFuncletizationIBTest(QueryFilterFuncletizationIBFixture fixture)
		: base(fixture)
	{ }

	[Fact]
	public override void DbContext_list_is_parameterized()
	{
		using var context = CreateContext();
		// Default value of TenantIds is null InExpression over null values throws
		Assert.Throws<NullReferenceException>(() => context.Set<ListFilter>().ToList());

		context.TenantIds = new List<int>();
		var query = context.Set<ListFilter>().ToList();
		Assert.Empty(query);

		context.TenantIds = new List<int> { 1 };
		query = context.Set<ListFilter>().ToList();
		Assert.Single(query);

		context.TenantIds = new List<int> { 2, 3 };
		query = context.Set<ListFilter>().ToList();
		Assert.Equal(2, query.Count);
	}

	public class QueryFilterFuncletizationIBFixture : QueryFilterFuncletizationRelationalFixture
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SimpleTableNames(modelBuilder);
			ModelHelpers.SetPrimaryKeyGeneration(modelBuilder, IBValueGenerationStrategy.IdentityColumn);
		}
	}
}
