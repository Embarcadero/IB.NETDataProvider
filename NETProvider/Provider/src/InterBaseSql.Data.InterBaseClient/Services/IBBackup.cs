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

using System;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Services;

public sealed class IBBackup : IBService
{
	public IBBackupFileCollection BackupFiles { get; }
	public IBBackupTablespaceCollection IncludeTablespaces { get; }
	public IBBackupTablespaceCollection ExcludeTablespaces { get; }

	public bool Verbose { get; set; }
	public int Factor { get; set; }
	public int PreAllocate { get; set; }
	public string EncryptName { get; set; }
	public string EncryptPassword { get; set; }
	public IBBackupFlags Options { get; set; }

	public IBBackup(string connectionString = null)
		: base(connectionString)
	{
		BackupFiles = new IBBackupFileCollection();
		IncludeTablespaces = new IBBackupTablespaceCollection();
		ExcludeTablespaces = new IBBackupTablespaceCollection();
	}

	public void Execute()
	{
		EnsureDatabase();

		try
		{
			try
			{
				Open();
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_backup);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				foreach (var file in BackupFiles)
				{
					startSpb.Append2(IscCodes.isc_spb_bkp_file, file.BackupFile);
					if (file.BackupLength.HasValue)
						startSpb.Append(IscCodes.isc_spb_bkp_length, (int)file.BackupLength);
				}
				if (Verbose)
					startSpb.Append(IscCodes.isc_spb_verbose);
				if (Factor > 0)
					startSpb.Append(IscCodes.isc_spb_bkp_factor, Factor);
				foreach (var tablespace in IncludeTablespaces)
				{
					if (!string.IsNullOrEmpty(tablespace))
						startSpb.Append1(IscCodes.isc_spb_tablespace_include, tablespace);
				}
				foreach (var tablespace in ExcludeTablespaces)
				{
					if (!string.IsNullOrEmpty(tablespace))
						startSpb.Append1(IscCodes.isc_spb_tablespace_exclude, tablespace);
				}
				startSpb.Append(IscCodes.isc_spb_options, (int)Options);
				if (PreAllocate != 0)
					startSpb.Append(IscCodes.isc_spb_bkp_preallocate, PreAllocate);

				if (!string.IsNullOrEmpty(EncryptName))
					startSpb.Append1(IscCodes.isc_spb_bkp_encrypt_name, EncryptName);
				if (!string.IsNullOrEmpty(EncryptPassword))
					startSpb.Append1(IscCodes.isc_spb_sys_encrypt_password, EncryptPassword);
				StartTask(startSpb);
				if (Verbose)
				{
					ProcessServiceOutput(new ServiceParameterBuffer(Service.ParameterBufferEncoding));
				}
			}
			finally
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			throw IBException.Create(ex);
		}
	}
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		EnsureDatabase();

		try
		{
			try
			{
				await OpenAsync(cancellationToken).ConfigureAwait(false);
				var startSpb = new ServiceParameterBuffer(Service.ParameterBufferEncoding);
				startSpb.Append(IscCodes.isc_action_svc_backup);
				startSpb.Append2(IscCodes.isc_spb_dbname, ConnectionStringOptions.Database);
				foreach (var file in BackupFiles)
				{
					startSpb.Append2(IscCodes.isc_spb_bkp_file, file.BackupFile);
					if (file.BackupLength.HasValue)
						startSpb.Append(IscCodes.isc_spb_bkp_length, (int)file.BackupLength);
				}
				if (Verbose)
					startSpb.Append(IscCodes.isc_spb_verbose);
				if (Factor > 0)
					startSpb.Append(IscCodes.isc_spb_bkp_factor, Factor);
				foreach (var tablespace in IncludeTablespaces)
				{
					if (!string.IsNullOrEmpty(tablespace))
						startSpb.Append1(IscCodes.isc_spb_tablespace_include, tablespace);
				}
				foreach (var tablespace in ExcludeTablespaces)
				{
					if (!string.IsNullOrEmpty(tablespace))
						startSpb.Append1(IscCodes.isc_spb_tablespace_exclude, tablespace);
				}
				startSpb.Append(IscCodes.isc_spb_options, (int)Options);
				if (PreAllocate != 0)
					startSpb.Append(IscCodes.isc_spb_bkp_preallocate, PreAllocate);

				if (!string.IsNullOrEmpty(EncryptName))
					startSpb.Append1(IscCodes.isc_spb_bkp_encrypt_name, EncryptName);
				if (!string.IsNullOrEmpty(EncryptPassword))
					startSpb.Append1(IscCodes.isc_spb_sys_encrypt_password, EncryptPassword);
				await StartTaskAsync(startSpb, cancellationToken).ConfigureAwait(false);
				if (Verbose)
				{
					await ProcessServiceOutputAsync(new ServiceParameterBuffer(Service.ParameterBufferEncoding), cancellationToken).ConfigureAwait(false);
				}
			}
			finally
			{
				await CloseAsync(cancellationToken).ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			throw IBException.Create(ex);
		}
	}
}