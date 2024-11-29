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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using InterBaseSql.Data.Client.Native;
using System;
using System.IO;

namespace InterBaseSql.Data.Common;

/// <summary>
/// Descriptor of query input and output parameters.
/// </summary>
/// <remarks>
/// This is similar to the XSQLDA structure described
/// in the Interbase 6.0 API docs.
/// </remarks>
internal sealed class Descriptor
{
	#region Fields

	private short _version;
	private short _count;
	private short _actualCount;
	private DbField[] _fields;

	#endregion

	#region Properties

	public IBDatabase Database;

	public short Version
	{
		get { return _version; }
		set { _version = value; }	
	}

	public short Count
	{
		get { return _count; }
	}

	public short ActualCount
	{
		get { return _actualCount; }
		set { _actualCount = value; }
	}

	#endregion

	#region Indexers

	public DbField this[int index]
	{
		get { return _fields[index]; }
	}

	#endregion

	#region Constructors

	public Descriptor(IBDatabase DB, short n)
	{
		_version = IscCodes.SQLDA_CURRENT_VERSION;
		_count = n;
		_actualCount = n;
		_fields = new DbField[n];
		Database = DB;
		for (var i = 0; i < n; i++)
		{
			_fields[i] = new DbField(DB);
		}
	}

	#endregion

	#region Methods

	public void ResetValues()
	{
		for (var i = 0; i < _fields.Length; i++)
		{
			_fields[i].SetValue(null);
		}
	}

	internal sealed class BlrData
	{
		public byte[] Data { get; }
		public int Length { get; }

		public BlrData(byte[] data, int length)
		{
			Data = data;
			Length = length;
		}
	}

	public BlrData ToBlr()
	{
		using (var blr = new MemoryStream(256))
		{
			var length = 0;

			blr.WriteByte(IscCodes.blr_version5);
			blr.WriteByte(IscCodes.blr_begin);
			blr.WriteByte(IscCodes.blr_message);
			blr.WriteByte(0);
			var par_count = Count * 2;
			blr.WriteByte((byte)(par_count & 255));
			blr.WriteByte((byte)(par_count >> 8));

			for (var i = 0; i < _fields.Length; i++)
			{
				var dtype = _fields[i].SqlType;
				int len = _fields[i].Length;

				switch (dtype)
				{
					case IscCodes.SQL_VARYING:
						blr.WriteByte(IscCodes.blr_varying);
						blr.WriteByte((byte)(len & 255));
						blr.WriteByte((byte)(len >> 8));
						length = TypeHelper.BlrAlign(length, 2);
						length += len + 2;
						break;

					case IscCodes.SQL_TEXT:
						blr.WriteByte(IscCodes.blr_text);
						blr.WriteByte((byte)(len & 255));
						blr.WriteByte((byte)(len >> 8));
						// no align
						length += len;
						break;

					case IscCodes.SQL_DOUBLE:
						blr.WriteByte(IscCodes.blr_double);
						length = TypeHelper.BlrAlign(length, 8);
						length += 8;
						break;

					case IscCodes.SQL_FLOAT:
						blr.WriteByte(IscCodes.blr_float);
						length = TypeHelper.BlrAlign(length, 4);
						length += 4;
						break;

					case IscCodes.SQL_D_FLOAT:
						blr.WriteByte(IscCodes.blr_d_float);
						length = TypeHelper.BlrAlign(length, 8);
						length += 8;
						break;

					case IscCodes.SQL_TYPE_DATE:
						blr.WriteByte(IscCodes.blr_sql_date);
						length = TypeHelper.BlrAlign(length, 4);
						length += 4;
						break;

					case IscCodes.SQL_TYPE_TIME:
						blr.WriteByte(IscCodes.blr_sql_time);
						length = TypeHelper.BlrAlign(length, 4);
						length += 4;
						break;

					case IscCodes.SQL_TIMESTAMP:
						blr.WriteByte(IscCodes.blr_timestamp);
						length = TypeHelper.BlrAlign(length, 4);
						length += 8;
						break;

					case IscCodes.SQL_BLOB:
						blr.WriteByte(IscCodes.blr_quad);
						blr.WriteByte(0);
						length = TypeHelper.BlrAlign(length, 4);
						length += 8;
						break;

					case IscCodes.SQL_ARRAY:
						blr.WriteByte(IscCodes.blr_quad);
						blr.WriteByte(0);
						length = TypeHelper.BlrAlign(length, 4);
						length += 8;
						break;

					case IscCodes.SQL_LONG:
						blr.WriteByte(IscCodes.blr_long);
						blr.WriteByte((byte)_fields[i].NumericScale);
						length = TypeHelper.BlrAlign(length, 4);
						length += 4;
						break;

					case IscCodes.SQL_SHORT:
						blr.WriteByte(IscCodes.blr_short);
						blr.WriteByte((byte)_fields[i].NumericScale);
						length = TypeHelper.BlrAlign(length, 2);
						length += 2;
						break;

					case IscCodes.SQL_INT64:
						blr.WriteByte(IscCodes.blr_int64);
						blr.WriteByte((byte)_fields[i].NumericScale);
						length = TypeHelper.BlrAlign(length, 8);
						length += 8;
						break;

					case IscCodes.SQL_QUAD:
						blr.WriteByte(IscCodes.blr_quad);
						blr.WriteByte((byte)_fields[i].NumericScale);
						length = TypeHelper.BlrAlign(length, 4);
						length += 8;
						break;

					case IscCodes.SQL_BOOLEAN:
						blr.WriteByte(IscCodes.blr_boolean);
						length = TypeHelper.BlrAlign(length, 1);
						length += 1;
						break;
				}

				blr.WriteByte(IscCodes.blr_short);
				blr.WriteByte(0);

				length = TypeHelper.BlrAlign(length, 2);
				length += 2;
			}

			blr.WriteByte(IscCodes.blr_end);
			blr.WriteByte(IscCodes.blr_eoc);

			return new BlrData(blr.ToArray(), length);
		}
	}

	#endregion
}
