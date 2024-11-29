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
using System.Text;
using System.Data;

using InterBaseSql.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace InterBaseSql.Data.InterBaseClient;

public sealed class IBDatabaseInfo
{
	#region Properties

	public IBConnection Connection { get; set; }

	#endregion

	#region Methods

	public string GetIscVersion()
	{
		return GetValue<string>(IscCodes.isc_info_version);
	}
	public Task<string> GetIscVersionAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<string>(IscCodes.isc_info_version, cancellationToken);
	}

	public string GetServerVersion()
	{
		return GetValue<string>(IscCodes.isc_info_svc_server_version);
	}
	public Task<string> GetServerVersionAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<string>(IscCodes.isc_info_svc_server_version, cancellationToken);
	}

	public string GetServerImplementation()
	{
		return GetValue<string>(IscCodes.isc_info_svc_implementation);
	}
	public Task<string> GetServerImplementationAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<string>(IscCodes.isc_info_svc_implementation, cancellationToken);
	}

	public int GetPageSize()
	{
		return GetValue<int>(IscCodes.isc_info_page_size);
	}
	public Task<int> GetPageSizeAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_page_size, cancellationToken);
	}

	public int GetAllocationPages()
	{
		return GetValue<int>(IscCodes.isc_info_allocation);
	}
	public Task<int> GetAllocationPagesAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_allocation, cancellationToken);
	}

	public string GetBaseLevel()
	{
		return GetValue<string>(IscCodes.isc_info_base_level);
	}
	public Task<string> GetBaseLevelAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<string>(IscCodes.isc_info_base_level, cancellationToken);
	}

	public string GetDbId()
	{
		return GetValue<string>(IscCodes.isc_info_db_id);
	}
	public Task<string> GetDbIdAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<string>(IscCodes.isc_info_db_id, cancellationToken);
	}

	public string GetImplementation()
	{
		return GetValue<string>(IscCodes.isc_info_implementation);
	}
	public Task<string> GetImplementationAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<string>(IscCodes.isc_info_implementation, cancellationToken);
	}

	public bool GetNoReserve()
	{
		return GetValue<bool>(IscCodes.isc_info_no_reserve);
	}
	public Task<bool> GetNoReserveAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<bool>(IscCodes.isc_info_no_reserve, cancellationToken);
	}

	public int GetOdsVersion()
	{
		return GetValue<int>(IscCodes.isc_info_ods_version);
	}
	public Task<int> GetOdsVersionAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_ods_version, cancellationToken);
	}

	public int GetOdsMinorVersion()
	{
		return GetValue<int>(IscCodes.isc_info_ods_minor_version);
	}
	public Task<int> GetOdsMinorVersionAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_ods_minor_version, cancellationToken);
	}

	public int GetMaxMemory()
	{
		return GetValue<int>(IscCodes.isc_info_max_memory);
	}
	public Task<int> GetMaxMemoryAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_max_memory, cancellationToken);
	}

	public int GetCurrentMemory()
	{
		return GetValue<int>(IscCodes.isc_info_current_memory);
	}
	public Task<int> GetCurrentMemoryAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_current_memory, cancellationToken);
	}

	public string GetForcedWrites()
	{
		return GetValue<string>(IscCodes.isc_info_forced_writes);
	}
	public Task<string> GetForcedWritesAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<string>(IscCodes.isc_info_forced_writes, cancellationToken);
	}

	public int GetNumBuffers()
	{
		return GetValue<int>(IscCodes.isc_info_num_buffers);
	}
	public Task<int> GetNumBuffersAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_num_buffers, cancellationToken);
	}

	public int GetSweepInterval()
	{
		return GetValue<int>(IscCodes.isc_info_sweep_interval);
	}
	public Task<int> GetSweepIntervalAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_sweep_interval, cancellationToken);
	}

	public bool GetReadOnly()
	{
		return GetValue<bool>(IscCodes.isc_info_db_read_only);
	}
	public Task<bool> GetReadOnlyAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<bool>(IscCodes.isc_info_db_read_only, cancellationToken);
	}

	public int GetFetches()
	{
		return GetValue<int>(IscCodes.isc_info_fetches);
	}
	public Task<int> GetFetchesAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_fetches, cancellationToken);
	}

	public int GetMarks()
	{
		return GetValue<int>(IscCodes.isc_info_marks);
	}
	public Task<int> GetMarksAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_marks, cancellationToken);
	}

	public int GetReads()
	{
		return GetValue<int>(IscCodes.isc_info_reads);
	}
	public Task<int> GetReadsAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_reads, cancellationToken);
	}

	public int GetWrites()
	{
		return GetValue<int>(IscCodes.isc_info_writes);
	}
	public Task<int> GetWritesAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_writes, cancellationToken);
	}

	public int GetBackoutCount()
	{
		return GetValue<int>(IscCodes.isc_info_backout_count);
	}
	public Task<int> GetBackoutCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_backout_count, cancellationToken);
	}

	public int GetDeleteCount()
	{
		return GetValue<int>(IscCodes.isc_info_delete_count);
	}
	public Task<int> GetDeleteCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_delete_count, cancellationToken);
	}

	public int GetExpungeCount()
	{
		return GetValue<int>(IscCodes.isc_info_expunge_count);
	}
	public Task<int> GetExpungeCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_expunge_count, cancellationToken);
	}

	public int GetInsertCount()
	{
		return GetValue<int>(IscCodes.isc_info_insert_count);
	}
	public Task<int> GetInsertCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_insert_count, cancellationToken);
	}

	public int GetPurgeCount()
	{
		return GetValue<int>(IscCodes.isc_info_purge_count);
	}
	public Task<int> GetPurgeCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_purge_count, cancellationToken);
	}

	public long GetReadIdxCount()
	{
		return GetValue<long>(IscCodes.isc_info_read_idx_count);
	}
	public Task<long> GetReadIdxCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<long>(IscCodes.isc_info_read_idx_count, cancellationToken);
	}

	public long GetReadSeqCount()
	{
		return GetValue<long>(IscCodes.isc_info_read_seq_count);
	}
	public Task<long> GetReadSeqCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<long>(IscCodes.isc_info_read_seq_count, cancellationToken);
	}

	public long GetUpdateCount()
	{
		return GetValue<long>(IscCodes.isc_info_update_count);
	}
	public Task<long> GetUpdateCountAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<long>(IscCodes.isc_info_update_count, cancellationToken);
	}

	public int GetDatabaseSizeInPages()
	{
		return GetValue<int>(IscCodes.isc_info_db_size_in_pages);
	}
	public Task<int> GetDatabaseSizeInPagesAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_db_size_in_pages, cancellationToken);
	}

	public int GetDBSQLDialect()
	{
		return GetValue<int>(IscCodes.isc_info_db_sql_dialect);
	}

	public Task<int> GetDBSQLDialectAsync(CancellationToken cancellationToken = default)
	{
		return GetValueAsync<int>(IscCodes.isc_info_db_sql_dialect, cancellationToken);
	}

	public List<string> GetActiveUsers()
	{
		return GetList<string>(IscCodes.isc_info_user_names);
	}
	public Task<List<string>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
	{
		return GetListAsync<string>(IscCodes.isc_info_user_names, cancellationToken);
	}

	#endregion

	#region Constructors

	public IBDatabaseInfo(IBConnection connection = null)
	{
		Connection = connection;
	}

	#endregion

	#region Private Methods

	private T GetValue<T>(byte item)
	{
		IBConnection.EnsureOpen(Connection);

		var items = new byte[]
		{
			item,
			IscCodes.isc_info_end
		};
		var info = Connection.InnerConnection.Database.GetDatabaseInfo(items);
		return info.Any() ? InfoValuesHelper.ConvertValue<T>(info[0]) : default;
	}
	private async Task<T> GetValueAsync<T>(byte item, CancellationToken cancellationToken = default)
	{
		IBConnection.EnsureOpen(Connection);

		var items = new byte[]
		{
			item,
			IscCodes.isc_info_end
		};
		var info = await Connection.InnerConnection.Database.GetDatabaseInfoAsync(items, cancellationToken).ConfigureAwait(false);
		return info.Any() ? InfoValuesHelper.ConvertValue<T>(info[0]) : default;
	}

	private List<T> GetList<T>(byte item)
	{
		IBConnection.EnsureOpen(Connection);

		var items = new byte[]
			{
				item,
				IscCodes.isc_info_end
			};

		return (Connection.InnerConnection.Database.GetDatabaseInfo(items)).Select(InfoValuesHelper.ConvertValue<T>).ToList();
	}
	private async Task<List<T>> GetListAsync<T>(byte item, CancellationToken cancellationToken = default)
	{
		IBConnection.EnsureOpen(Connection);

		var items = new byte[]
			{
				item,
				IscCodes.isc_info_end
			};

		return (await Connection.InnerConnection.Database.GetDatabaseInfoAsync(items, cancellationToken).ConfigureAwait(false)).Select(InfoValuesHelper.ConvertValue<T>).ToList();
	}

	#endregion
}
