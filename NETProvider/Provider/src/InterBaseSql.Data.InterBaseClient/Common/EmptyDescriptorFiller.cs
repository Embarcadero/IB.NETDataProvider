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

using System.Threading;
using System.Threading.Tasks;

namespace InterBaseSql.Data.Common;

internal sealed class EmptyDescriptorFiller : IDescriptorFiller
{
	public static readonly EmptyDescriptorFiller Instance = new EmptyDescriptorFiller();

	private EmptyDescriptorFiller()
	{ }

	public void Fill(Descriptor descriptor, int index)
	{ }

	public ValueTask FillAsync(Descriptor descriptor, int index, CancellationToken cancellationToken = default)
	{
		return ValueTask2.CompletedTask;
	}
}
