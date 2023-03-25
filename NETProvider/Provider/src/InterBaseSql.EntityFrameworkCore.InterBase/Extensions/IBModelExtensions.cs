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
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore;

public static class IBModelExtensions
{
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
}
