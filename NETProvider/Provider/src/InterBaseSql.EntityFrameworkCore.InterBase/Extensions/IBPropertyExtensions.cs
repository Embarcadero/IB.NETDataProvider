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
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore;

public static class IBPropertyExtensions
{
	public static IBValueGenerationStrategy GetValueGenerationStrategy(this IProperty property)
	{
		var annotation = property[IBAnnotationNames.ValueGenerationStrategy];
		if (annotation != null)
		{
			return (IBValueGenerationStrategy)annotation;
		}

		if (property.ValueGenerated != ValueGenerated.OnAdd
			|| property.IsForeignKey()
			|| property.TryGetDefaultValue(out _)
			|| property.GetDefaultValueSql() != null
			|| property.GetComputedColumnSql() != null)
		{
			return IBValueGenerationStrategy.None;
		}

		var modelStrategy = property.DeclaringType.Model.GetValueGenerationStrategy();

		if (modelStrategy == IBValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return IBValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == IBValueGenerationStrategy.IdentityColumn)
		{
			if (property.DeclaringType.GetMappingStrategy() == RelationalAnnotationNames.TpcMappingStrategy)
			{
				return IBValueGenerationStrategy.SequenceTrigger;
			}
			else if (IsCompatibleIdentityColumn(property))
			{
				return IBValueGenerationStrategy.IdentityColumn;
			}
		}
		if (modelStrategy == IBValueGenerationStrategy.HiLo && IsCompatibleHiLoColumn(property))
		{
			return IBValueGenerationStrategy.HiLo;
		}

		return IBValueGenerationStrategy.None;
	}

	public static IBValueGenerationStrategy GetValueGenerationStrategy(this IMutableProperty property)
	{
		var annotation = property[IBAnnotationNames.ValueGenerationStrategy];
		if (annotation != null)
		{
			return (IBValueGenerationStrategy)annotation;
		}

		if (property.ValueGenerated != ValueGenerated.OnAdd
			|| property.IsForeignKey()
			|| property.TryGetDefaultValue(out _)
			|| property.GetDefaultValueSql() != null
			|| property.GetComputedColumnSql() != null)
		{
			return IBValueGenerationStrategy.None;
		}

		var modelStrategy = property.DeclaringType.Model.GetValueGenerationStrategy();

		if (modelStrategy == IBValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return IBValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == IBValueGenerationStrategy.IdentityColumn)
		{
			if (property.DeclaringType.GetMappingStrategy() == RelationalAnnotationNames.TpcMappingStrategy)
			{
				return IBValueGenerationStrategy.SequenceTrigger;
			}
			else if (IsCompatibleIdentityColumn(property))
			{
				return IBValueGenerationStrategy.IdentityColumn;
			}
		}
		if (modelStrategy == IBValueGenerationStrategy.HiLo && IsCompatibleHiLoColumn(property))
		{
			return IBValueGenerationStrategy.HiLo;
		}

		return IBValueGenerationStrategy.None;
	}

	public static IBValueGenerationStrategy GetValueGenerationStrategy(this IConventionProperty property)
	{
		var annotation = property[IBAnnotationNames.ValueGenerationStrategy];
		if (annotation != null)
		{
			return (IBValueGenerationStrategy)annotation;
		}

		if (property.ValueGenerated != ValueGenerated.OnAdd
			|| property.IsForeignKey()
			|| property.TryGetDefaultValue(out _)
			|| property.GetDefaultValueSql() != null
			|| property.GetComputedColumnSql() != null)
		{
			return IBValueGenerationStrategy.None;
		}

		var modelStrategy = property.DeclaringType.Model.GetValueGenerationStrategy();

		if (modelStrategy == IBValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return IBValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == IBValueGenerationStrategy.IdentityColumn)
		{
			if (property.DeclaringType.GetMappingStrategy() == RelationalAnnotationNames.TpcMappingStrategy)
			{
				return IBValueGenerationStrategy.SequenceTrigger;
			}
			else if (IsCompatibleIdentityColumn(property))
			{
				return IBValueGenerationStrategy.IdentityColumn;
			}
		}
		if (modelStrategy == IBValueGenerationStrategy.HiLo && IsCompatibleHiLoColumn(property))
		{
			return IBValueGenerationStrategy.HiLo;
		}

		return IBValueGenerationStrategy.None;
	}

