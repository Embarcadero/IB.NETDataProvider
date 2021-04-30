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

namespace InterBaseSql.Data.Client.Native.Marshalers
{
	[StructLayout(LayoutKind.Sequential)]
	public class CTimeStructure
	{
		public int tm_sec;  // Seconds 
		public int tm_min;   // Minutes 
		public int tm_hour;  // Hour (0--23) 
		public int tm_mday;  // Day of month (1--31) 
		public int tm_mon;   // Month (0--11) 
		public int tm_year;  // Year (calendar year minus 1900) 
		public int tm_wday;  // Weekday (0--6) Sunday = 0) 
		public int tm_yday;  // Day of year (0--365) 
		public int tm_isdst; // 0 if daylight savings time is not in effect) 
	}
}
