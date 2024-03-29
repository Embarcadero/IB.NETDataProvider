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
using System.Text;

using InterBaseSql.Data.Common;
using InterBaseSql.Data.Client.Native.Marshalers;
using InterBaseSql.Data.Client.Native.Handle;

namespace InterBaseSql.Data.Client.Native
{
	internal sealed class IBStatement : StatementBase
	{
		#region Fields

		private StatementHandle _handle;
		private bool _disposed;
		private IBDatabase _db;
		private IBTransaction _transaction;
		private Descriptor _parameters;
		private Descriptor _fields;
		private bool _allRowsFetched;
		private Queue<DbValue[]> _outputParams;
		private IntPtr[] _statusVector;
		private IntPtr _fetchSqlDa;

		#endregion

		#region Properties

		public override IDatabase Database
		{
			get { return _db; }
		}

		public override TransactionBase Transaction
		{
			get { return _transaction; }
			set
			{
				if (_transaction != value)
				{
					if (TransactionUpdate != null && _transaction != null)
					{
						_transaction.Update -= TransactionUpdate;
						TransactionUpdate = null;
					}

					if (value == null)
					{
						_transaction = null;
					}
					else
					{
						_transaction = (IBTransaction)value;
						TransactionUpdate = new EventHandler(TransactionUpdated);
						_transaction.Update += TransactionUpdate;
					}
				}
			}
		}

		public override Descriptor Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		public override Descriptor Fields
		{
			get { return _fields; }
		}

		public override int FetchSize
		{
			get { return 200; }
			set { }
		}

		#endregion

		#region Constructors

		public IBStatement(IDatabase db)
			: this(db, null)
		{
		}

		public IBStatement(IDatabase db, TransactionBase transaction)
		{
			if (!(db is IBDatabase))
			{
				throw new ArgumentException($"Specified argument is not of {nameof(IBDatabase)} type.");
			}

			_db = (IBDatabase)db;
			_handle = new StatementHandle();
			_outputParams = new Queue<DbValue[]>();
			_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
			_fetchSqlDa = IntPtr.Zero;

			if (transaction != null)
			{
				Transaction = transaction;
			}
		}

		#endregion

		#region IDisposable methods

