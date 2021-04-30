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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;

using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.Services
{
	public sealed class IBConfiguration : IBService
	{
		public IBConfiguration(string connectionString = null)
			: base(connectionString)
		{ }

		public void SetSqlDialect(int sqlDialect)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			startSpb.Append(IscCodes.isc_spb_prp_set_sql_dialect, sqlDialect);
			StartTask(startSpb);
			Close();
		}

		public void SetSweepInterval(int sweepInterval)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			startSpb.Append(IscCodes.isc_spb_prp_sweep_interval, sweepInterval);
			StartTask(startSpb);
			Close();
		}

		public void SetPageBuffers(int pageBuffers)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			startSpb.Append(IscCodes.isc_spb_prp_page_buffers, pageBuffers);
			StartTask(startSpb);
			Close();
		}

		public void DatabaseShutdown(IBShutdownMode mode, int seconds)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			switch (mode)
			{
				case IBShutdownMode.Forced:
					startSpb.Append(IscCodes.isc_spb_prp_shutdown_db, seconds);
					break;
				case IBShutdownMode.DenyTransaction:
					startSpb.Append(IscCodes.isc_spb_prp_deny_new_transactions, seconds);
					break;
				case IBShutdownMode.DenyConnection:
					startSpb.Append(IscCodes.isc_spb_prp_deny_new_attachments, seconds);
					break;
			}
			StartTask(startSpb);
			Close();
		}

		public void DatabaseOnline()
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			startSpb.Append(IscCodes.isc_spb_options, IscCodes.isc_spb_prp_db_online);
			StartTask(startSpb);
			Close();
		}

		public void ActivateShadows()
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			startSpb.Append(IscCodes.isc_spb_options, IscCodes.isc_spb_prp_activate);
			StartTask(startSpb);
			Close();
		}

		public void SetForcedWrites(bool forcedWrites)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			if (forcedWrites)
			{
				startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_sync);
			}
			else
			{
				startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_async);
			}
			StartTask(startSpb);
			Close();
		}

		public void SetReserveSpace(bool reserveSpace)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			if (reserveSpace)
			{
				startSpb.Append(IscCodes.isc_spb_prp_reserve_space, (byte)IscCodes.isc_spb_prp_res);
			}
			else
			{
				startSpb.Append(IscCodes.isc_spb_prp_reserve_space, (byte)IscCodes.isc_spb_prp_res_use_full);
			}
			StartTask(startSpb);
			Close();
		}

		public void SetAccessMode(bool readOnly)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			startSpb.Append(IscCodes.isc_spb_prp_access_mode, (byte)(readOnly ? IscCodes.isc_spb_prp_am_readonly : IscCodes.isc_spb_prp_am_readwrite));
			StartTask(startSpb);
			Close();
		}

		public void SetWriteMode(IBWriteMode Value)
		{
			EnsureDatabase();

			Open();
			var startSpb = new ServiceParameterBuffer();
			startSpb.Append(IscCodes.isc_action_svc_properties);
			startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);
			switch (Value)
			{
				case IBWriteMode.wmSync:
					startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_sync);
					break;
				case IBWriteMode.wmASync:
					startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_async);
					break;
				case IBWriteMode.wmDirect:
					startSpb.Append(IscCodes.isc_spb_prp_write_mode, (byte)IscCodes.isc_spb_prp_wm_direct);
					break;
			}
			StartTask(startSpb);
			Close();
		}

	}
}
