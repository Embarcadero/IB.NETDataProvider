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
using System.Data;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using InterBaseSql.Data.Common;
using System.Linq;
using InterBaseSql.Data.Schema;

namespace InterBaseSql.Data.InterBaseClient
{
	internal class IBConnectionInternal : IDisposable
	{
		#region Fields

		private IDatabase _db;
		private IBTransaction _activeTransaction;
		private HashSet<IBCommand> _preparedCommands;
		private ConnectionString _options;
		private IBConnection _owningConnection;
		private bool _disposed;
		private IBEnlistmentNotification _enlistmentNotification;

		#endregion

		#region Properties

		public IDatabase Database
		{
			get { return _db; }
		}

		public bool HasActiveTransaction
		{
			get
			{
				return _activeTransaction != null && !_activeTransaction.IsCompleted;
			}
		}

		public IBTransaction ActiveTransaction
		{
			get { return _activeTransaction; }
		}

		public IBConnection OwningConnection
		{
			get { return _owningConnection; }
		}

		public bool IsEnlisted
		{
			get
			{
				return _enlistmentNotification != null && !_enlistmentNotification.IsCompleted;
			}
		}

		public ConnectionString Options
		{
			get { return _options; }
		}

		public bool CancelDisabled { get; set; }

		#endregion

		#region Constructors

		public IBConnectionInternal(ConnectionString options)
		{
			_preparedCommands = new HashSet<IBCommand>();

			_options = options;
		}

		#endregion

