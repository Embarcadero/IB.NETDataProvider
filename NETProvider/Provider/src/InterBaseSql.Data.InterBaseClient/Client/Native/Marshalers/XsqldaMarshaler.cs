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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net), Hennadii Zabula

using System;
using System.Runtime.InteropServices;
using System.IO;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.Client.Native.Marshalers
{
	internal static class XsqldaMarshaler
	{

		private static int sizeofXSQLDA = Marshal.SizeOf<XSQLDA>();
		private static int sizeofXSQLVAR_V1 = Marshal.SizeOf<XSQLVAR_V1>();
		private static int sizeofXSQLVAR = Marshal.SizeOf<XSQLVAR>();
		private static int sqlvarPad = Environment.Is64BitProcess ? 6 : 2;

		public static void CleanUpNativeData(ref IntPtr pNativeData)
		{
			if (pNativeData != IntPtr.Zero)
			{
				var xsqlda = Marshal.PtrToStructure<XSQLDA>(pNativeData);
				Marshal.DestroyStructure<XSQLDA>(pNativeData);
				for (var i = 0; i < xsqlda.sqln; i++)
				{
					var ptr = GetIntPtr(pNativeData, ComputeLength(i, xsqlda.version));
					if (xsqlda.version == IscCodes.SQLDA_VERSION2)
					{
						var sqlvar = new XSQLVAR();
						MarshalXSQLVARNativeToManaged(ptr, sqlvar, true);
						if (sqlvar.sqldata != IntPtr.Zero)
						{
							Marshal.FreeHGlobal(sqlvar.sqldata);
							sqlvar.sqldata = IntPtr.Zero;
						}
						if (sqlvar.sqlind != IntPtr.Zero)
						{
							Marshal.FreeHGlobal(sqlvar.sqlind);
							sqlvar.sqlind = IntPtr.Zero;
						}
						Marshal.DestroyStructure<XSQLVAR>(ptr);
					}
					else
					{
						var sqlvar_v1 = new XSQLVAR_V1();
						MarshalXSQLVARNativeToManaged(ptr, sqlvar_v1, true);
						if (sqlvar_v1.sqldata != IntPtr.Zero)
						{
							Marshal.FreeHGlobal(sqlvar_v1.sqldata);
							sqlvar_v1.sqldata = IntPtr.Zero;
						}
						if (sqlvar_v1.sqlind != IntPtr.Zero)
						{
							Marshal.FreeHGlobal(sqlvar_v1.sqlind);
							sqlvar_v1.sqlind = IntPtr.Zero;
						}
						Marshal.DestroyStructure<XSQLVAR_V1>(ptr);
					}
				}
				Marshal.FreeHGlobal(pNativeData);
				pNativeData = IntPtr.Zero;
			}
		}

		public static IntPtr MarshalManagedToNative(Charset charset, Descriptor descriptor)
		{
			var xsqlda = new XSQLDA
			{
				version = descriptor.Version,
				sqln = descriptor.Count,
				sqld = descriptor.ActualCount
			};
			if (xsqlda.version == IscCodes.SQLDA_VERSION2)
			{
				var xsqlvar = new XSQLVAR[descriptor.Count];
				for (var i = 0; i < xsqlvar.Length; i++)
				{
					xsqlvar[i] = new XSQLVAR
					{
						sqltype = descriptor[i].DataType,
						sqlscale = descriptor[i].NumericScale,
						sqlprecision = descriptor[i].PrecisionScale,
						sqlsubtype = descriptor[i].SubType,
						sqllen = descriptor[i].Length
					};
					if (descriptor[i].HasDataType() && descriptor[i].DbDataType != DbDataType.Null)
					{
						var buffer = descriptor[i].DbValue.GetBytes();
						xsqlvar[i].sqldata = Marshal.AllocHGlobal(buffer.Length);
						Marshal.Copy(buffer, 0, xsqlvar[i].sqldata, buffer.Length);
					}
					else
					{
						xsqlvar[i].sqldata = Marshal.AllocHGlobal(0);
					}

					xsqlvar[i].sqlind = Marshal.AllocHGlobal(Marshal.SizeOf<short>());
					Marshal.WriteInt16(xsqlvar[i].sqlind, descriptor[i].NullFlag);

					xsqlvar[i].sqlname = GetStringBuffer(charset, descriptor[i].Name);
					xsqlvar[i].sqlname_length = (short)descriptor[i].Name.Length;

					xsqlvar[i].relname = GetStringBuffer(charset, descriptor[i].Relation);
					xsqlvar[i].relname_length = (short)descriptor[i].Relation.Length;

					xsqlvar[i].ownername = GetStringBuffer(charset, descriptor[i].Owner);
					xsqlvar[i].ownername_length = (short)descriptor[i].Owner.Length;

					xsqlvar[i].aliasname = GetStringBuffer(charset, descriptor[i].Alias);
					xsqlvar[i].aliasname_length = (short)descriptor[i].Alias.Length;
				}
				return MarshalManagedToNative(xsqlda, xsqlvar);
			}
			else
			{
				var xsqlvar_v1 = new XSQLVAR_V1[descriptor.Count];
				for (var i = 0; i < xsqlvar_v1.Length; i++)
				{
					xsqlvar_v1[i] = new XSQLVAR_V1
					{
						sqltype = descriptor[i].DataType,
						sqlscale = descriptor[i].NumericScale,
						sqlsubtype = descriptor[i].SubType,
						sqllen = descriptor[i].Length
					};
					if (descriptor[i].HasDataType())
					{
						var buffer = descriptor[i].DbValue.GetBytes();
						xsqlvar_v1[i].sqldata = Marshal.AllocHGlobal(buffer.Length);
						Marshal.Copy(buffer, 0, xsqlvar_v1[i].sqldata, buffer.Length);
					}
					else
					{
						xsqlvar_v1[i].sqldata = Marshal.AllocHGlobal(0);
					}

					xsqlvar_v1[i].sqlind = Marshal.AllocHGlobal(Marshal.SizeOf<short>());
					Marshal.WriteInt16(xsqlvar_v1[i].sqlind, descriptor[i].NullFlag);

					xsqlvar_v1[i].sqlname = GetStringBuffer_V1(charset, descriptor[i].Name);
					xsqlvar_v1[i].sqlname_length = (short)descriptor[i].Name.Length;

					xsqlvar_v1[i].relname = GetStringBuffer_V1(charset, descriptor[i].Relation);
					xsqlvar_v1[i].relname_length = (short)descriptor[i].Relation.Length;

					xsqlvar_v1[i].ownername = GetStringBuffer_V1(charset, descriptor[i].Owner);
					xsqlvar_v1[i].ownername_length = (short)descriptor[i].Owner.Length;

					xsqlvar_v1[i].aliasname = GetStringBuffer_V1(charset, descriptor[i].Alias);
					xsqlvar_v1[i].aliasname_length = (short)descriptor[i].Alias.Length;
				}
				return MarshalManagedToNative_V1(xsqlda, xsqlvar_v1);
			}
		}

		public static IntPtr MarshalManagedToNative(XSQLDA xsqlda, XSQLVAR[] xsqlvar)
		{
			var size = ComputeLength(xsqlda.sqln, xsqlda.version);
			var ptr = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(xsqlda, ptr, true);
			for (var i = 0; i < xsqlvar.Length; i++)
			{
				var offset = ComputeLength(i, xsqlda.version);
				Marshal.StructureToPtr(xsqlvar[i], GetIntPtr(ptr, offset), true);
			}
			return ptr;
		}

		public static IntPtr MarshalManagedToNative_V1(XSQLDA xsqlda, XSQLVAR_V1[] xsqlvar)
		{
			var size = ComputeLength(xsqlda.sqln, xsqlda.version);
			var ptr = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(xsqlda, ptr, true);
			for (var i = 0; i < xsqlvar.Length; i++)
			{
				var offset = ComputeLength(i, xsqlda.version);
				Marshal.StructureToPtr(xsqlvar[i], GetIntPtr(ptr, offset), true);
			}
			return ptr;
		}

		public static Descriptor MarshalNativeToManaged(Charset charset, IntPtr pNativeData)
		{
			return MarshalNativeToManaged(charset, pNativeData, false);
		}

		private static void MarshalNativeSQLVarV1ToManaged(Descriptor descriptor, Charset charset, IntPtr pNativeData, bool fetching)
		{
			var xsqlvar = new XSQLVAR_V1(); 

			for (var i = 0; i < descriptor.ActualCount; i++)
			{
				var ptr = GetIntPtr(pNativeData, ComputeLength(i, IscCodes.SQLDA_VERSION1));
				MarshalXSQLVARNativeToManaged(ptr, xsqlvar);

				descriptor[i].DataType = xsqlvar.sqltype;
				descriptor[i].NumericScale = xsqlvar.sqlscale;
				descriptor[i].SubType = xsqlvar.sqlsubtype;
				descriptor[i].Length = xsqlvar.sqllen;

				descriptor[i].NullFlag = xsqlvar.sqlind == IntPtr.Zero
					? (short)0
					: Marshal.ReadInt16(xsqlvar.sqlind);

				if (fetching)
				{
					if (!descriptor[i].IsNull())
					{
						descriptor[i].SetValue(GetBytes(xsqlvar));
					}
				}
				descriptor[i].Name = GetString(charset, xsqlvar.sqlname, xsqlvar.sqlname_length);
				descriptor[i].Relation = GetString(charset, xsqlvar.relname, xsqlvar.relname_length);
				descriptor[i].Owner = GetString(charset, xsqlvar.ownername, xsqlvar.ownername_length);
				descriptor[i].Alias = GetString(charset, xsqlvar.aliasname, xsqlvar.aliasname_length);
			}
		}

		private static void MarshalNativeSQLVarToManaged(Descriptor descriptor, Charset charset, IntPtr pNativeData, bool fetching)
		{
			var xsqlvar = new XSQLVAR();

			for (var i = 0; i < descriptor.Count; i++)
			{
				var ptr = GetIntPtr(pNativeData, ComputeLength(i, IscCodes.SQLDA_VERSION2));
				MarshalXSQLVARNativeToManaged(ptr, xsqlvar);

				descriptor[i].DataType = xsqlvar.sqltype;
				descriptor[i].NumericScale = xsqlvar.sqlscale;
				descriptor[i].PrecisionScale = xsqlvar.sqlprecision;
				descriptor[i].SubType = xsqlvar.sqlsubtype;
				descriptor[i].Length = xsqlvar.sqllen;

				descriptor[i].NullFlag = xsqlvar.sqlind == IntPtr.Zero
					? (short)0
					: Marshal.ReadInt16(xsqlvar.sqlind);

				if (fetching)
				{
					if (!descriptor[i].IsNull())
					{
						descriptor[i].SetValue(GetBytes(xsqlvar));
					}
				}
				descriptor[i].Name = GetString(charset, xsqlvar.sqlname, xsqlvar.sqlname_length);
				descriptor[i].Relation = GetString(charset, xsqlvar.relname, xsqlvar.relname_length);
				descriptor[i].Owner = GetString(charset, xsqlvar.ownername, xsqlvar.ownername_length);
				descriptor[i].Alias = GetString(charset, xsqlvar.aliasname, xsqlvar.aliasname_length);
			}
		}

		public static Descriptor MarshalNativeToManaged(Charset charset, IntPtr pNativeData, bool fetching)
		{
			var xsqlda = Marshal.PtrToStructure<XSQLDA>(pNativeData);

			var descriptor = new Descriptor(xsqlda.sqln) { ActualCount = xsqlda.sqld, Version = xsqlda.version};

			if (xsqlda.version == IscCodes.SQLDA_VERSION2)
			{
				var xsqlvar = new XSQLVAR();
				for (var i = 0; i < xsqlda.sqln; i++)
				{
					var ptr = GetIntPtr(pNativeData, ComputeLength(i, xsqlda.version));
					MarshalXSQLVARNativeToManaged(ptr, xsqlvar);

					descriptor[i].DataType = xsqlvar.sqltype;
					descriptor[i].NumericScale = xsqlvar.sqlscale;
					descriptor[i].PrecisionScale = xsqlvar.sqlprecision;
					descriptor[i].SubType = xsqlvar.sqlsubtype;
					descriptor[i].Length = xsqlvar.sqllen;

					descriptor[i].NullFlag = xsqlvar.sqlind == IntPtr.Zero
						? (short)0
						: Marshal.ReadInt16(xsqlvar.sqlind);

					if (fetching)
					{
						if (descriptor[i].NullFlag >= 0)
						{
							descriptor[i].SetValue(GetBytes(xsqlvar));
						}
					}
					descriptor[i].Name = GetString(charset, xsqlvar.sqlname, xsqlvar.sqlname_length);
					descriptor[i].Relation = GetString(charset, xsqlvar.relname, xsqlvar.relname_length);
					descriptor[i].Owner = GetString(charset, xsqlvar.ownername, xsqlvar.ownername_length);
					descriptor[i].Alias = GetString(charset, xsqlvar.aliasname, xsqlvar.aliasname_length);
				}
			}
			else
			{
				var xsqlvar = new XSQLVAR_V1();
				for (var i = 0; i < xsqlda.sqln; i++)
				{
					var ptr = GetIntPtr(pNativeData, ComputeLength(i, xsqlda.version));
					MarshalXSQLVARNativeToManaged(ptr, xsqlvar);

					descriptor[i].DataType = xsqlvar.sqltype;
					descriptor[i].NumericScale = xsqlvar.sqlscale;
					descriptor[i].SubType = xsqlvar.sqlsubtype;
					descriptor[i].Length = xsqlvar.sqllen;

					descriptor[i].NullFlag = xsqlvar.sqlind == IntPtr.Zero
						? (short)0
						: Marshal.ReadInt16(xsqlvar.sqlind);

					if (fetching)
					{
						if (descriptor[i].NullFlag >= 0)
						{
							descriptor[i].SetValue(GetBytes(xsqlvar));
						}
					}
					descriptor[i].Name = GetString(charset, xsqlvar.sqlname, xsqlvar.sqlname_length);
					descriptor[i].Relation = GetString(charset, xsqlvar.relname, xsqlvar.relname_length);
					descriptor[i].Owner = GetString(charset, xsqlvar.ownername, xsqlvar.ownername_length);
					descriptor[i].Alias = GetString(charset, xsqlvar.aliasname, xsqlvar.aliasname_length);
				}
			}
			return descriptor;
		}

		private static void MarshalXSQLVARNativeToManaged(IntPtr ptr, XSQLVAR_V1 xsqlvar, bool onlyPointers = false)
		{
			unsafe
			{
				using (var reader = new BinaryReader(new UnmanagedMemoryStream((byte*)ptr.ToPointer(), sizeofXSQLVAR_V1)))
				{
					if (!onlyPointers) xsqlvar.sqltype = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqlscale = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqlsubtype = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqllen = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					xsqlvar.sqldata = reader.ReadIntPtr();
					xsqlvar.sqlind = reader.ReadIntPtr();
					if (!onlyPointers) xsqlvar.sqlname_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqlname = reader.ReadBytes(32); else reader.BaseStream.Position += 32;
					if (!onlyPointers) xsqlvar.relname_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.relname = reader.ReadBytes(32); else reader.BaseStream.Position += 32;
					if (!onlyPointers) xsqlvar.ownername_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.ownername = reader.ReadBytes(32); else reader.BaseStream.Position += 32;
					if (!onlyPointers) xsqlvar.aliasname_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.aliasname = reader.ReadBytes(32); else reader.BaseStream.Position += 32;
				}
			}
		}

		private static void MarshalXSQLVARNativeToManaged(IntPtr ptr, XSQLVAR xsqlvar, bool onlyPointers = false)
		{
			unsafe
			{
				using (var reader = new BinaryReader(new UnmanagedMemoryStream((byte*)ptr.ToPointer(), sizeofXSQLVAR)))
				{
					if (!onlyPointers) xsqlvar.sqltype = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqlscale = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqlprecision = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqlsubtype = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqllen = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					// because of alignment the intptr's start at offset 12 or 16 
					reader.BaseStream.Position += sqlvarPad; 
					xsqlvar.sqldata = reader.ReadIntPtr();
					xsqlvar.sqlind = reader.ReadIntPtr();
					if (!onlyPointers) xsqlvar.sqlname_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.sqlname = reader.ReadBytes(68); else reader.BaseStream.Position += 68;
					if (!onlyPointers) xsqlvar.relname_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.relname = reader.ReadBytes(68); else reader.BaseStream.Position += 68;
					if (!onlyPointers) xsqlvar.ownername_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.ownername = reader.ReadBytes(68); else reader.BaseStream.Position += 68;
					if (!onlyPointers) xsqlvar.aliasname_length = reader.ReadInt16(); else reader.BaseStream.Position += sizeof(short);
					if (!onlyPointers) xsqlvar.aliasname = reader.ReadBytes(68); else reader.BaseStream.Position += 68;
				}
			}
		}

		private static IntPtr GetIntPtr(IntPtr ptr, int offset)
		{
			return new IntPtr(ptr.ToInt64() + offset);
		}

		private static int ComputeLength(int n, int version)
		{
		    int aSize;
            if (version == IscCodes.SQLDA_VERSION2)
				{ aSize = sizeofXSQLVAR; }
		    else
				{ aSize = sizeofXSQLVAR_V1; }

		    var length = (sizeofXSQLDA + (n * aSize));
			if (IntPtr.Size == 8)
			{
				length += 4;
			}
			return length;
		}

		private static byte[] GetBytes(XSQLVAR_V1 xsqlvar)
		{
			if (xsqlvar.sqllen == 0 || xsqlvar.sqldata == IntPtr.Zero)
			{
				return null;
			}

			var type = xsqlvar.sqltype & ~1;
			switch (type)
			{
				case IscCodes.SQL_VARYING:
					{
						var buffer = new byte[Marshal.ReadInt16(xsqlvar.sqldata)];
						var tmp = GetIntPtr(xsqlvar.sqldata, 2);
						Marshal.Copy(tmp, buffer, 0, buffer.Length);
						return buffer;
					}
				case IscCodes.SQL_TEXT:
				case IscCodes.SQL_SHORT:
				case IscCodes.SQL_LONG:
				case IscCodes.SQL_FLOAT:
				case IscCodes.SQL_DOUBLE:
				case IscCodes.SQL_D_FLOAT:
				case IscCodes.SQL_QUAD:
				case IscCodes.SQL_INT64:
				case IscCodes.SQL_BLOB:
				case IscCodes.SQL_ARRAY:
				case IscCodes.SQL_TIMESTAMP:
				case IscCodes.SQL_TYPE_TIME:
				case IscCodes.SQL_TYPE_DATE:
				case IscCodes.SQL_BOOLEAN:
					{
						var buffer = new byte[xsqlvar.sqllen];
						Marshal.Copy(xsqlvar.sqldata, buffer, 0, buffer.Length);
						return buffer;
					}
				default:
					throw TypeHelper.InvalidDataType(type);
			}
		}

		private static byte[] GetBytes(XSQLVAR xsqlvar)
		{
			if (xsqlvar.sqllen == 0 || xsqlvar.sqldata == IntPtr.Zero)
			{
				return null;
			}

			var type = xsqlvar.sqltype & ~1;
			switch (type)
			{
				case IscCodes.SQL_VARYING:
					{
						var buffer = new byte[Marshal.ReadInt16(xsqlvar.sqldata)];
						var tmp = GetIntPtr(xsqlvar.sqldata, 2);
						Marshal.Copy(tmp, buffer, 0, buffer.Length);
						return buffer;
					}
				case IscCodes.SQL_TEXT:
				case IscCodes.SQL_SHORT:
				case IscCodes.SQL_LONG:
				case IscCodes.SQL_FLOAT:
				case IscCodes.SQL_DOUBLE:
				case IscCodes.SQL_D_FLOAT:
				case IscCodes.SQL_QUAD:
				case IscCodes.SQL_INT64:
				case IscCodes.SQL_BLOB:
				case IscCodes.SQL_ARRAY:
				case IscCodes.SQL_TIMESTAMP:
				case IscCodes.SQL_TYPE_TIME:
				case IscCodes.SQL_TYPE_DATE:
				case IscCodes.SQL_BOOLEAN:
					{
						var buffer = new byte[xsqlvar.sqllen];
						Marshal.Copy(xsqlvar.sqldata, buffer, 0, buffer.Length);
						return buffer;
					}
				default:
					throw TypeHelper.InvalidDataType(type);
			}
		}

		private static byte[] GetStringBuffer(Charset charset, string value)
		{
			var buffer = new byte[68];
			charset.GetBytes(value, 0, value.Length, buffer, 0);
			return buffer;
		}

		private static byte[] GetStringBuffer_V1(Charset charset, string value)
		{
			var buffer = new byte[32];
			charset.GetBytes(value, 0, value.Length, buffer, 0);
			return buffer;
		}

		private static string GetString(Charset charset, byte[] buffer)
		{
			var value = charset.GetString(buffer);
			return value.TrimEnd('\0', ' ');
		}

		private static string GetString(Charset charset, byte[] buffer, short bufferLength)
		{
			return charset.GetString(buffer, 0, bufferLength);
		}
	}
}