	public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(this IConventionProperty property)
	{
		return property.FindAnnotation(IBAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();
	}

	public static void SetValueGenerationStrategy(this IMutableProperty property, IBValueGenerationStrategy? value)
	{
		CheckValueGenerationStrategy(property, value);
		property.SetOrRemoveAnnotation(IBAnnotationNames.ValueGenerationStrategy, value);
	}

	public static void SetValueGenerationStrategy(this IConventionProperty property, IBValueGenerationStrategy? value, bool fromDataAnnotation = false)
	{
		CheckValueGenerationStrategy(property, value);
		property.SetOrRemoveAnnotation(IBAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);
	}

	public static string GetHiLoSequenceName(this IReadOnlyProperty property)
	{
		return (string)property[IBAnnotationNames.HiLoSequenceName];
	}

	public static string GetHiLoSequenceName(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
	{
		var annotation = property.FindAnnotation(IBAnnotationNames.HiLoSequenceName);
		if (annotation != null)
		{
			return (string)annotation.Value;
		}

		return property.FindSharedStoreObjectRootProperty(storeObject)?.GetHiLoSequenceName(storeObject);
	}

	public static void SetHiLoSequenceName(this IMutableProperty property, string name)
	{
		property.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceName, name);
	}

	public static string SetHiLoSequenceName(this IConventionProperty property, string name, bool fromDataAnnotation = false)
	{
		return (string)property.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceName, name, fromDataAnnotation)?.Value;
	}

	public static ConfigurationSource? GetHiLoSequenceNameConfigurationSource(this IConventionProperty property)
	{
		return property.FindAnnotation(IBAnnotationNames.HiLoSequenceName)?.GetConfigurationSource();
	}

	public static string GetHiLoSequenceSchema(this IReadOnlyProperty property)
	{
		return (string)property[IBAnnotationNames.HiLoSequenceSchema];
	}

	public static string GetHiLoSequenceSchema(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
	{
		var annotation = property.FindAnnotation(IBAnnotationNames.HiLoSequenceSchema);
		if (annotation != null)
		{
			return (string)annotation.Value;
		}

		return property.FindSharedStoreObjectRootProperty(storeObject)?.GetHiLoSequenceSchema(storeObject);
	}

	public static void SetHiLoSequenceSchema(this IMutableProperty property, string schema)
	{
		property.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceSchema, schema);
	}

