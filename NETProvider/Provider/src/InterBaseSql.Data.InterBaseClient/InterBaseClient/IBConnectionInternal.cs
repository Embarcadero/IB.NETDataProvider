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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.Schema;

namespace InterBaseSql.Data.InterBaseClient;

internal class IBConnectionInternal
{
	#region Fields

	private DatabaseBase _db;
	private IBTransaction _activeTransaction;
	private HashSet<IIBPreparedCommand> _preparedCommands;
	private ConnectionString _connectionStringOptions;
	private IBConnection _owningConnection;
	private IBEnlistmentNotification _enlistmentNotification;

	#endregion

	#region Properties

	public DatabaseBase Database
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

	public ConnectionString ConnectionStringOptions
	{
		get { return _connectionStringOptions; }
	}

	public bool CancelDisabled { get; private set; }

	#endregion

	#region Constructors

	public IBConnectionInternal(ConnectionString options)
	{
		_preparedCommands = new HashSet<IIBPreparedCommand>();
		_connectionStringOptions = options;
	}

	#endregion

	#region Create and Drop database methods

	public void CreateDatabase(int pageSize, bool forcedWrites, bool overwrite)
	{
		var db = ClientFactory.CreateDatabase(_connectionStringOptions);
		var dpb = db.CreateDatabaseParameterBuffer();

		dpb.Append(IscCodes.isc_dpb_dummy_packet_interval, new byte[] { 120, 10, 0, 0 });
		dpb.Append(IscCodes.isc_dpb_sql_dialect, new byte[] { (byte)_connectionStringOptions.Dialect, 0, 0, 0 });
		if (!string.IsNullOrEmpty(_connectionStringOptions.UserID))
		{
			dpb.Append(IscCodes.isc_dpb_user_name, _connectionStringOptions.UserID);
		}
		if (!string.IsNullOrEmpty(_connectionStringOptions.Password))
		{
			dpb.Append(IscCodes.isc_dpb_password, _connectionStringOptions.Password);
		}
		dpb.Append(IscCodes.isc_dpb_force_write, (short)(forcedWrites ? 1 : 0));
		dpb.Append(IscCodes.isc_dpb_overwrite, (overwrite ? 1 : 0));
		if (pageSize > 0)
		{
			if (!SizeHelper.IsValidPageSize(pageSize))
				throw SizeHelper.InvalidSizeException("page size");
			dpb.Append(IscCodes.isc_dpb_page_size, pageSize);
		}

		try
		{
			db.CreateDatabase(dpb, _connectionStringOptions.Database);
		}
		finally
		{
			db.Detach();
		}
	}
	public async Task CreateDatabaseAsync(int pageSize, bool forcedWrites, bool overwrite, CancellationToken cancellationToken = default)
	{
		// Async version of this in Fb is for Managed mode which we do not support
		var db = ClientFactory.CreateDatabase(_connectionStringOptions);

		var dpb = db.CreateDatabaseParameterBuffer();

		dpb.Append(IscCodes.isc_dpb_dummy_packet_interval, new byte[] { 120, 10, 0, 0 });
		dpb.Append(IscCodes.isc_dpb_sql_dialect, new byte[] { (byte)_connectionStringOptions.Dialect, 0, 0, 0 });
		if (!string.IsNullOrEmpty(_connectionStringOptions.UserID))
		{
			dpb.Append(IscCodes.isc_dpb_user_name, _connectionStringOptions.UserID);
		}
		dpb.Append(IscCodes.isc_dpb_force_write, (short)(forcedWrites ? 1 : 0));
		dpb.Append(IscCodes.isc_dpb_overwrite, (overwrite ? 1 : 0));
		if (pageSize > 0)
		{
			if (!SizeHelper.IsValidPageSize(pageSize))
				throw SizeHelper.InvalidSizeException("page size");
			dpb.Append(IscCodes.isc_dpb_page_size, pageSize);
		}
		try
		{
			await db.CreateDatabaseAsync(dpb, _connectionStringOptions.Database, cancellationToken).ConfigureAwait(false);
		}
		finally
		{
			await db.DetachAsync(cancellationToken).ConfigureAwait(false);
		}
	}

