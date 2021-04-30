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

using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.Client.Native.Marshalers
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ArrayDescMarshal_V2
	{
		#region Fields

		public short DescVersion;
		public byte DataType;
		public byte SubType;
		public byte Scale;
		public short Length;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 68)]
		public string FieldName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 68)]
		public string RelationName;
		public short Dimensions;
		public short Flags;

		#endregion

		#region Static Methods

		public static int ComputeLength(int n)
		{
			return Marshal.SizeOf<ArrayDescMarshal_V2>() + n * Marshal.SizeOf<ArrayBoundMarshal>();
		}

		#endregion
	}
}
