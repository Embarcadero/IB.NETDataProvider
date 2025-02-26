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

namespace InterBaseSql.Data.InterBaseClient
{
	[Serializable]
	public enum IBCharset
	{
		Default = -1,
		None = 0,
		Octets = 1,
		Ascii = 2,
		UnicodeFss = 3,
		Utf8 = 59,
		ShiftJis0208 = 5,
		EucJapanese0208 = 6,
		Iso2022Japanese = 7,
		Dos437 = 10,
		Dos850 = 11,
		Dos865 = 12,
		Dos860 = 13,
		Dos863 = 14,
		Iso8859_1 = 21,
		Iso8859_2 = 22,
		Ksc5601 = 44,
		Dos861 = 47,
		Windows1250 = 51,
		Windows1251 = 52,
		Windows1252 = 53,
		Windows1253 = 54,
		Windows1254 = 55,
		Big5 = 56,
		Gb2312 = 57,
		UNICODE_BE = 8,
		ISO8859_15 = 39,
		KOI8R = 58,
		UNICODE_LE = 64,
	}
}
