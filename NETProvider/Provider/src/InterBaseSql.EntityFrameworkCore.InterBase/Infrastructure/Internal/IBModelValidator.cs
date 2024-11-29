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

using System.Linq;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal;

public class IBModelValidator : RelationalModelValidator
{
	public IBModelValidator(ModelValidatorDependencies dependencies, RelationalModelValidatorDependencies relationalDependencies)
		: base(dependencies, relationalDependencies)
	{ }

	protected override void ValidateValueGeneration(IEntityType entityType, IKey key, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
	{
		if (entityType.GetMappingStrategy() == RelationalAnnotationNames.TpcMappingStrategy && entityType.BaseType == null)
		{
			foreach (var storeGeneratedProperty in key.Properties.Where(p => (p.ValueGenerated & ValueGenerated.OnAdd) != 0 && p.GetValueGenerationStrategy() == IBValueGenerationStrategy.IdentityColumn))
			{
				logger.TpcStoreGeneratedIdentityWarning(storeGeneratedProperty);
			}
		}
	}
}
