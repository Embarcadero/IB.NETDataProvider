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

//$Authors = Embarcadero, Jeff Overcash

using System;
using System.Runtime.InteropServices;

namespace InterBaseSql.Data.Client.Native
{
	internal static class MacUnsafeNativeMethods
	{
		[DllImport("libdl.dylib")]
#pragma warning disable IDE1006 // Naming Styles
		internal static extern IntPtr dlopen(string filename, int flags);

		[DllImport("libdl.dylib")]
		internal static extern IntPtr dlsym(IntPtr handle, string symbol);
#pragma warning restore IDE1006 // Naming Styles

		internal const int RTLD_NOW = 2; // for dlopen's flags
	}

	class MacOSClient : LinuxClient
	{
		public override string Platform { get { return "MacOS"; } }

		public override string LibraryName()
		{
			return "libgds.dylib";
		}
		public override string LibCryptName()
		{
			return "libcrypt.dylib";
		}

		protected override IntPtr TryGetProcAddess(string ProcName)
		{
			return MacUnsafeNativeMethods.dlsym(FIBLibrary, ProcName);
		}

		protected override IntPtr GetProcAddress(string ProcName)
		{
			IntPtr Result = MacUnsafeNativeMethods.dlsym(FIBLibrary, ProcName);
			if (Result != IntPtr.Zero)
				return Result;
			else
				throw new Exception(ProcName + " not found in " + LibraryName());
		}

		protected override IntPtr LoadLibrary(string libName)
		{
			return MacUnsafeNativeMethods.dlopen(libName, MacUnsafeNativeMethods.RTLD_NOW);
		}

	}
}
