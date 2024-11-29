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
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore;

public static class IBModelExtensions
{
	public const string DefaultHiLoSequenceName = "EntityFrameworkHiLoSequence";
	public const string DefaultSequenceNameSuffix = "Sequence";

	public static void SetValueGenerationStrategy(this IMutableModel model, IBValueGenerationStrategy? value)
		=> model.SetOrRemoveAnnotation(IBAnnotationNames.ValueGenerationStrategy, value);

	public static void SetValueGenerationStrategy(this IConventionModel model, IBValueGenerationStrategy? value, bool fromDataAnnotation = false)
		=> model.SetOrRemoveAnnotation(IBAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);

	public static IBValueGenerationStrategy? GetValueGenerationStrategy(this IModel model)
		=> (IBValueGenerationStrategy?)model[IBAnnotationNames.ValueGenerationStrategy];

	public static IBValueGenerationStrategy? GetValueGenerationStrategy(this IMutableModel model)
		=> (IBValueGenerationStrategy?)model[IBAnnotationNames.ValueGenerationStrategy];

	public static IBValueGenerationStrategy? GetValueGenerationStrategy(this IConventionModel model)
		=> (IBValueGenerationStrategy?)model[IBAnnotationNames.ValueGenerationStrategy];

	public static string GetHiLoSequenceName(this IReadOnlyModel model)
		=> (string)model[IBAnnotationNames.HiLoSequenceName] ?? DefaultHiLoSequenceName;

	public static void SetHiLoSequenceName(this IMutableModel model, string name)
		=> model.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceName, name);

	public static string SetHiLoSequenceName(this IConventionModel model, string name, bool fromDataAnnotation = false)
		=> (string)model.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceName, name, fromDataAnnotation)?.Value;

	public static ConfigurationSource? GetHiLoSequenceNameConfigurationSource(this IConventionModel model)
		=> model.FindAnnotation(IBAnnotationNames.HiLoSequenceName)?.GetConfigurationSource();

	public static string GetHiLoSequenceSchema(this IReadOnlyModel model)
		=> (string)model[IBAnnotationNames.HiLoSequenceSchema];

	public static void SetHiLoSequenceSchema(this IMutableModel model, string value)
		=> model.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceSchema, value);

	public static string SetHiLoSequenceSchema(this IConventionModel model, string value, bool fromDataAnnotation = false)
		=> (string)model.SetOrRemoveAnnotation(IBAnnotationNames.HiLoSequenceSchema, value, fromDataAnnotation)?.Value;

	public static ConfigurationSource? GetHiLoSequenceSchemaConfigurationSource(this IConventionModel model)
		=> model.FindAnnotation(IBAnnotationNames.HiLoSequenceSchema)?.GetConfigurationSource();

	public static string GetSequenceNameSuffix(this IReadOnlyModel model)
		=> (string)model[IBAnnotationNames.SequenceNameSuffix] ?? DefaultSequenceNameSuffix;

	public static void SetSequenceNameSuffix(this IMutableModel model, string name)
		=> model.SetOrRemoveAnnotation(IBAnnotationNames.SequenceNameSuffix, name);

	public static string SetSequenceNameSuffix(this IConventionModel model, string name, bool fromDataAnnotation = false)
		=> (string)model.SetOrRemoveAnnotation(IBAnnotationNames.SequenceNameSuffix, name, fromDataAnnotation)?.Value;

	public static ConfigurationSource? GetSequenceNameSuffixConfigurationSource(this IConventionModel model)
		=> model.FindAnnotation(IBAnnotationNames.SequenceNameSuffix)?.GetConfigurationSource();

	public static string GetSequenceSchema(this IReadOnlyModel model)
		=> (string)model[IBAnnotationNames.SequenceSchema];

	public static void SetSequenceSchema(this IMutableModel model, string value)
		=> model.SetOrRemoveAnnotation(IBAnnotationNames.SequenceSchema, value);

	public static string SetSequenceSchema(this IConventionModel model, string value, bool fromDataAnnotation = false)
		=> (string)model.SetOrRemoveAnnotation(IBAnnotationNames.SequenceSchema, value, fromDataAnnotation)?.Value;

	public static ConfigurationSource? GetSequenceSchemaConfigurationSource(this IConventionModel model)
		=> model.FindAnnotation(IBAnnotationNames.SequenceSchema)?.GetConfigurationSource();
}