		public override void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Release();
				Clear();
				_db = null;
				_fields = null;
				_parameters = null;
				_transaction = null;
				_outputParams = null;
				_statusVector = null;
				_allRowsFetched = false;
				_handle.Dispose();
				FetchSize = 0;
				base.Dispose();
			}
		}

		#endregion

		#region Blob Creation Metods

		public override BlobBase CreateBlob()
		{
			return new IBBlob(_db, _transaction);
		}

		public override BlobBase CreateBlob(long blobId)
		{
			return new IBBlob(_db, _transaction, blobId);
		}

		#endregion

		#region Array Creation Methods

		public override ArrayBase CreateArray(ArrayDesc descriptor)
		{
			return new IBArray(descriptor);
		}

		public override ArrayBase CreateArray(string tableName, string fieldName)
		{
			return new IBArray(_db, _transaction, tableName, fieldName);
		}

		public override ArrayBase CreateArray(long handle, string tableName, string fieldName)
		{
			return new IBArray(_db, _transaction, handle, tableName, fieldName);
		}

		#endregion

		#region Methods

		public override void Release()
		{
			XsqldaMarshaler.CleanUpNativeData(ref _fetchSqlDa);

			base.Release();
		}

		public override void Close()
		{
			XsqldaMarshaler.CleanUpNativeData(ref _fetchSqlDa);

			base.Close();
		}

		public override void Prepare(string commandText)
		{
			ClearAll();

			ClearStatusVector();

			if (State == StatementState.Deallocated)
			{
				Allocate();
			}

			_fields = new Descriptor(_db, 1);

			var sqlda = XsqldaMarshaler.MarshalManagedToNative(_fields);
			var trHandle = _transaction.HandlePtr;

			var buffer = _db.Charset.GetBytes(commandText);

			_db.IBClient.isc_dsql_prepare(
				_statusVector,
				ref trHandle,
				ref _handle,
				(short) buffer.Length,
				buffer,
				_db.Dialect,
				sqlda);

			var descriptor = XsqldaMarshaler.MarshalNativeToManaged(_db, sqlda);

			XsqldaMarshaler.CleanUpNativeData(ref sqlda);

			_db.ProcessStatusVector(_statusVector);

			_fields = descriptor;

			if (_fields.ActualCount > 0 && _fields.ActualCount != _fields.Count)
			{
				Describe();
			}
			else
			{
				if (_fields.ActualCount == 0)
				{
					_fields = new Descriptor(_db, 0);
				}
			}

			_fields.ResetValues();

			StatementType = GetStatementType();

			State = StatementState.Prepared;
		}

		public override void Execute()
		{
			if (State == StatementState.Deallocated)
			{
				throw new InvalidOperationException("Statment is not correctly created.");
			}

			ClearStatusVector();

			var inSqlda = IntPtr.Zero;
			var outSqlda = IntPtr.Zero;

			if (_parameters != null)
			{
				inSqlda = XsqldaMarshaler.MarshalManagedToNative(_parameters);
			}
			if (StatementType == DbStatementType.StoredProcedure)
			{
				Fields.ResetValues();
				outSqlda = XsqldaMarshaler.MarshalManagedToNative(_fields);
			}

			var trHandle = _transaction.HandlePtr;

			_db.IBClient.isc_dsql_execute2(
				_statusVector,
				ref trHandle,
				ref _handle,
				_db.IBClient.SQLDAVersion,
				inSqlda,
				outSqlda);

			if (outSqlda != IntPtr.Zero)
			{
				var descriptor = XsqldaMarshaler.MarshalNativeToManaged(_db, outSqlda, true);

				var values = new DbValue[descriptor.Count];

				for (var i = 0; i < values.Length; i++)
				{
					values[i] = new DbValue(this, descriptor[i]);
				}

				_outputParams.Enqueue(values);
			}

			XsqldaMarshaler.CleanUpNativeData(ref inSqlda);
			XsqldaMarshaler.CleanUpNativeData(ref outSqlda);

			_db.ProcessStatusVector(_statusVector);

			if (DoRecordsAffected)
			{
				RecordsAffected = GetRecordsAffected();
			}
			else
			{
				RecordsAffected = -1;
			}

			State = StatementState.Executed;
		}

		public override DbValue[] Fetch()
		{
			DbValue[] row = null;

			if (State == StatementState.Deallocated)
			{
				throw new InvalidOperationException("Statement is not correctly created.");
			}
			if (StatementType == DbStatementType.StoredProcedure && !_allRowsFetched)
			{
				_allRowsFetched = true;
				return GetOutputParameters();
			}
			else if (StatementType == DbStatementType.Insert && _allRowsFetched)
			{
				return null;
			}
			else if (StatementType != DbStatementType.Select && StatementType != DbStatementType.SelectForUpdate)
			{
				return null;
			}

			if (!_allRowsFetched)
			{
				_fields.ResetValues();

				if (_fetchSqlDa == IntPtr.Zero)
				{
					_fetchSqlDa = XsqldaMarshaler.MarshalManagedToNative(_fields);
				}

				ClearStatusVector();

				var status = _db.IBClient.isc_dsql_fetch(_statusVector, ref _handle, IscCodes.SQLDA_CURRENT_VERSION, _fetchSqlDa);

				var rowDesc = XsqldaMarshaler.MarshalNativeToManaged(_db, _fetchSqlDa, true);

				if (_fields.Count == rowDesc.Count)
				{
					for (var i = 0; i < _fields.Count; i++)
					{
						if (_fields[i].IsArray() && _fields[i].ArrayHandle != null)
						{
							rowDesc[i].ArrayHandle = _fields[i].ArrayHandle;
						}
					}
				}

				_fields = rowDesc;

				_db.ProcessStatusVector(_statusVector);

				if (status == new IntPtr(100))
				{
					_allRowsFetched = true;

					XsqldaMarshaler.CleanUpNativeData(ref _fetchSqlDa);
				}
				else
				{
					row = new DbValue[_fields.ActualCount];
					for (var i = 0; i < row.Length; i++)
					{
						row[i] = new DbValue(this, _fields[i]);
					}
				}
			}

			return row;
		}

		public override DbValue[] GetOutputParameters()
		{
			if (_outputParams != null && _outputParams.Count > 0)
			{
				return _outputParams.Dequeue();
			}

			return null;
		}

		public override void Describe()
		{
			ClearStatusVector();

			_fields = new Descriptor(_db, _fields.ActualCount);

			var sqlda = XsqldaMarshaler.MarshalManagedToNative(_fields);


			_db.IBClient.isc_dsql_describe(
				_statusVector,
				ref _handle,
				IscCodes.SQLDA_VERSION1,
				sqlda);

			var descriptor = XsqldaMarshaler.MarshalNativeToManaged(_db, sqlda);

			XsqldaMarshaler.CleanUpNativeData(ref sqlda);

			_db.ProcessStatusVector(_statusVector);

			_fields = descriptor;
		}

		public override void DescribeParameters()
		{
			ClearStatusVector();

			_parameters = new Descriptor(_db, 1);

			var sqlda = XsqldaMarshaler.MarshalManagedToNative(_parameters);


			_db.IBClient.isc_dsql_describe_bind(
				_statusVector,
				ref _handle,
				IscCodes.SQLDA_CURRENT_VERSION,
				sqlda);

			var descriptor = XsqldaMarshaler.MarshalNativeToManaged(_db, sqlda);

			_db.ProcessStatusVector(_statusVector);

			if (descriptor.ActualCount != 0 && descriptor.Count != descriptor.ActualCount)
			{
				var n = descriptor.ActualCount;
				descriptor = new Descriptor(_db, n);

				XsqldaMarshaler.CleanUpNativeData(ref sqlda);

				sqlda = XsqldaMarshaler.MarshalManagedToNative(descriptor);

				_db.IBClient.isc_dsql_describe_bind(
					_statusVector,
					ref _handle,
					IscCodes.SQLDA_CURRENT_VERSION,
					sqlda);

				descriptor = XsqldaMarshaler.MarshalNativeToManaged(_db, sqlda);

				XsqldaMarshaler.CleanUpNativeData(ref sqlda);

				_db.ProcessStatusVector(_statusVector);
			}
			else
			{
				if (descriptor.ActualCount == 0)
				{
					descriptor = new Descriptor(_db, 0);
				}
			}

			if (sqlda != IntPtr.Zero)
			{
				XsqldaMarshaler.CleanUpNativeData(ref sqlda);
			}

			_parameters = descriptor;
		}

		#endregion

		#region Protected Methods

		protected override void Free(int option)
		{
			// Does	not	seem to	be possible	or necessary to	close
			// an execute procedure	statement.
			if (StatementType == DbStatementType.StoredProcedure && option == IscCodes.DSQL_close)
			{
				return;
			}

			ClearStatusVector();

			_db.IBClient.isc_dsql_free_statement(
				_statusVector,
				ref _handle,
				(short)option);

			if (option == IscCodes.DSQL_drop)
			{
				_parameters = null;
				_fields = null;
			}

			Clear();
			_allRowsFetched = false;

			_db.ProcessStatusVector(_statusVector);
		}

		protected override void TransactionUpdated(object sender, EventArgs e)
		{
			if (Transaction != null && TransactionUpdate != null)
			{
				Transaction.Update -= TransactionUpdate;
			}
			Clear();
			State = StatementState.Closed;
			TransactionUpdate = null;
			_allRowsFetched = false;
		}

		protected override byte[] GetSqlInfo(byte[] items, int bufferLength)
		{
			ClearStatusVector();

			var buffer = new byte[bufferLength];


			_db.IBClient.isc_dsql_sql_info(
				_statusVector,
				ref _handle,
				(short)items.Length,
				items,
				(short)bufferLength,
				buffer);

			_db.ProcessStatusVector(_statusVector);

			return buffer;
		}

		#endregion

		#region Private Methods

		private void ClearStatusVector()
		{
			Array.Clear(_statusVector, 0, _statusVector.Length);
		}

		private void Clear()
		{
			_outputParams?.Clear();
		}

		private void ClearAll()
		{
			Clear();

			_parameters = null;
			_fields = null;
		}

		private void Allocate()
		{
			ClearStatusVector();

			var dbHandle = _db.HandlePtr;

			_db.IBClient.isc_dsql_alloc_statement2(
				_statusVector,
				ref dbHandle,
				ref _handle);

			_db.ProcessStatusVector(_statusVector);

			_allRowsFetched = false;
			State = StatementState.Allocated;
			StatementType = DbStatementType.None;
		}

		#endregion
	}
}
