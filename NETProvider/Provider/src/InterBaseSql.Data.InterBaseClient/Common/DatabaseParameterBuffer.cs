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
	internal sealed class DatabaseParameterBuffer : ParameterBuffer
	{
		public DatabaseParameterBuffer()
		{ }

		public void Append(int type, byte value)
		{
			WriteByte(type);
			WriteByte(1);
			Write(value);
		}

		public void Append(int type, short value)
		{
			WriteByte(type);
			WriteByte(2);
			Write(value);
		}

		public void Append(int type, int value)
		{
			WriteByte(type);
			WriteByte((byte)4);
			Write(value);
		}

		public void Append(int type, string content)
		{
			Append(type, Encoding.Default.GetBytes(content));
		}

		public void Append(int type, byte[] buffer)
		{
			WriteByte(type);
			WriteByte(buffer.Length);
			Write(buffer);
		}
	}
}
