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
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Services
{
	public sealed class IBRestore : IBService
	{
		private int? _pageSize;
		public int? PageSize
		{
			get { return _pageSize; }
			set
			{
				if (value.HasValue && !PageSizeHelper.IsValidPageSize((int)value))
					throw new InvalidOperationException("Invalid page size.");

				_pageSize = value;
			}
		}

		public IBBackupFileCollection BackupFiles { get; }
		public IBRestoreIncludeTablespaceInfoCollection IncludeTablespaces { get; }
		public IBRestoreExcludeTablespaceInfoCollection ExcludeTablespaces { get; }

		public bool Verbose { get; set; }
		public int? PageBuffers { get; set; }
		public bool ReadOnly { get; set; }
		public string EUAUserName { get; set; }
		public string EUAPassword { get; set; }
		public string EncryptPassword { get; set; }
		public string DecryptPassword { get; set; }
		public int? ODSMajorVersion { get; set; }
		public long? StartingTransactionID { get; set; }
		public int? PreAllocate { get; set; }
		public IBWriteMode? WriteMode { get; set; }

		public IBRestoreFlags Options { get; set; }
		public IBRestoreType RestoreType { get; set; }

		public IBRestore(string connectionString = null)
			: base(connectionString)
		{
			BackupFiles = new IBBackupFileCollection();
			IncludeTablespaces = new IBRestoreIncludeTablespaceInfoCollection();
			ExcludeTablespaces = new IBRestoreExcludeTablespaceInfoCollection();
			RestoreType = IBRestoreType.rtDatabase;
		}

		public void Execute()
		{
			EnsureDatabase();

			try
			{
				Open();

				var startSpb = new ServiceParameterBuffer();
				startSpb.Append(IscCodes.isc_action_svc_restore);

				if ((Options & IBRestoreFlags.MetaDataOnly) == IBRestoreFlags.MetaDataOnly)
					RestoreType = IBRestoreType.rtDatabase;
				// Make sure Replace and REplaceTablespace or Create and CreateTablespace are mutually exclusive
				if (RestoreType == IBRestoreType.rtDatabase)
				{
					if ((Options & IBRestoreFlags.Replace) == IBRestoreFlags.Replace)
						Options &= ~IBRestoreFlags.ReplaceTablespace;
					if ((Options & IBRestoreFlags.Create) == IBRestoreFlags.Create)
						Options &= ~IBRestoreFlags.CreateTablespace;
				}
				else
				{
					if ((Options & IBRestoreFlags.ReplaceTablespace) == IBRestoreFlags.ReplaceTablespace)
						Options &= ~IBRestoreFlags.Replace;
					if ((Options & IBRestoreFlags.CreateTablespace) == IBRestoreFlags.CreateTablespace)
						Options &= ~IBRestoreFlags.Create;
				}
				startSpb.Append(IscCodes.isc_spb_options, (int)Options);

				if (Verbose)
					startSpb.Append(IscCodes.isc_spb_verbose);
				if (PageBuffers.HasValue)
					startSpb.Append(IscCodes.isc_spb_res_buffers, (int)PageBuffers);
				if (_pageSize.HasValue)
					startSpb.Append(IscCodes.isc_spb_res_page_size, (int)_pageSize);
				if (!string.IsNullOrEmpty(EUAUserName))
					startSpb.Append(IscCodes.isc_spb_res_eua_user_name, EUAUserName);
				if (!string.IsNullOrEmpty(EUAPassword))
					startSpb.Append(IscCodes.isc_spb_res_eua_password, EUAPassword);
				if (ODSMajorVersion.HasValue)
					startSpb.Append(IscCodes.isc_spb_res_ods_version_major, (int)ODSMajorVersion);
				if (StartingTransactionID.HasValue)
					startSpb.Append(IscCodes.isc_spb_res_starting_trans, (long)StartingTransactionID);
				if (!string.IsNullOrEmpty(EncryptPassword))
					startSpb.Append(IscCodes.isc_spb_sys_encrypt_password, EncryptPassword);
				if (!string.IsNullOrEmpty(DecryptPassword))
					startSpb.Append(IscCodes.isc_spb_res_decrypt_password, DecryptPassword);

				startSpb.Append(IscCodes.isc_spb_res_access_mode, (byte)(ReadOnly ? IscCodes.isc_spb_res_am_readonly : IscCodes.isc_spb_res_am_readwrite));
				if (PreAllocate.HasValue)
					startSpb.Append(IscCodes.isc_spb_res_preallocate, (int)PreAllocate);
				if (WriteMode.HasValue)
                    switch (WriteMode)
					{
						case IBWriteMode.wmASync:
							startSpb.Append(IscCodes.isc_spb_res_write_mode, IscCodes.isc_spb_res_wm_async);
							break;
						case IBWriteMode.wmSync:
							startSpb.Append(IscCodes.isc_spb_res_write_mode, IscCodes.isc_spb_res_wm_sync);
							break;
						case IBWriteMode.wmDirect:
							startSpb.Append(IscCodes.isc_spb_res_write_mode, IscCodes.isc_spb_res_wm_direct);
							break;

					}
				foreach (var bkpFile in BackupFiles)
				{
					startSpb.Append(IscCodes.isc_spb_bkp_file, bkpFile.BackupFile, SpbFilenameEncoding);
					if (bkpFile.BackupLength.HasValue)
						startSpb.Append(IscCodes.isc_spb_bkp_length, (int) bkpFile.BackupLength);
				}
				startSpb.Append(IscCodes.isc_spb_dbname, Database, SpbFilenameEncoding);

				foreach (var incTablespace in IncludeTablespaces)
				{
					if (!string.IsNullOrEmpty(incTablespace.Name))
					{
						startSpb.Append(IscCodes.isc_spb_tablespace_include, incTablespace.Name);
						if (!string.IsNullOrEmpty(incTablespace.Location))
						    startSpb.Append(IscCodes.isc_spb_tablespace_file, incTablespace.Location, SpbFilenameEncoding);
					}
				}
				foreach (var excTablespace in ExcludeTablespaces)
				{
					if (!string.IsNullOrEmpty(excTablespace))
						startSpb.Append(IscCodes.isc_spb_tablespace_exclude, excTablespace);
				}

				StartTask(startSpb);
				if (Verbose)
				{
					ProcessServiceOutput(EmptySpb);
				}
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
