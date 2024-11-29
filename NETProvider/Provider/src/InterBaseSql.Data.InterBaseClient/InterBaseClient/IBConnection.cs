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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.Logging;

namespace InterBaseSql.Data.InterBaseClient;

[DefaultEvent("InfoMessage")]
public sealed class IBConnection : DbConnection, ICloneable
{
	static readonly IIBLogger Log = IBLogManager.CreateLogger(nameof(IBConnection));

	#region Static Pool Handling Methods

	public static void ClearAllPools()
	{
		IBConnectionPoolManager.Instance.ClearAllPools();
	}

	public static void ClearPool(IBConnection connection)
	{
		if (connection == null)
			throw new ArgumentNullException(nameof(connection));

		IBConnectionPoolManager.Instance.ClearPool(connection.ConnectionOptions);
	}

	public static void ClearPool(string connectionString)
	{
		if (connectionString == null)
			throw new ArgumentNullException(nameof(connectionString));

		IBConnectionPoolManager.Instance.ClearPool(new ConnectionString(connectionString));
	}

	#endregion

	#region Static Database Creation/Drop methods

	public static void CreateDatabase(string connectionString, int pageSize = 4096, bool forcedWrites = true, bool overwrite = false)
	{
		var options = new ConnectionString(connectionString);
		options.Validate();

		try
		{
			var db = new IBConnectionInternal(options);
			try
			{
				db.CreateDatabase(pageSize, forcedWrites, overwrite);
				if (options.Charset.Length > 0)
				{
					var aDb = new IBConnection();
					aDb.ConnectionString = connectionString;
					aDb.Open();
					if (Charset.TryGetByName(options.Charset, out var charset))
					{
						var sDefaultCharset = $"UPDATE RDB$DATABASE SET RDB$CHARACTER_SET_NAME='{charset.Name}'";
						var trans = aDb.BeginTransaction();
						var cmd = new IBCommand(sDefaultCharset, aDb, trans);
						cmd.ExecuteNonQuery();
						trans.Commit();
						aDb.Close();
					}
				}
			}
			finally
			{
				db.Disconnect();
			}
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}
	public static async Task CreateDatabaseAsync(string connectionString, int pageSize = 4096, bool forcedWrites = true, bool overwrite = false, CancellationToken cancellationToken = default)
	{
		var options = new ConnectionString(connectionString);
		options.Validate();

		try
		{
			var db = new IBConnectionInternal(options);
			try
			{
				await db.CreateDatabaseAsync(pageSize, forcedWrites, overwrite, cancellationToken).ConfigureAwait(false);
				if (options.Charset.Length > 0)
				{
					var aDb = new IBConnection();
					aDb.ConnectionString = connectionString;
					await aDb.OpenAsync(cancellationToken).ConfigureAwait(false);

					if (Charset.TryGetByName(options.Charset, out var charset))
					{
						var sDefaultCharset = $"UPDATE RDB$DATABASE SET RDB$CHARACTER_SET_NAME='{charset.Name}'";
						var trans = aDb.BeginTransaction();
						var cmd = new IBCommand(sDefaultCharset, aDb, trans);
						await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
						await trans.CommitAsync(cancellationToken).ConfigureAwait(false);
						await aDb.CloseAsync().ConfigureAwait(false);
					}
				}
			}
			finally
			{
				await db.DisconnectAsync(cancellationToken).ConfigureAwait(false);
			}
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}

	public static void DropDatabase(string connectionString)
	{
		var options = new ConnectionString(connectionString);
		options.Validate();

		try
		{
			var db = new IBConnectionInternal(options);
			try
			{
				db.DropDatabase();
			}
			finally
			{
				db.Disconnect();
			}
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}
	public static async Task DropDatabaseAsync(string connectionString, CancellationToken cancellationToken = default)
	{
		var options = new ConnectionString(connectionString);
		options.Validate();

		try
		{
			var db = new IBConnectionInternal(options);
			try
			{
				await db.DropDatabaseAsync(cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				await db.DisconnectAsync(cancellationToken).ConfigureAwait(false);
			}
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}

	#endregion

	#region Events

	public override event StateChangeEventHandler StateChange;

	public event EventHandler<IBInfoMessageEventArgs> InfoMessage;

	public delegate void DialectDowngradeWarningHandler(object sender, int dbDialect);

	public event DialectDowngradeWarningHandler DialectDowngradeWarning;


	#endregion

	#region Fields

	private IBConnectionInternal _innerConnection;
	private ConnectionState _state;
	private ConnectionString _options;
	private bool _disposed;
	private string _connectionString;
	private short _dbSQLDialect;

	#endregion

	internal DatabaseBase IBDatabase => _innerConnection.Database;
	public short DBSQLDialect { get { return _dbSQLDialect; } }

	#region Properties

	[Category("Data")]
	[SettingsBindable(true)]
	[RefreshProperties(RefreshProperties.All)]
	[DefaultValue("")]
	public override string ConnectionString
	{
		get { return _connectionString; }
		set
		{
			if (_state == ConnectionState.Closed)
			{
				if (value == null)
				{
					value = string.Empty;
				}

				_options = new ConnectionString(value);
				_options.Validate();
				_connectionString = value;
			}
		}
	}

	[Category("Data")]
	[SettingsBindable(true)]
	[RefreshProperties(RefreshProperties.All)]
	[DefaultValue(false)]
	public bool TruncateChar
	{
		get { return _options.TruncateChar; }
		set
		{
			if (_options.TruncateChar != value)
			{
				if (!IsClosed)
				{
					throw new InvalidOperationException("Changing TruncateChar requires a closed connection as it modifies the connectionstring.");
				}
				var newOptions = new IBConnectionStringBuilder(_connectionString);
				newOptions.TruncateChar = value;
				ConnectionString = newOptions.ToString();
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override int ConnectionTimeout
	{
		get { return _options.ConnectionTimeout; }
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string Database
	{
		get { return _options.Database; }
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string DataSource
	{
		get { return _options.DataSource; }
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string ServerVersion
	{
		get
		{
			if (_state == ConnectionState.Closed)
			{
				throw new InvalidOperationException("The connection is closed.");
			}

			if (_innerConnection != null)
			{
				return _innerConnection.Database.ServerVersion;
			}

			return string.Empty;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override ConnectionState State
	{
		get { return _state; }
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int PacketSize
	{
		get { return _options.PacketSize; }
	}

	#endregion

	#region Internal Properties

	internal IBConnectionInternal InnerConnection
	{
		get { return _innerConnection; }
	}

	internal ConnectionString ConnectionOptions
	{
		get { return _options; }
	}

	internal bool IsClosed
	{
		get { return _state == ConnectionState.Closed; }
	}

	#endregion

	#region Protected Properties

	protected override DbProviderFactory DbProviderFactory
	{
		get { return InterBaseClientFactory.Instance; }
	}

	#endregion

	#region Constructors

	public IBConnection()
	{
		_options = new ConnectionString();
		_state = ConnectionState.Closed;
		_connectionString = string.Empty;
	}

	public IBConnection(string connectionString)
		: this()
	{
		if (!string.IsNullOrEmpty(connectionString))
		{
			ConnectionString = connectionString;
		}
	}

	#endregion

	#region IDisposable, IAsyncDisposable methods

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (!_disposed)
			{
				_disposed = true;
				Close();
				_innerConnection = null;
				_options = null;
				_connectionString = null;
			}
		}
		base.Dispose(disposing);
	}
#if !(NET48 || NETSTANDARD2_0)
	public override async ValueTask DisposeAsync()
	{
		if (!_disposed)
		{
			_disposed = true;
			await CloseAsync().ConfigureAwait(false);
			_innerConnection = null;
			_options = null;
			_connectionString = null;
		}
		await base.DisposeAsync().ConfigureAwait(false);
	}
#endif

	#endregion

	#region ICloneable Methods

	object ICloneable.Clone()
	{
		return new IBConnection(ConnectionString);
	}

	#endregion

	#region Transaction Handling Methods

	public new IBTransaction BeginTransaction() => BeginTransaction(IBTransaction.DefaultIsolationLevel, null);
#if NET48 || NETSTANDARD2_0
	public Task<IBTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
#else
	public new Task<IBTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
#endif
		=> BeginTransactionAsync(IBTransaction.DefaultIsolationLevel, null, cancellationToken);

	public new IBTransaction BeginTransaction(IsolationLevel level) => BeginTransaction(level, null);
#if NET48 || NETSTANDARD2_0
	public Task<IBTransaction> BeginTransactionAsync(IsolationLevel level, CancellationToken cancellationToken = default)
#else
	public new Task<IBTransaction> BeginTransactionAsync(IsolationLevel level, CancellationToken cancellationToken = default)
#endif
		=> BeginTransactionAsync(level, null, cancellationToken);

	public IBTransaction BeginTransaction(string transactionName) => BeginTransaction(IBTransaction.DefaultIsolationLevel, transactionName);
	public Task<IBTransaction> BeginTransactionAsync(string transactionName, CancellationToken cancellationToken = default) => BeginTransactionAsync(IBTransaction.DefaultIsolationLevel, transactionName, cancellationToken);

	public IBTransaction BeginTransaction(IsolationLevel level, string transactionName)
	{
		CheckClosed();

		return _innerConnection.BeginTransaction(level, transactionName);
	}
	public Task<IBTransaction> BeginTransactionAsync(IsolationLevel level, string transactionName, CancellationToken cancellationToken = default)
	{
		CheckClosed();

		return _innerConnection.BeginTransactionAsync(level, transactionName, cancellationToken);
	}

	public IBTransaction BeginTransaction(IBTransactionOptions options) => BeginTransaction(options, null);
	public Task<IBTransaction> BeginTransactionAsync(IBTransactionOptions options, CancellationToken cancellationToken = default) => BeginTransactionAsync(options, null, cancellationToken);

	public IBTransaction BeginTransaction(IBTransactionOptions options, string transactionName)
	{
		CheckClosed();

		return _innerConnection.BeginTransaction(options, transactionName);
	}
	public Task<IBTransaction> BeginTransactionAsync(IBTransactionOptions options, string transactionName, CancellationToken cancellationToken = default)
	{
		CheckClosed();

		return _innerConnection.BeginTransactionAsync(options, transactionName, cancellationToken);
	}

	protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);
#if !(NET48 || NETSTANDARD2_0)
	protected override async ValueTask<DbTransaction> BeginDbTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken) => await BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
#endif

	#endregion

	#region Transaction Enlistement

	public override void EnlistTransaction(System.Transactions.Transaction transaction)
	{
		CheckClosed();

		if (transaction == null)
			return;

		_innerConnection.EnlistTransaction(transaction);
	}

	#endregion

	#region Database Schema Methods

	public override DataTable GetSchema()
	{
		return GetSchema("MetaDataCollections");
	}
#if NET48 || NETSTANDARD2_0 || NETSTANDARD2_1
	public Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default)
#else
	public override Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default)
#endif
	{
		return GetSchemaAsync("MetaDataCollections", cancellationToken);
	}

	public override DataTable GetSchema(string collectionName)
	{
		return GetSchema(collectionName, null);
	}
#if NET48 || NETSTANDARD2_0 || NETSTANDARD2_1
	public Task<DataTable> GetSchemaAsync(string collectionName, CancellationToken cancellationToken = default)
#else
	public override Task<DataTable> GetSchemaAsync(string collectionName, CancellationToken cancellationToken = default)
#endif
	{
		return GetSchemaAsync(collectionName, null, cancellationToken);
	}
	public override DataTable GetSchema(string collectionName, string[] restrictions)
	{
		CheckClosed();

		if (!IBDBXLegacyTypes.IncludeLegacySchemaType)
			return _innerConnection.GetSchema(collectionName, restrictions);
		else
			return IBDBXLegacyTypes.UpdateColumnNames( _innerConnection.GetSchema(collectionName, restrictions));
	}
#if NET48 || NETSTANDARD2_0 || NETSTANDARD2_1
	public Task<DataTable> GetSchemaAsync(string collectionName, string[] restrictions, CancellationToken cancellationToken = default)
#else
	public override Task<DataTable> GetSchemaAsync(string collectionName, string[] restrictions, CancellationToken cancellationToken = default)
#endif
	{
		CheckClosed();

		if (!IBDBXLegacyTypes.IncludeLegacySchemaType)
		    return _innerConnection.GetSchemaAsync(collectionName, restrictions, cancellationToken);
		else
		{
			var dt = _innerConnection.GetSchemaAsync(collectionName, restrictions).Result;
			return Task.FromResult<DataTable>(IBDBXLegacyTypes.UpdateColumnNames(dt));
		}
	}

	#endregion

	#region Methods

	public new IBCommand CreateCommand()
	{
		return (IBCommand)CreateDbCommand();
	}

	protected override DbCommand CreateDbCommand()
	{
		return new IBCommand(null, this);
	}

#if NET6_0_OR_GREATER
	public new DbBatch CreateBatch()
	{
		throw new NotSupportedException("DbBatch is currently not supported. Use IBBatchCommand instead.");
	}

	protected override DbBatch CreateDbBatch()
	{
		return CreateBatch();
	}
#endif

	public override void ChangeDatabase(string databaseName)
	{
		CheckClosed();

		if (string.IsNullOrEmpty(databaseName))
		{
			throw new InvalidOperationException("Database name is not valid.");
		}

		var oldConnectionString = _connectionString;
		try
		{
			var csb = new IBConnectionStringBuilder(_connectionString);

			/* Close current connection	*/
			Close();

			/* Set up the new Database	*/
			csb.Database = databaseName;
			ConnectionString = csb.ToString();

			/* Open	new	connection	*/
			Open();
		}
		catch (IscException ex)
		{
			ConnectionString = oldConnectionString;
			throw IBException.Create(ex);
		}
	}
#if NET48 || NETSTANDARD2_0
	public async Task ChangeDatabaseAsync(string db, CancellationToken cancellationToken = default)
#else
	public override async Task ChangeDatabaseAsync(string db, CancellationToken cancellationToken = default)
#endif
	{
		CheckClosed();

		if (string.IsNullOrEmpty(db))
		{
			throw new InvalidOperationException("Database name is not valid.");
		}

		var oldConnectionString = _connectionString;
		try
		{
			var csb = new IBConnectionStringBuilder(_connectionString);

			/* Close current connection	*/
			await CloseAsync().ConfigureAwait(false);

			/* Set up the new Database	*/
			csb.Database = db;
			ConnectionString = csb.ToString();

			/* Open	new	connection	*/
			await OpenAsync(cancellationToken).ConfigureAwait(false);
		}
		catch (IscException ex)
		{
			ConnectionString = oldConnectionString;
			throw IBException.Create(ex);
		}
	}

	private short GetDBSQLDialect()
	{
		var DatabaseInfo = new IBDatabaseInfo(this);
		return (short)DatabaseInfo.GetDBSQLDialect();
	}

	void ValidateClientSQLDialect()
	{
		if (_dbSQLDialect < _innerConnection.Database.Dialect)
		{

			_innerConnection.Database.Dialect = _dbSQLDialect;
			DialectDowngradeWarning?.Invoke(this, _dbSQLDialect);
		}
	}

	public override void Open()
	{
		if (string.IsNullOrEmpty(_connectionString))
		{
			throw new InvalidOperationException("Connection String is not initialized.");
		}
		if (!IsClosed && _state != ConnectionState.Connecting)
		{
			throw new InvalidOperationException("Connection already Open.");
		}

		try
		{
			OnStateChange(_state, ConnectionState.Connecting);

			var createdNew = default(bool);
			if (_options.Pooling)
			{
				_innerConnection = IBConnectionPoolManager.Instance.Get(_options, out createdNew);
			}
			else
			{
				_innerConnection = new IBConnectionInternal(_options);
				createdNew = true;
			}
			if (createdNew)
			{
				try
				{
					_innerConnection.Connect();
				}
				catch (OperationCanceledException ex)
				{
					//cancellationToken.ThrowIfCancellationRequested();
					throw new TimeoutException("Timeout while connecting.", ex);
				}
				catch
				{
					if (_options.Pooling)
					{
						IBConnectionPoolManager.Instance.Release(_innerConnection, false);
					}
					throw;
				}
			}
			_innerConnection.SetOwningConnection(this);

			if (_options.Enlist)
			{
				try
				{
					EnlistTransaction(System.Transactions.Transaction.Current);
				}
				catch
				{
					// if enlistment fails clean up innerConnection
					_innerConnection.DisposeTransaction();

					if (_options.Pooling)
					{
						IBConnectionPoolManager.Instance.Release(_innerConnection, true);
					}
					else
					{
						_innerConnection.Disconnect();
						_innerConnection = null;
					}

					throw;
				}
			}

			// Bind	Warning	messages event
			_innerConnection.Database.WarningMessage = OnWarningMessage;

			// Update the connection state
			OnStateChange(_state, ConnectionState.Open);
			_dbSQLDialect = GetDBSQLDialect();
			ValidateClientSQLDialect();
		}
		catch (IscException ex)
		{
			OnStateChange(_state, ConnectionState.Closed);
			throw IBException.Create(ex);
		}
		catch
		{
			OnStateChange(_state, ConnectionState.Closed);
			throw;
		}
	}
	public override async Task OpenAsync(CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(_connectionString))
		{
			throw new InvalidOperationException("Connection String is not initialized.");
		}
		if (!IsClosed && _state != ConnectionState.Connecting)
		{
			throw new InvalidOperationException("Connection already Open.");
		}

		try
		{
			OnStateChange(_state, ConnectionState.Connecting);

			var createdNew = default(bool);
			if (_options.Pooling)
			{
				_innerConnection = IBConnectionPoolManager.Instance.Get(_options, out createdNew);
			}
			else
			{
				_innerConnection = new IBConnectionInternal(_options);
				createdNew = true;
			}
			if (createdNew)
			{
				try
				{
					await _innerConnection.ConnectAsync(cancellationToken).ConfigureAwait(false);
				}
				catch (OperationCanceledException ex)
				{
					cancellationToken.ThrowIfCancellationRequested();
					throw new TimeoutException("Timeout while connecting.", ex);
				}
				catch
				{
					if (_options.Pooling)
					{
						IBConnectionPoolManager.Instance.Release(_innerConnection, false);
					}
					throw;
				}
			}
			_innerConnection.SetOwningConnection(this);

			if (_options.Enlist)
			{
				try
				{
					EnlistTransaction(System.Transactions.Transaction.Current);
				}
				catch
				{
					// if enlistment fails clean up innerConnection
					await _innerConnection.DisposeTransactionAsync(cancellationToken).ConfigureAwait(false);

					if (_options.Pooling)
					{
						IBConnectionPoolManager.Instance.Release(_innerConnection, true);
					}
					else
					{
						await _innerConnection.DisconnectAsync(cancellationToken).ConfigureAwait(false);
						_innerConnection = null;
					}

					throw;
				}
			}

			// Bind	Warning	messages event
			_innerConnection.Database.WarningMessage = OnWarningMessage;

			// Update the connection state
			OnStateChange(_state, ConnectionState.Open);
			_dbSQLDialect = GetDBSQLDialect();
			ValidateClientSQLDialect();
		}
		catch (IscException ex)
		{
			OnStateChange(_state, ConnectionState.Closed);
			throw IBException.Create(ex);
		}
		catch
		{
			OnStateChange(_state, ConnectionState.Closed);
			throw;
		}
	}

	public override void Close()
	{
		if (!IsClosed && _innerConnection != null)
		{
			try
			{
				_innerConnection.CloseEventManager();

				if (_innerConnection.Database != null)
				{
					_innerConnection.Database.WarningMessage = null;
				}

				_innerConnection.DisposeTransaction();

				_innerConnection.ReleasePreparedCommands();

				if (_options.Pooling)
				{
					var broken = _innerConnection.Database.ConnectionBroken;
					IBConnectionPoolManager.Instance.Release(_innerConnection, !broken);
					if (broken)
					{
						DisconnectEnlistedHelper();
					}
				}
				else
				{
					DisconnectEnlistedHelper();
				}
			}
			catch
			{ }
			finally
			{
				OnStateChange(_state, ConnectionState.Closed);
			}
		}

		void DisconnectEnlistedHelper()
		{
			if (!_innerConnection.IsEnlisted)
			{
				_innerConnection.Disconnect();
			}
			_innerConnection = null;
		}
	}
#if NET48 || NETSTANDARD2_0
	public async Task CloseAsync()
#else
	public override async Task CloseAsync()
#endif
	{
		if (!IsClosed && _innerConnection != null)
		{
			try
			{
				await _innerConnection.CloseEventManagerAsync(CancellationToken.None).ConfigureAwait(false);

				if (_innerConnection.Database != null)
				{
					_innerConnection.Database.WarningMessage = null;
				}

				await _innerConnection.DisposeTransactionAsync(CancellationToken.None).ConfigureAwait(false);

				await _innerConnection.ReleasePreparedCommandsAsync(CancellationToken.None).ConfigureAwait(false);

				if (_options.Pooling)
				{
					var broken = _innerConnection.Database.ConnectionBroken;
					IBConnectionPoolManager.Instance.Release(_innerConnection, !broken);
					if (broken)
					{
						await DisconnectEnlistedHelper().ConfigureAwait(false);
					}
				}
				else
				{
					await DisconnectEnlistedHelper().ConfigureAwait(false);
				}
			}
			catch
			{ }
			finally
			{
				OnStateChange(_state, ConnectionState.Closed);
			}
		}

		async Task DisconnectEnlistedHelper()
		{
			if (!_innerConnection.IsEnlisted)
			{
				await _innerConnection.DisconnectAsync(CancellationToken.None).ConfigureAwait(false);
			}
			_innerConnection = null;
		}
	}

	#endregion

	#region Private Methods

	private void CheckClosed()
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("Operation requires an open and available connection.");
		}
	}

	#endregion

	#region Event Handlers

	private void OnWarningMessage(IscException warning)
	{
		InfoMessage?.Invoke(this, new IBInfoMessageEventArgs(warning));
	}

	private void OnStateChange(ConnectionState originalState, ConnectionState currentState)
	{
		_state = currentState;
		StateChange?.Invoke(this, new StateChangeEventArgs(originalState, currentState));
	}

	#endregion

	#region Cancelation

	//internal void CancelCommand()
	//{
	//	CheckClosed();

	//	//_innerConnection.CancelCommand();
	//}
	//public Task CancelCommandAsync(CancellationToken cancellationToken = default)
	//{
	//	CheckClosed();

	//	return _innerConnection.CancelCommandAsync(cancellationToken);
	//}
	#endregion

	#region Internal Methods

	internal static void EnsureOpen(IBConnection connection)
	{
		if (connection == null || connection.State != ConnectionState.Open)
			throw new InvalidOperationException("Connection must be valid and open.");
	}

	#endregion
}