	public void DropDatabase()
	{
		var db = ClientFactory.CreateDatabase(_connectionStringOptions);
		try
		{
			db.Attach(BuildDpb(db, _connectionStringOptions), _connectionStringOptions.Database);
			db.DropDatabase();
		}
		finally
		{
			db.Detach();
		}
	}
	public async Task DropDatabaseAsync(CancellationToken cancellationToken = default)
	{
		var db = ClientFactory.CreateDatabase(_connectionStringOptions);
		try
		{
			await db.AttachAsync(BuildDpb(db, _connectionStringOptions), _connectionStringOptions.Database, cancellationToken).ConfigureAwait(false);
			await db.DropDatabaseAsync(cancellationToken).ConfigureAwait(false);
		}
		finally
		{
			await db.DetachAsync(cancellationToken).ConfigureAwait(false);
		}
	}

	#endregion

	#region Connect and Disconnect methods

	public void Connect()
	{
		if (!Charset.TryGetByName(_connectionStringOptions.Charset, out var charset))
			throw new ArgumentException("Invalid character set specified.");

		try
		{
			_db = ClientFactory.CreateDatabase(_connectionStringOptions);
			var dpb = BuildDpb(_db, _connectionStringOptions);
			_db.Attach(dpb, _connectionStringOptions.Database);
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}
	public async Task ConnectAsync(CancellationToken cancellationToken = default)
	{
		if (Charset.GetCharset(_connectionStringOptions.Charset) == null)
		{
			throw IBException.Create("Invalid character set specified");
		}
		try
		{
			_db = ClientFactory.CreateDatabase(_connectionStringOptions);
			var dpb = BuildDpb(_db, _connectionStringOptions);
			await _db.AttachAsync(dpb, _connectionStringOptions.Database, cancellationToken).ConfigureAwait(false);
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
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
				_connectionStringOptions = null;
			}
		}
	}
	public async Task DisconnectAsync(CancellationToken cancellationToken = default)
	{
		if (_db != null)
		{
			try
			{
				await _db.DetachAsync(cancellationToken).ConfigureAwait(false);
			}
			catch
			{ }
			finally
			{
				_db = null;
				_owningConnection = null;
				_connectionStringOptions = null;
			}
		}
	}

	#endregion

	#region Transaction Handling Methods

	public IBTransaction BeginTransaction(IsolationLevel level, string transactionName)
	{
		EnsureNoActiveTransaction();

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
			throw IBException.Create(ex);
		}