		#region IDisposable Methods

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Disconnect();
			}
		}

		#endregion

		#region Create and Drop database methods

		public void CreateDatabase(DatabaseParameterBuffer dpb)
		{
			var db = ClientFactory.CreateIDatabase(_options);
			try
			{
				db.CreateDatabase(dpb, _options.ComposeDatabase());
			}
			finally
			{
				db.Detach();
			}
		}

		public void DropDatabase()
		{
			var db = ClientFactory.CreateIDatabase(_options);
			try
			{
				db.Attach(BuildDpb(db, _options), _options.ComposeDatabase());
				db.DropDatabase();
			}
			finally
			{
				db.Detach();
			}
		}

		#endregion

		#region Connect and Disconnect methods

		public void Connect()
		{
			if (Charset.GetCharset(_options.Charset) == null)
			{
				throw new IBException("Invalid character set specified");
			}

			try
			{
				_db = ClientFactory.CreateIDatabase(_options);
				_db.Charset = Charset.GetCharset(_options.Charset);
				_db.Dialect = _options.Dialect;
				_db.PacketSize = _options.PacketSize;
				_db.TruncateChar = _options.TruncateChar;

				var dpb = BuildDpb(_db, _options);

				_db.Attach(dpb, _options.ComposeDatabase());
			}
			catch (IscException ex)
			{
				throw new IBException(ex.Message, ex);
			}
		}

		public void Disconnect()
		{
			if (_db != null)
			{
				try
				{
					_db.Detach();
				}
				catch
				{ }
				finally
				{
					_db = null;
					_owningConnection = null;
					_options = null;
				}
			}
		}

		#endregion

		#region Transaction Handling Methods

		public IBTransaction BeginTransaction(IsolationLevel level, string transactionName)
		{
			EnsureActiveTransaction();

			try
			{
				_activeTransaction = new IBTransaction(_owningConnection, level);
				_activeTransaction.BeginTransaction();

				if (transactionName != null)
				{
					_activeTransaction.Save(transactionName);
				}
			}
			catch (IscException ex)
			{
				DisposeTransaction();
				throw new IBException(ex.Message, ex);
			}

			return _activeTransaction;
		}

		public IBTransaction BeginTransaction(IBTransactionOptions options, string transactionName)
		{
			EnsureActiveTransaction();

			try
			{
				_activeTransaction = new IBTransaction(_owningConnection, IsolationLevel.Unspecified);
				_activeTransaction.BeginTransaction(options);

				if (transactionName != null)
				{
					_activeTransaction.Save(transactionName);
				}
			}
			catch (IscException ex)
			{
				DisposeTransaction();
				throw new IBException(ex.Message, ex);
			}

			return _activeTransaction;
		}

		public void DisposeTransaction()
		{
			if (_activeTransaction != null && !IsEnlisted)
			{
				_activeTransaction.Dispose();
				_activeTransaction = null;
			}
		}

		public void TransactionCompleted()
		{
			foreach (var command in _preparedCommands)
			{
				if (command.Transaction != null)
				{
					command.DisposeReader();
					command.Transaction = null;
				}
			}
		}

		#endregion

		#region Transaction Enlistement

		public void EnlistTransaction(System.Transactions.Transaction transaction)
		{
			if (_owningConnection != null)
			{
				if (_enlistmentNotification != null && _enlistmentNotification.SystemTransaction == transaction)
					return;

				if (HasActiveTransaction)
				{
					throw new ArgumentException("Unable to enlist in transaction, a local transaction already exists");
				}
				if (_enlistmentNotification != null)
				{
					throw new ArgumentException("Already enlisted in a transaction");
				}

				_enlistmentNotification = new IBEnlistmentNotification(this, transaction);
				_enlistmentNotification.Completed += new EventHandler(EnlistmentCompleted);
			}
		}

		private void EnlistmentCompleted(object sender, EventArgs e)
		{
			_enlistmentNotification = null;
		}

		public IBTransaction BeginTransaction(System.Transactions.IsolationLevel isolationLevel)
		{
			switch (isolationLevel)
			{
				case System.Transactions.IsolationLevel.Chaos:
					return BeginTransaction(System.Data.IsolationLevel.Chaos, null);

				case System.Transactions.IsolationLevel.ReadUncommitted:
					return BeginTransaction(System.Data.IsolationLevel.ReadUncommitted, null);

				case System.Transactions.IsolationLevel.RepeatableRead:
					return BeginTransaction(System.Data.IsolationLevel.RepeatableRead, null);

				case System.Transactions.IsolationLevel.Serializable:
					return BeginTransaction(System.Data.IsolationLevel.Serializable, null);

				case System.Transactions.IsolationLevel.Snapshot:
					return BeginTransaction(System.Data.IsolationLevel.Snapshot, null);

				case System.Transactions.IsolationLevel.Unspecified:
					return BeginTransaction(System.Data.IsolationLevel.Unspecified, null);

				case System.Transactions.IsolationLevel.ReadCommitted:
				default:
					return BeginTransaction(System.Data.IsolationLevel.ReadCommitted, null);
			}
		}

		#endregion

		#region Schema Methods

		public DataTable GetSchema(string collectionName, string[] restrictions)
		{
			return IBSchemaFactory.GetSchema(_owningConnection, collectionName, restrictions);
		}

		#endregion

		#region Prepared Commands Methods

		public void AddPreparedCommand(IBCommand command)
		{
			if (_preparedCommands.Contains(command))
				return;
			_preparedCommands.Add(command);
		}

		public void RemovePreparedCommand(IBCommand command)
		{
			_preparedCommands.Remove(command);
		}

		public void ReleasePreparedCommands()
		{
			// copy the data because the collection will be modified via RemovePreparedCommand from Release
			var data = _preparedCommands.ToList();
			foreach (var item in data)
			{
				try
				{
					item.Release();
				}
				catch (IOException)
				{
					// If an IO error occurs when trying to release the command
					// avoid it. (It maybe the connection to the server was down
					// for unknown reasons.)
				}
				catch (IscException ex) when (ex.ErrorCode == IscCodes.isc_network_error
					|| ex.ErrorCode == IscCodes.isc_net_read_err
					|| ex.ErrorCode == IscCodes.isc_net_write_err)
				{ }
			}
		}

		#endregion

		#region InterBase Events Methods

		public void CloseEventManager()
		{
			if (_db != null && _db.HasRemoteEventSupport)
			{
				_db.CloseEventManager();
			}
		}

		#endregion

		#region Private Methods

		private DatabaseParameterBuffer BuildDpb(IDatabase db, ConnectionString options)
		{
			var dpb = new DatabaseParameterBuffer();

			dpb.Append(IscCodes.isc_dpb_version1);
			dpb.Append(IscCodes.isc_dpb_dummy_packet_interval, new byte[] { 120, 10, 0, 0 });
			dpb.Append(IscCodes.isc_dpb_sql_dialect, new byte[] { options.Dialect, 0, 0, 0 });
			if (options.Charset.ToLower() != "none")
				dpb.Append(IscCodes.isc_dpb_lc_ctype, options.Charset);
			if (options.DbCachePages > 0)
				dpb.Append(IscCodes.isc_dpb_num_buffers, options.DbCachePages);
			if (!string.IsNullOrEmpty(options.UserID))
				dpb.Append(IscCodes.isc_dpb_user_name, options.UserID);
			if (!string.IsNullOrEmpty(options.Password))
				dpb.Append(IscCodes.isc_dpb_password, options.Password);
			if (!string.IsNullOrEmpty(options.Role))
				dpb.Append(IscCodes.isc_dpb_sql_role_name, options.Role);
			dpb.Append(IscCodes.isc_dpb_connect_timeout, options.ConnectionTimeout);
			if (!string.IsNullOrEmpty(options.InstanceName))
				dpb.Append(IscCodes.isc_dpb_instance_name, options.InstanceName);
			if (!string.IsNullOrEmpty(options.SEPPassword))
				dpb.Append(IscCodes.isc_dpb_sys_encrypt_password, options.SEPPassword);


			return dpb;
		}

		private string GetProcessName()
		{
			return GetSystemWebHostingPath() ?? GetRealProcessName() ?? string.Empty;
		}


		private static string GetSystemWebHostingPath()
		{
#if NETSTANDARD2_0 || NET5_0
			return null;
#else
			var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Equals("System.Web", StringComparison.Ordinal)).FirstOrDefault();
			if (assembly == null)
				return null;
			// showing ApplicationPhysicalPath may be wrong because of connection pooling
			// better idea?
			return (string)assembly.GetType("System.Web.Hosting.HostingEnvironment").GetProperty("ApplicationPhysicalPath").GetValue(null, null);
#endif
		}

		private static string GetRealProcessName()
		{
			string FromProcess()
			{
				try
				{
					return Process.GetCurrentProcess().MainModule.FileName;
				}
				catch (InvalidOperationException)
				{
					return null;
				}
			}
			return Assembly.GetEntryAssembly()?.Location ?? FromProcess();
		}

		private static int GetProcessId()
		{
			try
			{
				return Process.GetCurrentProcess().Id;
			}
			catch (InvalidOperationException)
			{
				return -1;
			}
		}

		private static string GetClientVersion()
		{
			return typeof(IBConnectionInternal).GetTypeInfo().Assembly.GetName().Version.ToString();
		}

		private void EnsureActiveTransaction()
		{
			if (HasActiveTransaction)
				throw new InvalidOperationException("A transaction is currently active. Parallel transactions are not supported.");
		}
		#endregion

		#region Infrastructure
		public IBConnectionInternal SetOwningConnection(IBConnection owningConnection)
		{
			_owningConnection = owningConnection;
			return this;
		}
		#endregion
	}
}
