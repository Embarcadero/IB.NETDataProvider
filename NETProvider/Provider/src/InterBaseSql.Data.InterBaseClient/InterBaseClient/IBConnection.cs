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
using System.ComponentModel;
using System.Data;
using System.Data.Common;

using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient
{
	[DefaultEvent("InfoMessage")]
	public sealed class IBConnection : DbConnection, ICloneable
	{
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

		public static void CreateDatabase(string connectionString, bool overwrite)
		{
			CreateDatabaseImpl(connectionString, overwrite: overwrite);
		}

		public static void CreateDatabase(string connectionString, int pageSize = 4096, bool forcedWrites = true, bool overwrite = false)
		{
			CreateDatabaseImpl(connectionString, pageSize, forcedWrites, overwrite);
		}

		private static void CreateDatabaseImpl(string connectionString, int pageSize = 4096, bool forcedWrites = true, bool overwrite = false)
		{
			var options = new ConnectionString(connectionString);
			options.Validate();
			try
			{
				var dpb = new DatabaseParameterBuffer();

				dpb.Append(IscCodes.isc_dpb_version1);
				dpb.Append(IscCodes.isc_dpb_dummy_packet_interval, new byte[] { 120, 10, 0, 0 });
				dpb.Append(IscCodes.isc_dpb_sql_dialect, new byte[] { options.Dialect, 0, 0, 0 });
				if (!string.IsNullOrEmpty(options.UserID))
				{
					dpb.Append(IscCodes.isc_dpb_user_name, options.UserID);
				}
				if (!string.IsNullOrEmpty(options.Password))
				{
					dpb.Append(IscCodes.isc_dpb_password, options.Password);
				}
				if (options.Charset.Length > 0)
				{
					var charset = Charset.GetCharset(options.Charset);
					if (charset == null)
					{
						throw new ArgumentException("Character set is not valid.");
					}
				}
				dpb.Append(IscCodes.isc_dpb_force_write, (short)(forcedWrites ? 1 : 0));
				dpb.Append(IscCodes.isc_dpb_overwrite, (overwrite ? 1 : 0));
				if (pageSize > 0)
				{
					dpb.Append(IscCodes.isc_dpb_page_size, pageSize);
				}

				using (var db = new IBConnectionInternal(options))
				{
					db.CreateDatabase(dpb);

					if (options.Charset.Length > 0)
					{
						var aDb = new IBConnection();
						aDb.ConnectionString = connectionString;
						aDb.Open();

			            var sDefaultCharset = $"UPDATE RDB$DATABASE SET RDB$CHARACTER_SET_NAME='{Charset.GetCharset(options.Charset).Name}'";
						var trans = aDb.BeginTransaction();
						var cmd = new IBCommand(sDefaultCharset, aDb, trans);
						cmd.ExecuteNonQuery();
						trans.Commit();
						aDb.Close();
					}
				}
			}
			catch (IscException ex)
			{
				throw new IBException(ex.Message, ex);
			}
		}

		public static void DropDatabase(string connectionString)
		{
			var options = new ConnectionString(connectionString);
			options.Validate();

			try
			{
				using (var db = new IBConnectionInternal(options))
				{
					db.DropDatabase();
				}
			}
			catch (IscException ex)
			{
				throw new IBException(ex.Message, ex);
			}
		}

		#endregion

		#region Events

		public override event StateChangeEventHandler StateChange;

		public event EventHandler<IBInfoMessageEventArgs> InfoMessage;

		#endregion

		#region Fields

		private IBConnectionInternal _innerConnection;
		private ConnectionState _state;
		private ConnectionString _options;
		private bool _disposed;
		private string _connectionString;

		#endregion

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

		#region IDisposable methods

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

		#endregion

		#region ICloneable Methods

		object ICloneable.Clone()
		{
			return new IBConnection(ConnectionString);
		}

		#endregion

		#region Transaction Handling Methods

		public new IBTransaction BeginTransaction()
		{
			return BeginTransaction(IsolationLevel.ReadCommitted, null);
		}

		public new IBTransaction BeginTransaction(IsolationLevel level)
		{
			return BeginTransaction(level, null);
		}

		public IBTransaction BeginTransaction(string transactionName)
		{
			return BeginTransaction(IsolationLevel.ReadCommitted, transactionName);
		}

		public IBTransaction BeginTransaction(IsolationLevel level, string transactionName)
		{
			CheckClosed();

			return _innerConnection.BeginTransaction(level, transactionName);
		}

		public IBTransaction BeginTransaction(IBTransactionOptions options)
		{
			return BeginTransaction(options, null);
		}

		public IBTransaction BeginTransaction(IBTransactionOptions options, string transactionName)
		{
			CheckClosed();

			return _innerConnection.BeginTransaction(options, transactionName);
		}

		#endregion

		#region DbConnection methods

		protected override DbCommand CreateDbCommand()
		{
			return new IBCommand(null, this);
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return BeginTransaction(isolationLevel);
		}

		#endregion

		#region Database Schema Methods

		public override DataTable GetSchema()
		{
			return GetSchema("MetaDataCollections");
		}

		public override DataTable GetSchema(string collectionName)
		{
			return GetSchema(collectionName, null);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictions)
		{
			CheckClosed();

			return _innerConnection.GetSchema(collectionName, restrictions);
		}

		#endregion

		#region Methods

		public new IBCommand CreateCommand()
		{
			return (IBCommand)CreateDbCommand();
		}

		public override void ChangeDatabase(string db)
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
				Close();

				/* Set up the new Database	*/
				csb.Database = db;
				ConnectionString = csb.ToString();

				/* Open	new	connection	*/
				Open();
			}
			catch (IscException ex)
			{
				ConnectionString = oldConnectionString;
				throw new IBException(ex.Message, ex);
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

				if (_options.Pooling)
				{
					_innerConnection = IBConnectionPoolManager.Instance.Get(_options, this);
				}
				else
				{
					_innerConnection = new IBConnectionInternal(_options);
					_innerConnection.SetOwningConnection(this);
					_innerConnection.Connect();
				}

				if (_options.Enlist)
				{
					try
					{
						var transaction = System.Transactions.Transaction.Current;
						if (transaction != null)
						{
							_innerConnection.EnlistTransaction(transaction);
						}
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
							_innerConnection.Dispose();
							_innerConnection = null;
						}

						throw;
					}
				}

				// Bind	Warning	messages event
				_innerConnection.Database.WarningMessage = OnWarningMessage;

				// Update the connection state
				OnStateChange(_state, ConnectionState.Open);
			}
			catch (IscException ex)
			{
				OnStateChange(_state, ConnectionState.Closed);
				throw new IBException(ex.Message, ex);
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
						//if (_innerConnection.CancelDisabled)
						//{
						//	_innerConnection.EnableCancel();
						//}

						var broken = _innerConnection.Database.ConnectionBroken;
						IBConnectionPoolManager.Instance.Release(_innerConnection, !broken);
						if (broken)
						{
							EnlistedHelper();
						}
					}
					else
					{
						EnlistedHelper();
					}
				}
				catch
				{ }
				finally
				{
					OnStateChange(_state, ConnectionState.Closed);
				}
			}

			void EnlistedHelper()
			{
				if (!_innerConnection.IsEnlisted)
				{
					_innerConnection.Dispose();
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
		public void EnableCancel()
		{
			CheckClosed();

			//_innerConnection.EnableCancel();
		}

		public void DisableCancel()
		{
			CheckClosed();

			//_innerConnection.DisableCancel();
		}

		internal void CancelCommand()
		{
			CheckClosed();

			//_innerConnection.CancelCommand();
		}
		#endregion

		#region Internal Methods

		internal static void EnsureOpen(IBConnection connection)
		{
			if (connection == null || connection.State != ConnectionState.Open)
				throw new InvalidOperationException("Connection must be valid and open.");
		}

		#endregion
	}
}
