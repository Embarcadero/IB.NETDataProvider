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
using System.Collections.Generic;
using System.Linq;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using System.Data;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;

public static class ModelHelpers
{
	public static void SetStringLengths(ModelBuilder modelBuilder)
	{
		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			HandleProperties(entityType.GetProperties());
			HandleComplexProperties(entityType.GetComplexProperties());
		}

		void HandleProperties(IEnumerable<IMutableProperty> properties)
		{
			foreach (var property in properties)
			{
				SetStringLength(property);
			}
		}
		void HandleComplexProperties(IEnumerable<IMutableComplexProperty> complexProperties)
		{
			foreach (var cp in complexProperties)
			{
				HandleProperties(cp.ComplexType.GetProperties());
				HandleComplexProperties(cp.ComplexType.GetComplexProperties());
			}
		}
		void SetStringLength(IMutableProperty property)
		{
			if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
			{
				property.SetMaxLength(100);
			}
		}

	}

	// InterBase does not allow multiple nulls in a unique key.  Just disable them
	public static void DisableUniqueKeys(DbContext context)
	{
		var indices =
				@"select i.rdb$index_name
					from rdb$indices i left outer join rdb$relation_constraints rc on

						i.rdb$index_name = rc.rdb$index_name
					where i.rdb$unique_flag = 1 and

						rc.rdb$index_name is null and
						coalesce(i.rdb$system_flag, 0) <> 1";
		var con = context.Database.GetDbConnection();
		con.Open();
		using (var cmd = con.CreateCommand())
		{
			cmd.CommandText = indices;
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				var idxcmd = con.CreateCommand();
				idxcmd.CommandText = "alter index " + reader.GetString(0).Trim().AddDoubleQuote() + " inactive";
				idxcmd.ExecuteNonQuery();
			}
		}
		con.Close();
	}

	public static void DropForeignKeys(DbContext context)
	{
		var keys =
				@"select distinct 'ALTER TABLE ""' || EF_TRIM('BOTH', rdb$relation_name) || '"" DROP CONSTRAINT ""' || EF_TRIM('BOTH', rdb$constraint_name) || '""' 
  from rdb$relation_constraints
 where rdb$constraint_type = 'FOREIGN KEY'";
		var con = context.Database.GetDbConnection();
		con.Open();
		using (var cmd = con.CreateCommand())
		{
			cmd.CommandText = keys;
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				try
				{
					context.Database.ExecuteSqlRaw(reader.GetString(0));
				}
				catch { }
			}
		}
		con.Close();
	}
	public static void SimpleTableNames(ModelBuilder modelBuilder)
	{
		return;

		//var names = new HashSet<string>(StringComparer.InvariantCulture);
		//foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		//{
		//	if (entityType.BaseType != null)
		//		continue;
		//	entityType.SetTableName(Simplify(entityType.GetTableName()));
		//	foreach (var property in entityType.GetProperties())
		//	{
		//		property.SetColumnName(Simplify(property.Name));
		//	}
		//}

		//string Simplify(string name)
		//{
		//	name = new string(name.Where(char.IsUpper).ToArray());
		//	var cnt = 1;
		//	while (names.Contains(name))
		//	{
		//		name += cnt++;
		//	}
		//	names.Add(name);
		//	return name;
		//}
	}
	public static void SetPrimaryKeyGeneration(ModelBuilder modelBuilder, IBValueGenerationStrategy valueGenerationStrategy = IBValueGenerationStrategy.SequenceTrigger, Func<IMutableEntityType, bool> filter = null)
	{
		filter ??= _ => true;
		foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(filter))
		{
			var pk = entityType.FindPrimaryKey();
			if (pk == null)
				continue;
			var properties = pk.Properties;
			foreach (var prop in properties)
			{
				if (!entityType.IsOwned())
				{
					try
					{
						if (prop.ClrType == typeof(string))
						{
							modelBuilder.Entity(entityType.ClrType)
										.Property(prop.Name)
										.HasValueGenerator<PsuedoGeneratorStringValues>();
						}
						else
						if (prop.ClrType == typeof(Guid))
						{
							modelBuilder.Entity(entityType.ClrType)
										.Property(prop.Name)
										.HasValueGenerator<PsuedoGeneratorGuidValues>();
						}
						else
						modelBuilder.Entity(entityType.ClrType)
										.Property(prop.Name)
										.HasValueGenerator<PsuedoGeneratorIntValues>();
					}
					catch { };
				}
			}
			if (properties.Count() != 1)
				continue;
			var property = properties[0];
			if (property.GetValueGenerationStrategy() == IBValueGenerationStrategy.None)
			{
				property.SetValueGenerationStrategy(valueGenerationStrategy);
			}
		}
	}

	public static void ShortenMM(ModelBuilder modelBuilder)
	{
		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			entityType.SetTableName(Shorten(entityType.ShortName()));
			foreach (var property in entityType.GetProperties())
			{
				property.SetColumnName(Shorten(property.Name));
			}
		}
		static string Shorten(string s)
		{
			return s
				.Replace("UnidirectionalEntity", "UE")
				.Replace("Unidirectional", "U")
				.Replace("JoinOneToThree", "J1_3")
				.Replace("EntityTableSharing", "ETS")
				.Replace("GeneratedKeys", "GK")
				.Replace("ImplicitManyToMany", "IMM");
		}
	}
}