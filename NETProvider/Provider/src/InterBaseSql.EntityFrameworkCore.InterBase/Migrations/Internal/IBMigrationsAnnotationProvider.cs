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

//$Authors = Jiri Cincura (jiri@cincura.net), Jean Ressouche, Rafael Almeida (ralms@ralms.net)

using System.Collections.Generic;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Migrations.Internal
{
	public class IBMigrationsAnnotationProvider : MigrationsAnnotationProvider
	{
		public IBMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
			: base(dependencies)
		{ }

		public override IEnumerable<IAnnotation> For(IProperty property)
		{
			var valueGenerationStrategy = property.GetValueGenerationStrategy();
			if (valueGenerationStrategy != IBValueGenerationStrategy.None)
			{
				yield return new Annotation(IBAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy);
			}
		}
	}
}
