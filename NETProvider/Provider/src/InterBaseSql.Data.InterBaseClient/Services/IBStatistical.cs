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

using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Services
{
	public sealed class IBStatistical : IBService
	{
		public IBStatisticalFlags Options { get; set; }

		public IBStatistical(string connectionString = null)
			: base(connectionString)
		{ }

		public void Execute()
		{
			EnsureDatabase();

			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer();
				startSpb.Append(IscCodes.isc_action_svc_db_stats);
				startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
				startSpb.Append(IscCodes.isc_spb_options, (int)Options);
				StartTask(startSpb);
				ProcessServiceOutput(EmptySpb);
			}
			catch (Exception ex)
			{
				throw new IBException(ex.Message, ex);
			}
			finally
			{
				Close();
			}
		}
	}
}
