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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.Services
{
	public sealed class IBServerProperties : IBService
	{
		public IBServerProperties(string connectionString = null)
			: base(connectionString)
		{ }

		public int GetVersion()
		{
			return GetInt32(IscCodes.isc_info_svc_version);
		}

		public string GetServerVersion()
		{
			return GetString(IscCodes.isc_info_svc_server_version);
		}

		public string GetImplementation()
		{
			return GetString(IscCodes.isc_info_svc_implementation);
		}

		public string GetRootDirectory()
		{
			return GetString(IscCodes.isc_info_svc_get_env);
		}

		public string GetLockManager()
		{
			return GetString(IscCodes.isc_info_svc_get_env_lock);
		}

		public string GetMessageFile()
		{
			return GetString(IscCodes.isc_info_svc_get_env_msg);
		}

		public IBDatabasesInfo GetDatabasesInfo()
		{
			return (IBDatabasesInfo)GetInfo(IscCodes.isc_info_svc_svr_db_info).FirstOrDefault() ?? new IBDatabasesInfo();
		}

		public IBServerConfig GetServerConfig()
		{
			return (IBServerConfig)GetInfo(IscCodes.isc_info_svc_get_config).FirstOrDefault() ?? new IBServerConfig();
		}

		private string GetString(int item)
		{
			return (string)GetInfo(item).FirstOrDefault();
		}

		private int GetInt32(int item)
		{
			return (int)GetInfo(item).FirstOrDefault();
		}

		private IList<object> GetInfo(int item)
		{
			return GetInfo(new byte[] { (byte)item });
		}

		private IList<object> GetInfo(byte[] items)
		{
			return Query(items, EmptySpb);
		}

		public static Version ParseServerVersion(string version)
		{
			var m = Regex.Match(version, @"\w{2}-\w(\d+\.\d+\.\d+\.\d+)");
			if (!m.Success)
				return null;
			return new Version(m.Groups[1].Value);
		}
	}
}
