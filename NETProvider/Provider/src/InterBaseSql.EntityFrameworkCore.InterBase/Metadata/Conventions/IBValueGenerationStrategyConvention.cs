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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Conventions
{
	public class IBValueGenerationStrategyConvention : IModelInitializedConvention, IModelFinalizedConvention
	{
		public IBValueGenerationStrategyConvention(ProviderConventionSetBuilderDependencies dependencies, RelationalConventionSetBuilderDependencies relationalDependencies)
		{
			Dependencies = dependencies;
		}

		protected virtual ProviderConventionSetBuilderDependencies Dependencies { get; }

		public virtual void ProcessModelInitialized(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
		{
			modelBuilder.HasValueGenerationStrategy(IBValueGenerationStrategy.IdentityColumn);
		}

		public virtual void ProcessModelFinalized(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
		{
			foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
			{
				foreach (var property in entityType.GetDeclaredProperties())
				{
					// Needed for the annotation to show up in the model snapshot
					var strategy = property.GetValueGenerationStrategy();
					if (strategy != IBValueGenerationStrategy.None)
					{
						property.Builder.HasValueGenerationStrategy(strategy);
					}
				}
			}
		}
	}
}
