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
using System.Collections.Generic;
using System.Text;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Services
{
	public abstract class IBService
	{
		public event EventHandler<ServiceOutputEventArgs> ServiceOutput;

		public event EventHandler<IBInfoMessageEventArgs> InfoMessage;

		private const string ServiceName = "service_mgr";

		private protected static readonly ServiceParameterBuffer EmptySpb = new ServiceParameterBuffer();

		private IServiceManager _svc;
		private ConnectionString _options;

		private protected Encoding SpbFilenameEncoding;

		private protected string Database => _options.Database;


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

				_options = new ConnectionString(value);

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

		private ServiceParameterBuffer BuildSpb()
		{
			SpbFilenameEncoding = Encoding.Default;
			var spb = new ServiceParameterBuffer();
			spb.Append(IscCodes.isc_spb_version);
			spb.Append(IscCodes.isc_spb_current_version);
			spb.Append((byte)IscCodes.isc_spb_user_name, _options.UserID);
			spb.Append((byte)IscCodes.isc_spb_password, _options.Password);
			spb.Append((byte)IscCodes.isc_spb_dummy_packet_interval, new byte[] { 120, 10, 0, 0 });
			if ((_options?.Role.Length ?? 0) != 0)
				spb.Append((byte)IscCodes.isc_spb_sql_role_name, _options.Role);
			return spb;
		}

		private protected void Open()
		{
			if (State != IBServiceState.Closed)
				throw new InvalidOperationException("Service already Open.");
			if (string.IsNullOrEmpty(_options.UserID))
				throw new InvalidOperationException("No user name was specified.");
			if (string.IsNullOrEmpty(_options.Password))
				throw new InvalidOperationException("No user password was specified.");

			try
			{
				if (_svc == null)
				{
					_svc = ClientFactory.CreateIServiceManager(_options);
				}
				_svc.Attach(BuildSpb(), _options.DataSource, _options.Port, ServiceName);
				_svc.WarningMessage = OnWarningMessage;
				State = IBServiceState.Open;
			}
			catch (Exception ex)
			{
				throw new IBException(ex.Message, ex);
			}
		}

		private protected void Close()
		{
			if (State != IBServiceState.Open)
			{
				return;
			}
			try
			{
				_svc.Detach();
				_svc = null;
				State = IBServiceState.Closed;
			}
			catch (Exception ex)
			{
				throw new IBException(ex.Message, ex);
			}
		}

		private protected void StartTask(ServiceParameterBuffer spb)
		{
			if (State == IBServiceState.Closed)
				throw new InvalidOperationException("Service is Closed.");

			try
			{
				_svc.Start(spb);
			}
			catch (Exception ex)
			{
				throw new IBException(ex.Message, ex);
			}
		}

		private protected IList<object> Query(byte[] items, ServiceParameterBuffer spb)
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

		private protected void Query(byte[] items, ServiceParameterBuffer spb, Action<bool, object> resultAction)
		{
			ProcessQuery(items, spb, resultAction);
		}

		private protected void ProcessServiceOutput(ServiceParameterBuffer spb)
		{
			string line;
			string pline;
			while ((line = GetNextLine(spb)) != null)
			{
				pline = line;
				OnServiceOutput(line);
			}
		}

		private protected string GetNextLine(ServiceParameterBuffer spb)
		{
			var info = Query(new byte[] { IscCodes.isc_info_svc_line }, spb);
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
			if (string.IsNullOrEmpty(Database))
				throw new IBException("Action should be executed against a specific database.");
		}

		private void ProcessQuery(byte[] items, ServiceParameterBuffer spb, Action<bool, object> queryResponseAction)
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
							queryResponseAction(truncated, Encoding.Default.GetString(buffer, pos, length));
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
							queryResponseAction(truncated, ParseDatabasesInfo(buffer, ref pos));
							truncated = false;
							break;
						}

					case IscCodes.isc_info_svc_get_users:
						{
							var length = GetLength(buffer, 2, ref pos);
							if (length == 0)
								continue;
							queryResponseAction(truncated, ParseUserData(buffer, ref pos));
							truncated = false;
							break;
						}

					case IscCodes.isc_info_svc_get_config:
						{
							var length = GetLength(buffer, 2, ref pos);
							if (length == 0)
								continue;
							queryResponseAction(truncated, ParseServerConfig(buffer, ref pos));
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

		private byte[] QueryService(byte[] items, ServiceParameterBuffer spb)
		{
			var shouldClose = false;
			if (State == IBServiceState.Closed)
			{
				Open();
				shouldClose = true;
			}
			try
			{
				var buffer = new byte[QueryBufferSize];
				_svc.Query(spb, items.Length, items, buffer.Length, buffer);
				return buffer;
			}
			finally
			{
				if (shouldClose)
				{
					Close();
				}
			}
		}

		private void OnWarningMessage(IscException warning)
		{
			InfoMessage?.Invoke(this, new IBInfoMessageEventArgs(warning));
		}

		private static IBServerConfig ParseServerConfig(byte[] buffer, ref int pos)
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

		private static IBDatabasesInfo ParseDatabasesInfo(byte[] buffer, ref int pos)
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
						dbInfo.AddDatabase(Encoding.Default.GetString(buffer, pos, length));
						pos += length;
						break;
				}
			}

			pos--;

			return dbInfo;
		}

		private static IBUserData[] ParseUserData(byte[] buffer, ref int pos)
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
							currentUser.UserName = Encoding.Default.GetString(buffer, pos, length);
							pos += length;

							users.Add(currentUser);
						}
						break;

					case IscCodes.isc_spb_sec_firstname:
						length = (int)IscHelper.VaxInteger(buffer, pos, 2);
						pos += 2;
						currentUser.FirstName = Encoding.Default.GetString(buffer, pos, length);
						pos += length;
						break;

					case IscCodes.isc_spb_sec_middlename:
						length = (int)IscHelper.VaxInteger(buffer, pos, 2);
						pos += 2;
						currentUser.MiddleName = Encoding.Default.GetString(buffer, pos, length);
						pos += length;
						break;

					case IscCodes.isc_spb_sec_lastname:
						length = (int)IscHelper.VaxInteger(buffer, pos, 2);
						pos += 2;
						currentUser.LastName = Encoding.Default.GetString(buffer, pos, length);
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
}
