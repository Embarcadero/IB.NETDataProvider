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

using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore;

public static class IBPropertyBuilderExtensions
{
	public static PropertyBuilder UseIdentityColumn(this PropertyBuilder propertyBuilder)
	{
		var property = propertyBuilder.Metadata;
		property.SetValueGenerationStrategy(IBValueGenerationStrategy.IdentityColumn);
		return propertyBuilder;
	}

	public static PropertyBuilder<TProperty> UseIdentityColumn<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
		=> (PropertyBuilder<TProperty>)UseIdentityColumn((PropertyBuilder)propertyBuilder);

	public static PropertyBuilder UseSequenceTrigger(this PropertyBuilder propertyBuilder)
	{
		var property = propertyBuilder.Metadata;
		property.SetValueGenerationStrategy(IBValueGenerationStrategy.SequenceTrigger);
		return propertyBuilder;
	}

	public static PropertyBuilder<TProperty> UseSequenceTrigger<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
		=> (PropertyBuilder<TProperty>)UseSequenceTrigger((PropertyBuilder)propertyBuilder);

	public static PropertyBuilder UseHiLo(this PropertyBuilder propertyBuilder, string name = null)
	{
		var property = propertyBuilder.Metadata;
		name ??= IBModelExtensions.DefaultHiLoSequenceName;
		var model = property.DeclaringType.Model;
		if (model.FindSequence(name) == null)
		{
			model.AddSequence(name).IncrementBy = 10;
		}
		property.SetValueGenerationStrategy(IBValueGenerationStrategy.HiLo);
		property.SetHiLoSequenceName(name);
		return propertyBuilder;
	}

	public static PropertyBuilder<TProperty> UseHiLo<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, string name = null)
		=> (PropertyBuilder<TProperty>)UseHiLo((PropertyBuilder)propertyBuilder, name);

	public static IConventionSequenceBuilder HasHiLoSequence(this IConventionPropertyBuilder propertyBuilder, string name, bool fromDataAnnotation = false)
	{
		if (!propertyBuilder.CanSetHiLoSequence(name, fromDataAnnotation))
		{
			return null;
		}
		propertyBuilder.Metadata.SetHiLoSequenceName(name, fromDataAnnotation);
		return name == null
			? null
			: propertyBuilder.Metadata.DeclaringType.Model.Builder.HasSequence(name, null, fromDataAnnotation);
	}

	public static bool CanSetHiLoSequence(this IConventionPropertyBuilder propertyBuilder, string name, bool fromDataAnnotation = false)
	{
		return propertyBuilder.CanSetAnnotation(IBAnnotationNames.HiLoSequenceName, name, fromDataAnnotation);
	}

	public static IConventionPropertyBuilder HasValueGenerationStrategy(this IConventionPropertyBuilder propertyBuilder, IBValueGenerationStrategy? valueGenerationStrategy, bool fromDataAnnotation = false)
	{
		if (propertyBuilder.CanSetAnnotation(IBAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation))
		{
			propertyBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
			if (valueGenerationStrategy != IBValueGenerationStrategy.IdentityColumn)
			{
			}
			if (valueGenerationStrategy != IBValueGenerationStrategy.SequenceTrigger)
			{
			}
			if (valueGenerationStrategy != IBValueGenerationStrategy.HiLo)
			{
			}
			return propertyBuilder;
		}
		return null;
	}
}