	public static string SetHiLoSequenceSchema(this IConventionProperty property, string schema, bool fromDataAnnotation = false)
	{
		return (string)property.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceSchema, schema, fromDataAnnotation)?.Value;
	}

	public static ConfigurationSource? GetHiLoSequenceSchemaConfigurationSource(this IConventionProperty property)
	{
		return property.FindAnnotation(IBAnnotationNames.HiLoSequenceSchema)?.GetConfigurationSource();
	}

	public static IReadOnlySequence FindHiLoSequence(this IReadOnlyProperty property)
	{
		var model = property.DeclaringType.Model;

		var sequenceName = property.GetHiLoSequenceName()
			?? model.GetHiLoSequenceName();

		var sequenceSchema = property.GetHiLoSequenceSchema()
			?? model.GetHiLoSequenceSchema();

		return model.FindSequence(sequenceName, sequenceSchema);
	}

	public static IReadOnlySequence FindHiLoSequence(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
	{
		var model = property.DeclaringType.Model;

		var sequenceName = property.GetHiLoSequenceName(storeObject)
			?? model.GetHiLoSequenceName();

		var sequenceSchema = property.GetHiLoSequenceSchema(storeObject)
			?? model.GetHiLoSequenceSchema();

		return model.FindSequence(sequenceName, sequenceSchema);
	}

	public static ISequence FindHiLoSequence(this IProperty property)
	{
		return (ISequence)((IReadOnlyProperty)property).FindHiLoSequence();
	}

	public static ISequence FindHiLoSequence(this IProperty property, in StoreObjectIdentifier storeObject)
	{
		return (ISequence)((IReadOnlyProperty)property).FindHiLoSequence(storeObject);
	}

	public static string GetSequenceName(this IReadOnlyProperty property)
	{
		return (string)property[IBAnnotationNames.SequenceName];
	}

	public static string GetSequenceName(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
	{
		var annotation = property.FindAnnotation(IBAnnotationNames.SequenceName);
		if (annotation != null)
		{
			return (string)annotation.Value;
		}

		return property.FindSharedStoreObjectRootProperty(storeObject)?.GetSequenceName(storeObject);
	}

	public static void SetSequenceName(this IMutableProperty property, string name)
	{
		property.SetOrRemoveAnnotation(IBAnnotationNames.SequenceName, name);
	}

	public static string SetSequenceName(this IConventionProperty property, string name, bool fromDataAnnotation = false)
	{
		return (string)property.SetOrRemoveAnnotation(IBAnnotationNames.SequenceName, name, fromDataAnnotation)?.Value;
	}

	public static ConfigurationSource? GetSequenceNameConfigurationSource(this IConventionProperty property)
	{
		return property.FindAnnotation(IBAnnotationNames.SequenceName)?.GetConfigurationSource();
	}

	public static string GetSequenceSchema(this IReadOnlyProperty property)
	{
		return (string)property[IBAnnotationNames.SequenceSchema];
	}

	public static string GetSequenceSchema(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
	{
		var annotation = property.FindAnnotation(IBAnnotationNames.SequenceSchema);
		if (annotation != null)
		{
			return (string)annotation.Value;
		}

		return property.FindSharedStoreObjectRootProperty(storeObject)?.GetSequenceSchema(storeObject);
	}

	public static void SetSequenceSchema(this IMutableProperty property, string schema)
	{
		property.SetOrRemoveAnnotation(IBAnnotationNames.SequenceSchema, schema);
	}

	public static string SetSequenceSchema(this IConventionProperty property, string schema, bool fromDataAnnotation = false)
	{
		return (string)property.SetOrRemoveAnnotation(IBAnnotationNames.SequenceSchema, schema, fromDataAnnotation)?.Value;
	}

	public static ConfigurationSource? GetSequenceSchemaConfigurationSource(this IConventionProperty property)
	{
		return property.FindAnnotation(IBAnnotationNames.SequenceSchema)?.GetConfigurationSource();
	}

	public static IReadOnlySequence FindSequence(this IReadOnlyProperty property)
	{
		var model = property.DeclaringType.Model;

		var sequenceName = property.GetSequenceName()
			?? model.GetSequenceNameSuffix();

		var sequenceSchema = property.GetSequenceSchema()
			?? model.GetSequenceSchema();

		return model.FindSequence(sequenceName, sequenceSchema);
	}

	public static IReadOnlySequence FindSequence(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
	{
		var model = property.DeclaringType.Model;

		var sequenceName = property.GetSequenceName(storeObject)
			?? model.GetSequenceNameSuffix();

		var sequenceSchema = property.GetSequenceSchema(storeObject)
			?? model.GetSequenceSchema();

		return model.FindSequence(sequenceName, sequenceSchema);
	}

	public static ISequence FindSequence(this IProperty property)
	{
		return (ISequence)((IReadOnlyProperty)property).FindSequence();
	}

	public static ISequence FindSequence(this IProperty property, in StoreObjectIdentifier storeObject)
	{
		return (ISequence)((IReadOnlyProperty)property).FindSequence(storeObject);
	}

	static void CheckValueGenerationStrategy(IReadOnlyPropertyBase property, IBValueGenerationStrategy? value)
	{
		if (value != null)
		{
			if (value == IBValueGenerationStrategy.IdentityColumn && !IsCompatibleIdentityColumn(property))
			{
				throw new ArgumentException($"Incompatible data type for {nameof(IBValueGenerationStrategy.IdentityColumn)} for '{property.Name}'.");
			}
			if (value == IBValueGenerationStrategy.SequenceTrigger && !IsCompatibleSequenceTrigger(property))
			{
				throw new ArgumentException($"Incompatible data type for {nameof(IBValueGenerationStrategy.SequenceTrigger)} for '{property.Name}'.");
			}
			if (value == IBValueGenerationStrategy.HiLo && !IsCompatibleHiLoColumn(property))
			{
				throw new ArgumentException($"Incompatible data type for {nameof(IBValueGenerationStrategy.HiLo)} for '{property.Name}'.");
			}
		}
	}

	static bool IsCompatibleIdentityColumn(IReadOnlyPropertyBase property)
	{
		//		return property.ClrType.IsInteger() || property.ClrType == typeof(decimal);
		//      InterBase does not support Identity columns
		return false;
	}

	static bool IsCompatibleSequenceTrigger(IReadOnlyPropertyBase property)
	{
		return true;
	}

	static bool IsCompatibleHiLoColumn(IReadOnlyPropertyBase property)
	{
		return property.ClrType.IsInteger() || property.ClrType == typeof(decimal);
	}
}