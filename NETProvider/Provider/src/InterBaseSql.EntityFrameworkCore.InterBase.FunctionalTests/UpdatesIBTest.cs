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

using System;
using System.Linq;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests;

public class UpdatesIBTest : UpdatesRelationalTestBase<UpdatesIBTest.UpdatesIBFixture>
{
	public UpdatesIBTest(UpdatesIBFixture fixture)
		: base(fixture)
	{ }

	[Fact]
	public override void Identifiers_are_generated_correctly()
	{
		using (var context = CreateContext())
		{
			var entityType = context.Model.FindEntityType(typeof(LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectly));
			Assert.Equal(
				"LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUse~",
				entityType.GetTableName());
			Assert.Equal(
				"PK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIs~",
				entityType.GetKeys().Single().GetName());
			Assert.Equal(
				"FK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIs~",
				entityType.GetForeignKeys().Single().GetConstraintName());
			Assert.Equal(
				"IX_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIs~",
				entityType.GetIndexes().Single().GetDatabaseName());
		}
	}
	[Fact(Skip = "Uses type of filtered index that is not supported on InterBase.")]
	public override void Swap_filtered_unique_index_values() => base.Swap_filtered_unique_index_values();

	public class UpdatesIBFixture : UpdatesRelationalFixture
	{
		protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

		protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
		{
			base.OnModelCreating(modelBuilder, context);
			ModelHelpers.SetStringLengths(modelBuilder);
			ModelHelpers.SetPrimaryKeyGeneration(modelBuilder, IBValueGenerationStrategy.SequenceTrigger, x => x.ClrType == typeof(Person));
			modelBuilder.Entity<ProductBase>();
		}
		protected override void Seed(UpdatesContext context)
		{
			ModelHelpers.DisableUniqueKeys(context);
			ModelHelpers.DropForeignKeys(context);
			base.Seed(context);
		}

	}
}