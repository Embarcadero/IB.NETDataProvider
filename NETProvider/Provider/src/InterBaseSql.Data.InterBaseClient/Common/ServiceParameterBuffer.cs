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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Text;

namespace InterBaseSql.Data.Common;

internal sealed class ServiceParameterBuffer : ServiceParameterBufferBase
{
	public ServiceParameterBuffer(Encoding encoding)
		: base(encoding)
	{ }

	public override void AppendPreamble()
	{
		Append(IscCodes.isc_spb_version);
		Append(IscCodes.isc_spb_current_version);
	}

	public override void Append1(int type, byte[] value)
	{
		WriteByte(type);
		WriteByte((byte)value.Length);
		Write(value);
	}

	public override void Append2(int type, byte[] value)
	{
		WriteByte(type);
		Write((short)value.Length);
		Write(value);
	}

	public override void Append(int type, byte value)
	{
		WriteByte(type);
		WriteByte(value);
	}

	public override void Append(int type, int value)
	{
		WriteByte(type);
		Write(value);
	}
	public override void Append(int type, long value)
	{
		WriteByte(type);
		Write(value);
	}
}