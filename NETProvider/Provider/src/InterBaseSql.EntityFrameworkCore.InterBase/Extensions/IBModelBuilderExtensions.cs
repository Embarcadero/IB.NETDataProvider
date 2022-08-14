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

using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
	public static class IBModelBuilderExtensions
	{
		//public static ModelBuilder UseIdentityColumns(this ModelBuilder modelBuilder)
		//{
		//	var model = modelBuilder.Model;
		//	model.SetValueGenerationStrategy(IBValueGenerationStrategy.IdentityColumn);
		//	return modelBuilder;
		//}

		public static ModelBuilder UseSequenceTriggers(this ModelBuilder modelBuilder)
		{
			var model = modelBuilder.Model;
			model.SetValueGenerationStrategy(IBValueGenerationStrategy.SequenceTrigger);
			return modelBuilder;
		}

		public static IConventionModelBuilder HasValueGenerationStrategy(this IConventionModelBuilder modelBuilder, IBValueGenerationStrategy? valueGenerationStrategy, bool fromDataAnnotation = false)
		{
			if (modelBuilder.CanSetAnnotation(IBAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation))
			{
				modelBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
				//if (valueGenerationStrategy != IBValueGenerationStrategy.IdentityColumn)
				//{
				//}
				if (valueGenerationStrategy != IBValueGenerationStrategy.SequenceTrigger)
				{
				}
				return modelBuilder;
			}
			return null;
		}
	}
}
