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

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests;

public class UpdatesIBTest : UpdatesRelationalTestBase<UpdatesIBFixture>
{
	public UpdatesIBTest(UpdatesIBFixture fixture)
		: base(fixture)
	{ }

	public override void Identifiers_are_generated_correctly()
	{
		using (var context = CreateContext())
		{
			var entityType = context.Model.FindEntityType(typeof(LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectly));
			Assert.Equal(
				"LoginEntityTypeWithAnExtremely~",
				entityType.GetTableName());
			Assert.Equal(
				"PK_LoginEntityTypeWithAnExtrem~",
				entityType.GetKeys().Single().GetName());
			Assert.Equal(
				"FK_LoginEntityTypeWithAnExtrem~",
				entityType.GetForeignKeys().Single().GetConstraintName());
			Assert.Equal(
				"IX_LoginEntityTypeWithAnExtrem~",
				entityType.GetIndexes().Single().GetDatabaseName());
		}
	}
}
