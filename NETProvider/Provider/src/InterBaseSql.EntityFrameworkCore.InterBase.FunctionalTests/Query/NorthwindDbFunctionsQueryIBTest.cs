﻿/*
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class NorthwindDbFunctionsQueryIBTest : NorthwindDbFunctionsQueryTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
{
	public NorthwindDbFunctionsQueryIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
		: base(fixture)
	{ }

	public override Task Like_literal(bool async)
	{
		// fix wrong assumptions on collate
		return AssertCount(
			async,
			ss => ss.Set<Microsoft.EntityFrameworkCore.TestModels.Northwind.Customer>(),
			ss => ss.Set<Microsoft.EntityFrameworkCore.TestModels.Northwind.Customer>(),
			c => EF.Functions.Like(c.ContactName, "%M%"),
			c => c.ContactName.Contains("M"));
	}
}
