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

using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class GearsOfWarQueryIBFixture : GearsOfWarQueryRelationalFixture
{
	protected override ITestStoreFactory TestStoreFactory => IBTestStoreFactory.Instance;

	protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
	{
		base.OnModelCreating(modelBuilder, context);
		ModelHelpers.SetStringLengths(modelBuilder);
		ModelHelpers.SetPrimaryKeyGeneration(modelBuilder);
	}

	protected override void Seed(GearsOfWarContext context)
	{
		ModelHelpers.DisableUniqueKeys(context);
		//context.Database.ExecuteSql($"drop index \"IX_LocustLeaders_DefeatedByNickname_DefeatedBySquadId\"");
		//context.Database.ExecuteSql($"drop index \"IX_Factions_CommanderName\"");
		//context.Database.ExecuteSql($"drop index \"IX_Tags_GearNickName_GearSquadId\"");
		//context.Database.ExecuteSql($"drop index \"IX_Weapons_SynergyWithId\"");
		GearsOfWarContext.Seed(context);
	} 

}
