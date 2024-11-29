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
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class OwnedEntityQueryIBTest : OwnedEntityQueryRelationalTestBase
{
	protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

	[HasDataInTheSameTransactionAsDDLTheory]
	[MemberData(nameof(IsAsyncData))]
	public override Task Multiple_owned_reference_mapped_to_own_table_containing_owned_collection_in_split_query(bool async)
	{
		return base.Multiple_owned_reference_mapped_to_own_table_containing_owned_collection_in_split_query(async);
	}
}
