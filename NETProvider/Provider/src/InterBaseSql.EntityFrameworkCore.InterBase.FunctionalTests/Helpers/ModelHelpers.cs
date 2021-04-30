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

using System;
using System.Collections.Generic;
using System.Linq;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers
{
	public static class ModelHelpers
	{
		public static void SetStringLengths(ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
					{
						property.SetMaxLength(500);
					}
				}
			}
		}

		public static void SimpleTableNames(ModelBuilder modelBuilder)
		{
			var names = new HashSet<string>(StringComparer.InvariantCulture);
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				if (entityType.BaseType != null)
					continue;
				var name = new string(entityType.GetTableName().Where(char.IsUpper).ToArray());
				var cnt = 1;
				while (names.Contains(name))
				{
					name += cnt++;
				}
				names.Add(name);
				entityType.SetTableName(name);
			}
		}

		public static void SetPrimaryKeyGeneration(ModelBuilder modelBuilder, IBValueGenerationStrategy valueGenerationStrategy = IBValueGenerationStrategy.SequenceTrigger)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				var pk = entityType.FindPrimaryKey();
				if (pk == null)
					continue;
				var properties = pk.Properties;
				if (properties.Count() != 1)
					continue;
				var property = properties[0];
				if (property.GetValueGenerationStrategy() == IBValueGenerationStrategy.None)
				{
					property.SetValueGenerationStrategy(valueGenerationStrategy);
				}
			}
		}
	}
}
