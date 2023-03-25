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

		var modelStrategy = property.DeclaringEntityType.Model.GetValueGenerationStrategy();

		if (modelStrategy == IBValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return IBValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == IBValueGenerationStrategy.IdentityColumn && IsCompatibleIdentityColumn(property))
		{
			throw new ArgumentException("InterBase does not support Identity columns");
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

		var modelStrategy = property.DeclaringEntityType.Model.GetValueGenerationStrategy();

		if (modelStrategy == IBValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return IBValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == IBValueGenerationStrategy.IdentityColumn && IsCompatibleIdentityColumn(property))
		{
			throw new ArgumentException("InterBase does not support Identity columns");
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

		var modelStrategy = property.DeclaringEntityType.Model.GetValueGenerationStrategy();

		if (modelStrategy == IBValueGenerationStrategy.SequenceTrigger && IsCompatibleSequenceTrigger(property))
		{
			return IBValueGenerationStrategy.SequenceTrigger;
		}
		if (modelStrategy == IBValueGenerationStrategy.IdentityColumn && IsCompatibleIdentityColumn(property))
		{
			throw new ArgumentException("InterBase does not support Identity columns");
		}

		return IBValueGenerationStrategy.None;
	}

	public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(this IConventionProperty property)
		=> property.FindAnnotation(IBAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

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

	static void CheckValueGenerationStrategy(IReadOnlyPropertyBase property, IBValueGenerationStrategy? value)
	{
		if (value != null)
		{
			if (value == IBValueGenerationStrategy.IdentityColumn && !IsCompatibleIdentityColumn(property))
			{
				throw new ArgumentException("InterBase does not support Identity columns");
			}
			if (value == IBValueGenerationStrategy.SequenceTrigger && !IsCompatibleSequenceTrigger(property))
			{
				throw new ArgumentException($"Incompatible data type for {nameof(IBValueGenerationStrategy.SequenceTrigger)} for '{property.Name}'.");
			}
		}
	}

	static bool IsCompatibleIdentityColumn(IReadOnlyPropertyBase property)
		=> property.ClrType.IsInteger() || property.ClrType == typeof(decimal);

	static bool IsCompatibleSequenceTrigger(IReadOnlyPropertyBase property)
		=> true;
}
