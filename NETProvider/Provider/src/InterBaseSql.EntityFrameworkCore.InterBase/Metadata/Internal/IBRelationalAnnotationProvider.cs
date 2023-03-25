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
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Collections.Generic;
using System.Linq;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;

public class IBRelationalAnnotationProvider : RelationalAnnotationProvider
{
#pragma warning disable EF1001
	public IBRelationalAnnotationProvider(RelationalAnnotationProviderDependencies dependencies)
#pragma warning restore EF1001
			: base(dependencies)
	{ }

	public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
	{
		if (!designTime)
		{
			yield break;
		}

		var property = column.PropertyMappings.Select(x => x.Property)
			.FirstOrDefault(x => x.GetValueGenerationStrategy() != IBValueGenerationStrategy.None);
		if (property != null)
		{
			var valueGenerationStrategy = property.GetValueGenerationStrategy();
			if (valueGenerationStrategy != IBValueGenerationStrategy.None)
			{
				yield return new Annotation(IBAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy);
			}
		}
	}
}