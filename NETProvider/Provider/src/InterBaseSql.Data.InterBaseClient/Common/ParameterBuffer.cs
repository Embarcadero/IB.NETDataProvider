﻿/*
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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.IO;
using System.Text;
using System.Net;

namespace InterBaseSql.Data.Common
{
	internal abstract class ParameterBuffer
	{
		private MemoryStream _stream;

		public short Length => (short)_stream.Length;

		protected ParameterBuffer()
		{
			_stream = new MemoryStream();
		}

		public virtual void Append(int type)
		{
			WriteByte(type);
		}

		public byte[] ToArray()
		{
			return _stream.ToArray();
		}

		protected void WriteByte(int value)
		{
			WriteByte((byte)value);
		}

		protected void WriteByte(byte value)
		{
			_stream.WriteByte(value);
		}

		protected void Write(byte value)
		{
			WriteByte(value);
		}

		protected void Write(short value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = IPAddress.NetworkToHostOrder(value);
			}

			var buffer = BitConverter.GetBytes(value);

			_stream.Write(buffer, 0, buffer.Length);
		}

		protected void Write(int value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = IPAddress.NetworkToHostOrder(value);
			}

			var buffer = BitConverter.GetBytes(value);

			_stream.Write(buffer, 0, buffer.Length);
		}

		protected void Write(long value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				value = IPAddress.NetworkToHostOrder(value);
			}

			var buffer = BitConverter.GetBytes(value);

			_stream.Write(buffer, 0, buffer.Length);
		}

		protected void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		protected void Write(byte[] buffer, int offset, int count)
		{
			_stream.Write(buffer, offset, count);
		}
	}
}
