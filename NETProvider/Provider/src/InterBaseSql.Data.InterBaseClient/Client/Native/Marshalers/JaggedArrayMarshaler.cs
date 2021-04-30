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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterBaseSql.Data.Client.Native.Marshalers
{
	class JaggedArrayMarshaler : ICustomMarshaler
	{
		static ICustomMarshaler GetInstance(string cookie)
		{
			return new JaggedArrayMarshaler();
		}
		GCHandle[] handles;
		GCHandle buffer;
		Array[] array;
		public void CleanUpManagedData(object ManagedObj)
		{
		}
		public void CleanUpNativeData(IntPtr pNativeData)
		{
			buffer.Free();
			foreach (GCHandle handle in handles)
			{
				handle.Free();
			}
		}
		public int GetNativeDataSize()
		{
			return 4;
		}
		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			array = (Array[])ManagedObj;
			handles = new GCHandle[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				handles[i] = GCHandle.Alloc(array[i], GCHandleType.Pinned);
			}
			IntPtr[] pointers = new IntPtr[handles.Length];
			for (int i = 0; i < handles.Length; i++)
			{
				pointers[i] = handles[i].AddrOfPinnedObject();
			}
			buffer = GCHandle.Alloc(pointers, GCHandleType.Pinned);
			return buffer.AddrOfPinnedObject();
		}
		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return array;
		}
	}
}
