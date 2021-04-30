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

//$Authors = Hennadii Zabula

using System;
using System.Diagnostics.Contracts;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.Client.Native.Handle
{
	// public visibility added, because auto-generated assembly can't work with internal types
	public class DatabaseHandle : InterBaseHandle
	{
		protected override bool ReleaseHandle()
		{
			Contract.Requires(IBClient != null);

			if (IsClosed)
			{
				return true;
			}

			var statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
			var @ref = this;
			IBClient.isc_detach_database(statusVector, ref @ref);
			handle = @ref.handle;
			var exception = IBConnection.ParseStatusVector(statusVector, Charset.DefaultCharset);
			return exception == null || exception.IsWarning;
		}
	}
}
