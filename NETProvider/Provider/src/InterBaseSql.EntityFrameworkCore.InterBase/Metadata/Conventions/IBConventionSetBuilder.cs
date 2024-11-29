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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Conventions;

public class IBConventionSetBuilder : RelationalConventionSetBuilder
{
	public IBConventionSetBuilder(ProviderConventionSetBuilderDependencies dependencies, RelationalConventionSetBuilderDependencies relationalDependencies)
		: base(dependencies, relationalDependencies)
	{ }

	public override ConventionSet CreateConventionSet()
	{
		var conventionSet = base.CreateConventionSet();

		conventionSet.Add(new IBValueGenerationStrategyConvention(Dependencies, RelationalDependencies));
		conventionSet.Add(new RelationalMaxIdentifierLengthConvention(67, Dependencies, RelationalDependencies));

		conventionSet.Replace<StoreGenerationConvention>(new IBStoreGenerationConvention(Dependencies, RelationalDependencies));
		conventionSet.Replace<ValueGenerationConvention>(new IBValueGenerationConvention(Dependencies, RelationalDependencies));

		return conventionSet;

	}

	public static ConventionSet Build()
	{
		var serviceProvider = new ServiceCollection()
			.AddEntityFrameworkInterBase()
			.AddDbContext<DbContext>(o => o.UseInterBase("database=localhost:_.ib;user=sysdba;password=masterkey;charset=utf8"))
			.BuildServiceProvider();

		using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
		{
			using (var context = serviceScope.ServiceProvider.GetService<DbContext>())
			{
				return ConventionSet.CreateConventionSet(context);
			}
		}
	}
}