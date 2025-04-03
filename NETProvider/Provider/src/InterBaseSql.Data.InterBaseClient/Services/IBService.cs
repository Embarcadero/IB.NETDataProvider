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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Services;

public abstract class IBService
{
	public event EventHandler<ServiceOutputEventArgs> ServiceOutput;

	public event EventHandler<IBInfoMessageEventArgs> InfoMessage;

	private const string ServiceName = "service_mgr";

	private ServiceManagerBase _svc;
	private ConnectionString _connectionStringOptions;

	private protected ServiceManagerBase Service => _svc;
	private protected ConnectionString ConnectionStringOptions => _connectionStringOptions;

	public IBServiceState State { get; private set; }
	public int QueryBufferSize { get; set; }

	private string _connectionString;
	public string ConnectionString
	{
		get { return _connectionString; }
		set
		{
			if (_svc != null && State == IBServiceState.Open)
			{
				throw new InvalidOperationException("ConnectionString cannot be modified on open instances.");
			}

			_connectionStringOptions = new ConnectionString(value);

			if (value == null)
			{
				_connectionString = string.Empty;
			}
			else
			{
				_connectionString = value;
			}
		}
	}

	private protected IBService(string connectionString = null)
	{
		State = IBServiceState.Closed;
		QueryBufferSize = IscCodes.DEFAULT_MAX_BUFFER_SIZE;
		ConnectionString = connectionString;
	}

	private ServiceParameterBufferBase BuildSpb()
	{
		var spb = Service.CreateServiceParameterBuffer();
		spb.AppendPreamble();
		spb.Append1(IscCodes.isc_spb_user_name, _connectionStringOptions.UserID);
		spb.Append1(IscCodes.isc_spb_password, _connectionStringOptions.Password);
		spb.Append1(IscCodes.isc_spb_dummy_packet_interval, new byte[] { 120, 10, 0, 0 });
		if ((_connectionStringOptions?.Role.Length ?? 0) != 0)
			spb.Append1(IscCodes.isc_spb_sql_role_name, _connectionStringOptions.Role);
		if ((_connectionStringOptions?.InstanceName.Length ?? 0) != 0)
			spb.Append1(IscCodes.isc_spb_instance_name, _connectionStringOptions.InstanceName);
		return spb;
	}

	private protected void Open()
	{
		if (State != IBServiceState.Closed)
			throw new InvalidOperationException("Service already open.");
		if (string.IsNullOrEmpty(_connectionStringOptions.UserID))
			throw new InvalidOperationException("No user name was specified.");
		if (string.IsNullOrEmpty(_connectionStringOptions.Password))
			throw new InvalidOperationException("No user password was specified.");
		if (!Charset.TryGetByName(_connectionStringOptions.Charset, out var charset))
			throw new ArgumentException("Invalid character set specified.");

		if (_svc == null)
		{
			_svc = ClientFactory.CreateServiceManager(_connectionStringOptions);
		}
		_svc.Attach(BuildSpb(), _connectionStringOptions.DataSource, _connectionStringOptions.Port, ServiceName);
		_svc.WarningMessage = OnWarningMessage;
		State = IBServiceState.Open;
	}
	private protected async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		if (State != IBServiceState.Closed)
			throw new InvalidOperationException("Service already open.");
		if (string.IsNullOrEmpty(_connectionStringOptions.UserID))
			throw new InvalidOperationException("No user name was specified.");
		if (string.IsNullOrEmpty(_connectionStringOptions.Password))
			throw new InvalidOperationException("No user password was specified.");
		if (!Charset.TryGetByName(_connectionStringOptions.Charset, out var charset))
			throw new ArgumentException("Invalid character set specified.");

