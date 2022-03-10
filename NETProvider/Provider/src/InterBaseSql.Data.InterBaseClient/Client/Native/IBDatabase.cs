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
using System.Threading;
using System.Text;

using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.Client.Native.Handle;
using System.Collections.Generic;

namespace InterBaseSql.Data.Client.Native
{
	internal sealed class IBDatabase : IDatabase
	{
		#region Callbacks

		public Action<IscException> WarningMessage
		{
			get { return _warningMessage; }
			set { _warningMessage = value; }
		}

		#endregion

		#region Fields

		private Action<IscException> _warningMessage;

		private DatabaseHandle _handle;
		private int _transactionCount;
		private string _serverVersion;
		private Charset _charset;
		private short _packetSize;
		private short _dialect;
		private IntPtr[] _statusVector;
		private bool _truncateChar;

		private IIBClient _ibClient;

		#endregion

		#region Properties

		public int Handle
		{
			get { return _handle.DangerousGetHandle().AsInt(); }
		}

		public DatabaseHandle HandlePtr
		{
			get { return _handle; }
		}

		public int TransactionCount
		{
			get { return _transactionCount; }
			set { _transactionCount = value; }
		}

		public string ServerVersion
		{
			get { return _serverVersion; }
		}

		public Charset Charset
		{
			get { return _charset; }
			set { _charset = value; }
		}

		public short PacketSize
		{
			get { return _packetSize; }
			set { _packetSize = value; }
		}

		public short Dialect
		{
			get { return _dialect; }
			set { _dialect = value; }
		}

		public bool TruncateChar
		{
			get { return _truncateChar; }
			set { _truncateChar = value; }
		}
		public bool HasRemoteEventSupport
		{
			get { return false; }
		}

		public IIBClient IBClient
		{
			get { return _ibClient; }
		}

		public bool ConnectionBroken
		{
			get { return false; }
		}

		#endregion

		#region Constructors

		public IBDatabase(IBServerType serverType, Charset charset)
		{
			_ibClient = IBClientFactory.GetGDSLibrary(serverType);
			_handle = new DatabaseHandle();
			_charset = charset ?? Charset.DefaultCharset;
			_dialect = 3;
			_packetSize = 8192;
			_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
		}

		#endregion

		#region Database Methods

		public void CreateDatabase(DatabaseParameterBuffer dpb, string database)
		{
			var databaseBuffer = Encoding.Default.GetBytes(database);

			ClearStatusVector();

			_ibClient.isc_create_database(
				_statusVector,
				(short)databaseBuffer.Length,
				databaseBuffer,
				ref _handle,
				dpb.Length,
				dpb.ToArray(),
				0);

			ProcessStatusVector(_statusVector);
		}

		public void DropDatabase()
		{
			ClearStatusVector();

			_ibClient.isc_drop_database(_statusVector, ref _handle);

			ProcessStatusVector(_statusVector);

			_handle.Dispose();
		}

		#endregion

		#region Remote Events Methods

		public void CloseEventManager()
		{
			throw new NotSupportedException();
		}

		public void QueueEvents(RemoteEvent events)
		{
			throw new NotSupportedException();
		}

		public void CancelEvents(RemoteEvent events)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Methods

		public void Attach(DatabaseParameterBuffer dpb, string database)
		{

			var databaseBuffer = Encoding.Default.GetBytes(database);

			ClearStatusVector();

			_ibClient.isc_attach_database(
				_statusVector,
				(short)databaseBuffer.Length,
				databaseBuffer,
				ref _handle,
				dpb.Length,
				dpb.ToArray());

			ProcessStatusVector(_statusVector);

			_serverVersion = GetServerVersion();
		}

		public void Detach()
		{
			if (TransactionCount > 0)
			{
				throw IscException.ForErrorCodeIntParam(IscCodes.isc_open_trans, TransactionCount);
			}

			if (!_handle.IsInvalid)
			{
				ClearStatusVector();

				_ibClient.isc_detach_database(_statusVector, ref _handle);

				ProcessStatusVector(_statusVector);

				_handle.Dispose();
			}

			_warningMessage = null;
			_charset = null;
			_serverVersion = null;
			_statusVector = null;
			_transactionCount = 0;
			_dialect = 0;
			_packetSize = 0;
		}

		#endregion

		#region Transaction Methods

		public TransactionBase BeginTransaction(TransactionParameterBuffer tpb)
		{
			var transaction = new IBTransaction(this);
			transaction.BeginTransaction(tpb);

			return transaction;
		}

		#endregion

		#region Cancel Methods

		public void CancelOperation(int kind)
		{
	//		const
 // SQLATTACHMENTCANCEL =
	//'UPDATE TMP$ATTACHMENTS SET TMP$STATE = ''CANCEL'' WHERE %s';
	//		begin
	//		  Result := ExecuteOperation(Format(SQLATTACHMENTCANCEL,
			
	//			[TwoColAndOrArray('TMP$ATTACHMENT_ID', 'TMP$DATABASE_ID', AAttachmentID,
	//			ADatabaseID)]));

			//var localStatusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];

			//_ibClient.fb_cancel_operation(localStatusVector, ref _handle, kind);

			//ProcessStatusVector(localStatusVector);
		}

		#endregion

		#region Statement Creation Methods

		public StatementBase CreateStatement()
		{
			return new IBStatement(this);
		}

		public StatementBase CreateStatement(TransactionBase transaction)
		{
			return new IBStatement(this, transaction as IBTransaction);
		}

		#endregion

		#region Database Information Methods

		public string GetServerVersion()
		{
			var items = new byte[]
			{
				IscCodes.isc_info_version,
				IscCodes.isc_info_end
			};
			return GetDatabaseInfo(items, IscCodes.BUFFER_SIZE_128)[0].ToString();
		}

		public List<object> GetDatabaseInfo(byte[] items)
		{
			return GetDatabaseInfo(items, IscCodes.DEFAULT_MAX_BUFFER_SIZE);
		}

		public List<object> GetDatabaseInfo(byte[] items, int bufferLength)
		{
			var buffer = new byte[bufferLength];

			DatabaseInfo(items, buffer, buffer.Length);

			return IscHelper.ParseDatabaseInfo(buffer);
		}

		#endregion

		#region Internal Methods

		internal void ProcessStatusVector(IntPtr[] statusVector)
		{
			var ex = IBConnection.ParseStatusVector(statusVector, _charset);

			if (ex != null)
			{
				if (ex.IsWarning)
				{
					_warningMessage?.Invoke(ex);
				}
				else
				{
					throw ex;
				}
			}
		}

		#endregion

		#region Private Methods

		private void ClearStatusVector()
		{
			Array.Clear(_statusVector, 0, _statusVector.Length);
		}

		private void DatabaseInfo(byte[] items, byte[] buffer, int bufferLength)
		{
			ClearStatusVector();

			_ibClient.isc_database_info(
				_statusVector,
				ref _handle,
				(short)items.Length,
				items,
				(short)bufferLength,
				buffer);

			ProcessStatusVector(_statusVector);
		}

		#endregion

	}
}
