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

//$Authors = Hennadii Zabula

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace InterBaseSql.Data.Client.Native.Handles;

// public visibility added, because auto-generated assembly can't work with internal types
public abstract class InterBaseHandle : SafeHandle, IInterBaseHandle
{
	private IIBClient _ibClient;

	protected InterBaseHandle()
		: base(IntPtr.Zero, true)
	{ }

	// Method added because we can't inject IIbClient in ctor
	public void SetClient(IIBClient ibClient)
	{
		Contract.Requires(_ibClient == null);
		Contract.Requires(ibClient != null);
		Contract.Ensures(_ibClient != null);

		_ibClient = ibClient;
	}

	public IIBClient IBClient => _ibClient;

	public override bool IsInvalid => handle == IntPtr.Zero;
}
