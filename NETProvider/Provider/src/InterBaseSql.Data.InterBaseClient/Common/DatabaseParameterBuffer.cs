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

using System.Text;

// Because InterBase only has 1 parameter buffer type no need for base/buffer1/buffer2 from Fb
//   Continue to jsut use the original DatabaseParameterBuffer
namespace InterBaseSql.Data.Common;

internal sealed class DatabaseParameterBuffer : ParameterBuffer
{
	public DatabaseParameterBuffer(Encoding encoding)
	{
		Encoding = encoding;
		Append(IscCodes.isc_dpb_version1);
	}

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

	public void Append(int type, byte[] buffer)
	{
		WriteByte(type);
		WriteByte(buffer.Length);
		Write(buffer);
	}
	public void Append(int type, string content) => Append(type, Encoding.GetBytes(content));
	public Encoding Encoding { get; }

}