		if (_svc == null)
		{
			_svc = ClientFactory.CreateServiceManager(_connectionStringOptions);
		}
		await _svc.AttachAsync(BuildSpb(), _connectionStringOptions.DataSource, _connectionStringOptions.Port, ServiceName, cancellationToken).ConfigureAwait(false);
		_svc.WarningMessage = OnWarningMessage;
		State = IBServiceState.Open;
	}

	private protected void Close()
	{
		if (State != IBServiceState.Open)
		{
			return;
		}
		_svc.Detach();
		_svc = null;
		State = IBServiceState.Closed;
	}
	private protected async Task CloseAsync(CancellationToken cancellationToken = default)
	{
		if (State != IBServiceState.Open)
		{
			return;
		}
		await _svc.DetachAsync(cancellationToken).ConfigureAwait(false);
		_svc = null;
		State = IBServiceState.Closed;
	}

	private protected void StartTask(ServiceParameterBufferBase spb)
	{
		if (State == IBServiceState.Closed)
			throw new InvalidOperationException("Service is closed.");

		Service.Start(spb);
	}
	private protected async Task StartTaskAsync(ServiceParameterBufferBase spb, CancellationToken cancellationToken = default)
	{
		if (State == IBServiceState.Closed)
			throw new InvalidOperationException("Service is closed.");

		await Service.StartAsync(spb, cancellationToken).ConfigureAwait(false);
	}

	private protected List<object> Query(byte[] items, ServiceParameterBufferBase spb)
	{
		var result = new List<object>();
		Query(items, spb, (truncated, item) =>
		{
			if (item is string stringItem)
			{
				if (!truncated)
				{
					result.Add(stringItem);
				}
				else
				{
					var lastValue = result[result.Count - 1] as string;
					result[result.Count - 1] = lastValue + stringItem;
				}
				return;
			}

			if (item is byte[] byteArrayItem)
			{
				if (!truncated)
				{
					result.Add(byteArrayItem);
				}
				else
				{
					var lastValue = result[result.Count - 1] as byte[];
					var lastValueLength = lastValue.Length;
					Array.Resize(ref lastValue, lastValue.Length + byteArrayItem.Length);
					Array.Copy(byteArrayItem, 0, lastValue, lastValueLength, byteArrayItem.Length);
				}
				return;
			}

			result.Add(item);
		});
		return result;
	}
	private protected async Task<List<object>> QueryAsync(byte[] items, ServiceParameterBufferBase spb, CancellationToken cancellationToken = default)
	{
		var result = new List<object>();
		await QueryAsync(items, spb, (truncated, item) =>
		{
			if (item is string stringItem)
			{
				if (!truncated)
				{
					result.Add(stringItem);
				}
				else
				{
					var lastValue = result[result.Count - 1] as string;
					result[result.Count - 1] = lastValue + stringItem;
				}
				return Task.CompletedTask;
			}

			if (item is byte[] byteArrayItem)
			{
				if (!truncated)
				{
					result.Add(byteArrayItem);
				}
				else
				{
					var lastValue = result[result.Count - 1] as byte[];
					var lastValueLength = lastValue.Length;
					Array.Resize(ref lastValue, lastValue.Length + byteArrayItem.Length);
					Array.Copy(byteArrayItem, 0, lastValue, lastValueLength, byteArrayItem.Length);
				}
				return Task.CompletedTask;
			}

			result.Add(item);

			return Task.CompletedTask;
		}, cancellationToken).ConfigureAwait(false);
		return result;
	}

	private protected void Query(byte[] items, ServiceParameterBufferBase spb, Action<bool, object> queryResponseAction)
	{
		var pos = 0;
		var truncated = false;
		var type = default(int);

		var buffer = QueryService(items, spb);

		while ((type = buffer[pos++]) != IscCodes.isc_info_end)
		{
			if (type == IscCodes.isc_info_truncated)
			{
				buffer = QueryService(items, spb);
				pos = 0;
				truncated = true;
				continue;
			}

			switch (type)
			{
				case IscCodes.isc_info_svc_version:
				case IscCodes.isc_info_svc_get_license_mask:
				case IscCodes.isc_info_svc_capabilities:
				case IscCodes.isc_info_svc_get_licensed_users:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						queryResponseAction(truncated, (int)IscHelper.VaxInteger(buffer, pos, 4));
						pos += length;
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_server_version:
				case IscCodes.isc_info_svc_implementation:
				case IscCodes.isc_info_svc_get_env:
				case IscCodes.isc_info_svc_get_env_lock:
				case IscCodes.isc_info_svc_get_env_msg:
				case IscCodes.isc_info_svc_user_dbpath:
				case IscCodes.isc_info_svc_line:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						queryResponseAction(truncated, Service.Charset.GetString(buffer, pos, length));
						pos += length;
						truncated = false;
						break;
					}
				case IscCodes.isc_info_svc_to_eof:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						var block = new byte[length];
						Array.Copy(buffer, pos, block, 0, length);
						queryResponseAction(truncated, block);
						pos += length;
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_svr_db_info:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						queryResponseAction(truncated, ParseDatabasesInfo(buffer, ref pos, Service.Charset));
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_get_users:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						queryResponseAction(truncated, ParseUserData(buffer, ref pos, Service.Charset));
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_get_config:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						queryResponseAction(truncated, ParseServerConfig(buffer, ref pos, Service.Charset));
						truncated = false;
						break;
					}

				case IscCodes.isc_info_data_not_ready:
					{
						queryResponseAction(truncated, typeof(void));
						truncated = false;
						break;
					}
			}
		}
	}
	private protected async Task QueryAsync(byte[] items, ServiceParameterBufferBase spb, Func<bool, object, Task> queryResponseAction, CancellationToken cancellationToken = default)
	{
		var pos = 0;
		var truncated = false;
		var type = default(int);

		var buffer = await QueryServiceAsync(items, spb, cancellationToken).ConfigureAwait(false);

		while ((type = buffer[pos++]) != IscCodes.isc_info_end)
		{
			if (type == IscCodes.isc_info_truncated)
			{
				buffer = await QueryServiceAsync(items, spb, cancellationToken).ConfigureAwait(false);
				pos = 0;
				truncated = true;
				continue;
			}

			switch (type)
			{
				case IscCodes.isc_info_svc_version:
				case IscCodes.isc_info_svc_get_license_mask:
				case IscCodes.isc_info_svc_capabilities:
				case IscCodes.isc_info_svc_get_licensed_users:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						await queryResponseAction(truncated, (int)IscHelper.VaxInteger(buffer, pos, 4)).ConfigureAwait(false);
						pos += length;
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_server_version:
				case IscCodes.isc_info_svc_implementation:
				case IscCodes.isc_info_svc_get_env:
				case IscCodes.isc_info_svc_get_env_lock:
				case IscCodes.isc_info_svc_get_env_msg:
				case IscCodes.isc_info_svc_user_dbpath:
				case IscCodes.isc_info_svc_line:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						await queryResponseAction(truncated, Service.Charset.GetString(buffer, pos, length)).ConfigureAwait(false);
						pos += length;
						truncated = false;
						break;
					}
				case IscCodes.isc_info_svc_to_eof:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						var block = new byte[length];
						Array.Copy(buffer, pos, block, 0, length);
						await queryResponseAction(truncated, block).ConfigureAwait(false);
						pos += length;
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_svr_db_info:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						await queryResponseAction(truncated, ParseDatabasesInfo(buffer, ref pos, Service.Charset)).ConfigureAwait(false);
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_get_users:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						await queryResponseAction(truncated, ParseUserData(buffer, ref pos, Service.Charset)).ConfigureAwait(false);
						truncated = false;
						break;
					}

				case IscCodes.isc_info_svc_get_config:
					{
						var length = GetLength(buffer, 2, ref pos);
						if (length == 0)
							continue;
						await queryResponseAction(truncated, ParseServerConfig(buffer, ref pos, Service.Charset)).ConfigureAwait(false);
						truncated = false;
						break;
					}

				case IscCodes.isc_info_data_not_ready:
					{
						await queryResponseAction(truncated, typeof(void)).ConfigureAwait(false);
						truncated = false;
						break;
					}
			}
		}
	}

	private protected void ProcessServiceOutput(ServiceParameterBufferBase spb)
	{
		string line;
		while ((line = GetNextLine(spb)) != null)
		{
			OnServiceOutput(line);
		}
	}
	private protected async Task ProcessServiceOutputAsync(ServiceParameterBufferBase spb, CancellationToken cancellationToken = default)
	{
		string line;
		while ((line = await GetNextLineAsync(spb, cancellationToken).ConfigureAwait(false)) != null)
		{
			OnServiceOutput(line);
		}
	}

	private protected string GetNextLine(ServiceParameterBufferBase spb)
	{
		var info = Query(new byte[] { IscCodes.isc_info_svc_line }, spb);
		if (info.Count == 0)
			return null;
		return info[0] as string;
	}
	private protected async Task<string> GetNextLineAsync(ServiceParameterBufferBase spb, CancellationToken cancellationToken = default)
	{
		var info = await QueryAsync(new byte[] { IscCodes.isc_info_svc_line }, spb, cancellationToken).ConfigureAwait(false);
		if (info.Count == 0)
			return null;
		return info[0] as string;
	}

	private protected void OnServiceOutput(string message)
	{
		ServiceOutput?.Invoke(this, new ServiceOutputEventArgs(message));
	}

	private protected void EnsureDatabase()
	{
		if (string.IsNullOrEmpty(ConnectionStringOptions.Database))
			throw IBException.Create("Action should be executed against a specific database.");
	}

	private byte[] QueryService(byte[] items, ServiceParameterBufferBase spb)
	{
		var buffer = new byte[QueryBufferSize];
		Service.Query(spb, items.Length, items, buffer.Length, buffer);
		return buffer;

	}
	private async Task<byte[]> QueryServiceAsync(byte[] items, ServiceParameterBufferBase spb, CancellationToken cancellationToken = default)
	{
		var buffer = new byte[QueryBufferSize];
		await Service.QueryAsync(spb, items.Length, items, buffer.Length, buffer, cancellationToken).ConfigureAwait(false);
		return buffer;
	}

	private void OnWarningMessage(IscException warning)
	{
		InfoMessage?.Invoke(this, new IBInfoMessageEventArgs(warning));
	}

	private static IBServerConfig ParseServerConfig(byte[] buffer, ref int pos, Charset charset)
	{
		var config = new IBServerConfig();

		pos = 1;
		while (buffer[pos] != IscCodes.isc_info_flag_end)
		{
			pos++;

			int key = buffer[pos - 1];
			var keyValue = (int)IscHelper.VaxInteger(buffer, pos, 4);

			pos += 4;

			switch (key)
			{
				case IscCodes.ISCCFG_LOCKMEM_KEY:
					config.LockMemSize = keyValue;
					break;

				case IscCodes.ISCCFG_LOCKSEM_KEY:
					config.LockSemCount = keyValue;
					break;

				case IscCodes.ISCCFG_LOCKSIG_KEY:
					config.LockSignal = keyValue;
					break;

				case IscCodes.ISCCFG_EVNTMEM_KEY:
					config.EventMemorySize = keyValue;
					break;

				case IscCodes.ISCCFG_PRIORITY_KEY:
					config.PrioritySwitchDelay = keyValue;
					break;

				case IscCodes.ISCCFG_MEMMIN_KEY:
					config.MinMemory = keyValue;
					break;

				case IscCodes.ISCCFG_MEMMAX_KEY:
					config.MaxMemory = keyValue;
					break;

				case IscCodes.ISCCFG_LOCKORDER_KEY:
					config.LockGrantOrder = keyValue;
					break;

				case IscCodes.ISCCFG_ANYLOCKMEM_KEY:
					config.AnyLockMemory = keyValue;
					break;

				case IscCodes.ISCCFG_ANYLOCKSEM_KEY:
					config.AnyLockSemaphore = keyValue;
					break;

				case IscCodes.ISCCFG_ANYLOCKSIG_KEY:
					config.AnyLockSignal = keyValue;
					break;

				case IscCodes.ISCCFG_ANYEVNTMEM_KEY:
					config.AnyEventMemory = keyValue;
					break;

				case IscCodes.ISCCFG_LOCKHASH_KEY:
					config.LockHashSlots = keyValue;
					break;

				case IscCodes.ISCCFG_DEADLOCK_KEY:
					config.DeadlockTimeout = keyValue;
					break;

				case IscCodes.ISCCFG_LOCKSPIN_KEY:
					config.LockRequireSpins = keyValue;
					break;

				case IscCodes.ISCCFG_CONN_TIMEOUT_KEY:
					config.ConnectionTimeout = keyValue;
					break;

				case IscCodes.ISCCFG_DUMMY_INTRVL_KEY:
					config.DummyPacketInterval = keyValue;
					break;

				case IscCodes.ISCCFG_IPCMAP_KEY:
					config.IpcMapSize = keyValue;
					break;

				case IscCodes.ISCCFG_DBCACHE_KEY:
					config.DefaultDbCachePages = keyValue;
					break;

				case IscCodes.ISCCFG_TRACE_POOLS_KEY:
					config.TracePools = keyValue;
					break;

				case IscCodes.ISCCFG_REMOTE_BUFFER_KEY:
					config.RemoteBuffer = keyValue;
					break;

				case IscCodes.ISCCFG_CPU_AFFINITY_KEY:
					config.CPUAffinity = keyValue;
					break;

				case IscCodes.ISCCFG_SWEEP_QUANTUM_KEY:
					config.SweepQuantum = keyValue;
					break;

				case IscCodes.ISCCFG_USER_QUANTUM_KEY:
					config.UserQuantum = keyValue;
					break;

				case IscCodes.ISCCFG_SLEEP_TIME_KEY:
					config.SleepTime = keyValue;
					break;

				case IscCodes.ISCCFG_MAX_THREADS_KEY:
					config.MaxThreads = keyValue;
					break;

				case IscCodes.ISCCFG_ADMIN_DB_KEY:
					config.AdminDB = keyValue;
					break;

				case IscCodes.ISCCFG_USE_SANCTUARY_KEY:
					config.UseSanctuary = keyValue;
					break;

				case IscCodes.ISCCFG_ENABLE_HT_KEY:
					config.EnableHT = keyValue;
					break;

				case IscCodes.ISCCFG_USE_ROUTER_KEY:
					config.UseRouter = keyValue;
					break;

				case IscCodes.ISCCFG_SORTMEM_BUFFER_SIZE_KEY:
					config.SortMemBufferSize = keyValue;
					break;

				case IscCodes.ISCCFG_SQL_CMP_RECURSION_KEY:
					config.SQLCmpRecursion = keyValue;
					break;

				case IscCodes.ISCCFG_SOL_BOUND_THREADS_KEY:
					config.SQLBoundThreads = keyValue;
					break;

				case IscCodes.ISCCFG_SOL_SYNC_SCOPE_KEY:
					config.SQLSyncScope = keyValue;
					break;

				case IscCodes.ISCCFG_IDX_RECNUM_MARKER_KEY:
					config.IdxRecnumMarker = keyValue;
					break;

				case IscCodes.ISCCFG_IDX_GARBAGE_COLLECTION_KEY:
					config.IdxGarbageCollection = keyValue;
					break;

				case IscCodes.ISCCFG_WIN_LOCAL_CONNECT_RETRIES_KEY:
					config.WinLocalConnectRetries = keyValue;
					break;

				case IscCodes.ISCCFG_EXPAND_MOUNTPOINT_KEY:
					config.ExpandMountpoint = keyValue;
					break;

				case IscCodes.ISCCFG_LOOPBACK_CONNECTION_KEY:
					config.LoopbackConnection = keyValue;
					break;

				case IscCodes.ISCCFG_THREAD_STACK_SIZE_KEY:
					config.ThreadStackSize = keyValue;
					break;

				case IscCodes.ISCCFG_MAX_DB_VIRMEM_USE_KEY:
					config.MaxDBVirmemUse = keyValue;
					break;

				case IscCodes.ISCCFG_MAX_ASSISTANTS_KEY:
					config.MaxAssistants = keyValue;
					break;

				case IscCodes.ISCCFG_APPDATA_DIR_KEY:
					config.AppdataDir = keyValue;
					break;

				case IscCodes.ISCCFG_MEMORY_RECLAMATION_KEY:
					config.MemoryReclamation = keyValue;
					break;

				case IscCodes.ISCCFG_PAGE_CACHE_EXPANSION_KEY:
					config.PageCacheExpansion = keyValue;
					break;

				case IscCodes.ISCCFG_STARTING_TRANSACTION_ID_KEY:
					config.StartingTransactionID = keyValue;
					break;

				case IscCodes.ISCCFG_DATABASE_ODS_VERSION_KEY:
					config.DatabaseODSVersion = keyValue;
					break;

				case IscCodes.ISCCFG_HOSTLIC_IMPORT_DIR_KEY:
					config.HostlicImportDir = keyValue;
					break;

				case IscCodes.ISCCFG_HOSTLIC_INFO_DIR_KEY:
					config.HostlicInfoDir = keyValue;
					break;

				case IscCodes.ISCCFG_ENABLE_PARTIAL_INDEX_SELECTIVITY_KEY:
					config.EnablePartialIndexSelectivity = keyValue;
					break;

				case IscCodes.ISCCFG_PREDICTIVE_IO_PAGES_KEY:
					config.PredictiveIOPages = keyValue;
					break;
			}
		}

		pos++;

		return config;
	}

	private static IBDatabasesInfo ParseDatabasesInfo(byte[] buffer, ref int pos, Charset charset)
	{
		var dbInfo = new IBDatabasesInfo();
		var type = 0;
		var length = 0;

		pos = 1;

		while ((type = buffer[pos++]) != IscCodes.isc_info_end)
		{
			switch (type)
			{
				case IscCodes.isc_spb_num_att:
					dbInfo.ConnectionCount = (int)IscHelper.VaxInteger(buffer, pos, 4);
					pos += 4;
					break;

				case IscCodes.isc_spb_num_db:
					pos += 4;
					break;

				case IscCodes.isc_spb_dbname:
					length = (int)IscHelper.VaxInteger(buffer, pos, 2);
					pos += 2;
					dbInfo.AddDatabase(charset.GetString(buffer, pos, length));
					pos += length;
					break;
			}
		}

		pos--;

		return dbInfo;
	}

	private static IBUserData[] ParseUserData(byte[] buffer, ref int pos, Charset charset)
	{
		var users = new List<IBUserData>();
		IBUserData currentUser = null;
		var type = 0;
		var length = 0;

		while ((type = buffer[pos++]) != IscCodes.isc_info_end)
		{
			switch (type)
			{
				case IscCodes.isc_spb_sec_username:
					{
						length = (int)IscHelper.VaxInteger(buffer, pos, 2);
						pos += 2;
						currentUser = new IBUserData();
						currentUser.UserName = charset.GetString(buffer, pos, length);
						pos += length;

						users.Add(currentUser);
					}
					break;

				case IscCodes.isc_spb_sec_firstname:
					length = (int)IscHelper.VaxInteger(buffer, pos, 2);
					pos += 2;
					currentUser.FirstName = charset.GetString(buffer, pos, length);
					pos += length;
					break;

				case IscCodes.isc_spb_sec_middlename:
					length = (int)IscHelper.VaxInteger(buffer, pos, 2);
					pos += 2;
					currentUser.MiddleName = charset.GetString(buffer, pos, length);
					pos += length;
					break;

				case IscCodes.isc_spb_sec_lastname:
					length = (int)IscHelper.VaxInteger(buffer, pos, 2);
					pos += 2;
					currentUser.LastName = charset.GetString(buffer, pos, length);
					pos += length;
					break;

				case IscCodes.isc_spb_sec_userid:
					currentUser.UserID = (int)IscHelper.VaxInteger(buffer, pos, 4);
					pos += 4;
					break;

				case IscCodes.isc_spb_sec_groupid:
					currentUser.GroupID = (int)IscHelper.VaxInteger(buffer, pos, 4);
					pos += 4;
					break;
			}
		}

		pos--;

		return users.ToArray();
	}

	private static int GetLength(byte[] buffer, int size, ref int pos)
	{
		var result = (int)IscHelper.VaxInteger(buffer, pos, size);
		pos += size;
		return result;
	}
}