		return _activeTransaction;
	}
	public async Task<IBTransaction> BeginTransactionAsync(IsolationLevel level, string transactionName, CancellationToken cancellationToken = default)
	{
		EnsureNoActiveTransaction();

		try
		{
			_activeTransaction = new IBTransaction(_owningConnection, level);
			await _activeTransaction.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

			if (transactionName != null)
			{
				_activeTransaction.Save(transactionName);
			}
		}
		catch (IscException ex)
		{
			await DisposeTransactionAsync(cancellationToken).ConfigureAwait(false);
			throw IBException.Create(ex);
		}

		return _activeTransaction;
	}

	public IBTransaction BeginTransaction(IBTransactionOptions options, string transactionName)
	{
		EnsureNoActiveTransaction();

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
			throw IBException.Create(ex);
		}

		return _activeTransaction;
	}
	public async Task<IBTransaction> BeginTransactionAsync(IBTransactionOptions options, string transactionName, CancellationToken cancellationToken = default)
	{
		EnsureNoActiveTransaction();

		try
		{
			_activeTransaction = new IBTransaction(_owningConnection, IsolationLevel.Unspecified);
			await _activeTransaction.BeginTransactionAsync(options, cancellationToken).ConfigureAwait(false);

			if (transactionName != null)
			{
				_activeTransaction.Save(transactionName);
			}
		}
		catch (IscException ex)
		{
			await DisposeTransactionAsync(cancellationToken).ConfigureAwait(false);
			throw IBException.Create(ex);
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
	public async Task DisposeTransactionAsync(CancellationToken cancellationToken = default)
	{
		if (_activeTransaction != null && !IsEnlisted)
		{
#if NET48 || NETSTANDARD2_0
			_activeTransaction.Dispose();
			await Task.CompletedTask.ConfigureAwait(false);
#else
			await _activeTransaction.DisposeAsync().ConfigureAwait(false);
#endif
			_activeTransaction = null;
		}
	}

	public void TransactionCompleted()
	{
		foreach (var command in _preparedCommands)
		{
			command.TransactionCompleted();
		}
	}
	public async Task TransactionCompletedAsync(CancellationToken cancellationToken = default)
	{
		foreach (var command in _preparedCommands)
		{
			await command.TransactionCompletedAsync(cancellationToken).ConfigureAwait(false);
		}
	}

	#endregion

	#region Transaction Enlistment

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
		var il = isolationLevel switch
		{
			System.Transactions.IsolationLevel.Chaos => IsolationLevel.Chaos,
			System.Transactions.IsolationLevel.ReadUncommitted => IsolationLevel.ReadUncommitted,
			System.Transactions.IsolationLevel.RepeatableRead => IsolationLevel.RepeatableRead,
			System.Transactions.IsolationLevel.Serializable => IsolationLevel.Serializable,
			System.Transactions.IsolationLevel.Snapshot => IsolationLevel.Snapshot,
			System.Transactions.IsolationLevel.Unspecified => IsolationLevel.Unspecified,
			_ => IsolationLevel.ReadCommitted,
		};
		return BeginTransaction(il, null);
	}
	public Task<IBTransaction> BeginTransactionAsync(System.Transactions.IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
	{
		var il = isolationLevel switch
		{
			System.Transactions.IsolationLevel.Chaos => IsolationLevel.Chaos,
			System.Transactions.IsolationLevel.ReadUncommitted => IsolationLevel.ReadUncommitted,
			System.Transactions.IsolationLevel.RepeatableRead => IsolationLevel.RepeatableRead,
			System.Transactions.IsolationLevel.Serializable => IsolationLevel.Serializable,
			System.Transactions.IsolationLevel.Snapshot => IsolationLevel.Snapshot,
			System.Transactions.IsolationLevel.Unspecified => IsolationLevel.Unspecified,
			_ => IsolationLevel.ReadCommitted,
		};
		return BeginTransactionAsync(il, null, cancellationToken);
	}

	#endregion

	#region Schema Methods

	public DataTable GetSchema(string collectionName, string[] restrictions)
	{
		return IBSchemaFactory.GetSchema(_owningConnection, collectionName, restrictions);
	}
	public Task<DataTable> GetSchemaAsync(string collectionName, string[] restrictions, CancellationToken cancellationToken = default)
	{
		return IBSchemaFactory.GetSchemaAsync(_owningConnection, collectionName, restrictions, cancellationToken);
	}

	#endregion

	#region Prepared Commands Methods

	public void AddPreparedCommand(IIBPreparedCommand command)
	{
		if (_preparedCommands.Contains(command))
			return;
		_preparedCommands.Add(command);
	}

	public void RemovePreparedCommand(IIBPreparedCommand command)
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
	public async Task ReleasePreparedCommandsAsync(CancellationToken cancellationToken = default)
	{
		// copy the data because the collection will be modified via RemovePreparedCommand from Release
		var data = _preparedCommands.ToList();
		foreach (var item in data)
		{
			try
			{
				await item.ReleaseAsync(cancellationToken).ConfigureAwait(false);
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
	public Task CloseEventManagerAsync(CancellationToken cancellationToken = default)
	{
		if (_db != null && _db.HasRemoteEventSupport)
		{
			return _db.CloseEventManagerAsync(cancellationToken).AsTask();
		}
		return Task.CompletedTask;
	}

	#endregion

	#region Private Methods

	private void EnsureNoActiveTransaction()
	{
		if (HasActiveTransaction)
			throw new InvalidOperationException("A transaction is currently active. Parallel transactions are not supported.");
	}

	private static DatabaseParameterBuffer BuildDpb(DatabaseBase db, ConnectionString options)
	{
		var dpb = db.CreateDatabaseParameterBuffer();

		dpb.Append(IscCodes.isc_dpb_dummy_packet_interval, new byte[] { 120, 10, 0, 0 });
		dpb.Append(IscCodes.isc_dpb_sql_dialect, new byte[] { (byte)options.Dialect, 0, 0, 0 });
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

	#endregion

	#region Infrastructure

	public IBConnectionInternal SetOwningConnection(IBConnection owningConnection)
	{
		_owningConnection = owningConnection;
		return this;
	}

	#endregion
}