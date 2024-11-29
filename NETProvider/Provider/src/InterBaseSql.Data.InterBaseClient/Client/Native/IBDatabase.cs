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
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Client.Native.Handles;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Client.Native;

internal sealed class IBDatabase : DatabaseBase
{
	#region Fields

	private DatabaseHandle _handle;
	private IntPtr[] _statusVector;

	#endregion

	#region Properties

	public override int Handle => _handle.DangerousGetHandle().AsInt();
	public override bool HasRemoteEventSupport => false;
	public override bool ConnectionBroken => false;
	public DatabaseHandle HandlePtr => _handle;

	#endregion

	#region Constructors

	public IBDatabase(IBServerType serverType, ConnectionString options)
		: base(options)
	{
		IBClient = IBClientFactory.GetGDSLibrary(serverType);
		_handle = new DatabaseHandle();
		_statusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];
	}

	#endregion

	#region Database Methods

	public override void CreateDatabase(DatabaseParameterBuffer dpb, string database)
	{
		var databaseBuffer = dpb.Encoding.GetBytes(database);

		StatusVectorHelper.ClearStatusVector(_statusVector);

		IBClient.isc_create_database(
			_statusVector,
			(short)databaseBuffer.Length,
			databaseBuffer,
			ref _handle,
			dpb.Length,
			dpb.ToArray(),
			0);

		ProcessStatusVector(Charset.DefaultCharset);
	}

	public override ValueTask CreateDatabaseAsync(DatabaseParameterBuffer dpb, string database, CancellationToken cancellationToken = default)
	{
		var databaseBuffer = dpb.Encoding.GetBytes(database);

		StatusVectorHelper.ClearStatusVector(_statusVector);

		IBClient.isc_create_database(
			_statusVector,
			(short)databaseBuffer.Length,
			databaseBuffer,
			ref _handle,
			dpb.Length,
			dpb.ToArray(),
			0);

		ProcessStatusVector(Charset.DefaultCharset);

		return ValueTask2.CompletedTask;
	}


	public override void DropDatabase()
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);
		IBClient.isc_drop_database(_statusVector, ref _handle);
		ProcessStatusVector();
		_handle.Dispose();
	}
	public override ValueTask DropDatabaseAsync(CancellationToken cancellationToken = default)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);
		IBClient.isc_drop_database(_statusVector, ref _handle);
		ProcessStatusVector();
		_handle.Dispose();
		return ValueTask2.CompletedTask;
	}

	#endregion

	#region Remote Events Methods

	public override void CloseEventManager()
	{
		throw new NotSupportedException();
	}
	public override ValueTask CloseEventManagerAsync(CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	public override void QueueEvents(RemoteEvent events)
	{
		throw new NotSupportedException();
	}
	public override ValueTask QueueEventsAsync(RemoteEvent events, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	public override void CancelEvents(RemoteEvent events)
	{
		throw new NotSupportedException();
	}
	public override ValueTask CancelEventsAsync(RemoteEvent events, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException();
	}

	#endregion

	#region Methods

	public override void Attach(DatabaseParameterBuffer dpb, string database)
	{

		var databaseBuffer = dpb.Encoding.GetBytes(database);

		StatusVectorHelper.ClearStatusVector(_statusVector);

		IBClient.isc_attach_database(
			_statusVector,
			(short)databaseBuffer.Length,
			databaseBuffer,
			ref _handle,
			dpb.Length,
			dpb.ToArray());

		ProcessStatusVector(Charset.DefaultCharset);

		ServerVersion = GetServerVersion();
	}

	public override async ValueTask AttachAsync(DatabaseParameterBuffer dpb, string database, CancellationToken cancellationToken = default)
	{

		var databaseBuffer = dpb.Encoding.GetBytes(database);

		StatusVectorHelper.ClearStatusVector(_statusVector);

		IBClient.isc_attach_database(
			_statusVector,
			(short)databaseBuffer.Length,
			databaseBuffer,
			ref _handle,
			dpb.Length,
			dpb.ToArray());

		ProcessStatusVector(Charset.DefaultCharset);

		ServerVersion = await GetServerVersionAsync(cancellationToken).ConfigureAwait(false);
	}

	public override void Detach()
	{
		if (TransactionCount > 0)
		{
			throw IscException.ForErrorCodeIntParam(IscCodes.isc_open_trans, TransactionCount);
		}

		if (!_handle.IsInvalid)
		{
			StatusVectorHelper.ClearStatusVector(_statusVector);

			IBClient.isc_detach_database(_statusVector, ref _handle);

			ProcessStatusVector();

			_handle.Dispose();
		}

		WarningMessage = null;
		ServerVersion = null;
		_statusVector = null;
		TransactionCount = 0;
	}
	public override ValueTask DetachAsync(CancellationToken cancellationToken = default)
	{
		if (TransactionCount > 0)
		{
			throw IscException.ForErrorCodeIntParam(IscCodes.isc_open_trans, TransactionCount);
		}

		if (!_handle.IsInvalid)
		{
			StatusVectorHelper.ClearStatusVector(_statusVector);

			IBClient.isc_detach_database(_statusVector, ref _handle);

			ProcessStatusVector();

			_handle.Dispose();
		}

		WarningMessage = null;
		ServerVersion = null;
		_statusVector = null;
		TransactionCount = 0;

		return ValueTask2.CompletedTask;
	}

	#endregion

	#region Transaction Methods

	public override TransactionBase BeginTransaction(TransactionParameterBuffer tpb)
	{
		var transaction = new IBTransaction(this);
		transaction.BeginTransaction(tpb);
		return transaction;
	}
	public override async ValueTask<TransactionBase> BeginTransactionAsync(TransactionParameterBuffer tpb, CancellationToken cancellationToken = default)
	{
		var transaction = new IBTransaction(this);
		await transaction.BeginTransactionAsync(tpb, cancellationToken).ConfigureAwait(false);
		return transaction;
	}

	#endregion

	#region Cancel Methods

	public override void CancelOperation(short kind)
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

	public override ValueTask CancelOperationAsync(short kind, CancellationToken cancellationToken = default)
	{
		//var localStatusVector = new IntPtr[IscCodes.ISC_STATUS_LENGTH];

		//IBClient.IB_cancel_operation(localStatusVector, ref _handle, kind);

		//try
		//{
		//	ProcessStatusVector(localStatusVector);
		//}
		//catch (IscException ex) when (ex.ErrorCode == IscCodes.isc_nothing_to_cancel)
		//{ }

		return ValueTask2.CompletedTask;
	}

	#endregion

	#region Statement Creation Methods

	public override StatementBase CreateStatement()
	{
		return new IBStatement(this);
	}

	public override StatementBase CreateStatement(TransactionBase transaction)
	{
		return new IBStatement(this, transaction as IBTransaction);
	}

	#endregion

	#region Parameter Buffers

	public override DatabaseParameterBuffer CreateDatabaseParameterBuffer()
	{
		return new DatabaseParameterBuffer(ParameterBufferEncoding);
	}

	public override EventParameterBuffer CreateEventParameterBuffer()
	{
		return new EventParameterBuffer(Charset.Encoding);
	}

	public override TransactionParameterBuffer CreateTransactionParameterBuffer()
	{
		return new TransactionParameterBuffer(Charset.Encoding);
	}

	#endregion

	#region Database Information Methods

	public override List<object> GetDatabaseInfo(byte[] items)
	{
		return GetDatabaseInfo(items, IscCodes.DEFAULT_MAX_BUFFER_SIZE);
	}
	public override ValueTask<List<object>> GetDatabaseInfoAsync(byte[] items, CancellationToken cancellationToken = default)
	{
		return GetDatabaseInfoAsync(items, IscCodes.DEFAULT_MAX_BUFFER_SIZE, cancellationToken);
	}

	public override List<object> GetDatabaseInfo(byte[] items, int bufferLength)
	{
		var buffer = new byte[bufferLength];
		DatabaseInfo(items, buffer, buffer.Length);
		return IscHelper.ParseDatabaseInfo(buffer, Charset);
	}
	public override ValueTask<List<object>> GetDatabaseInfoAsync(byte[] items, int bufferLength, CancellationToken cancellationToken = default)
	{
		var buffer = new byte[bufferLength];

		DatabaseInfo(items, buffer, buffer.Length);

		return ValueTask2.FromResult(IscHelper.ParseDatabaseInfo(buffer, Charset));
	}

	#endregion

	#region Internal Methods

	internal void ProcessStatusVector(IntPtr[] statusVector)
	{
		StatusVectorHelper.ProcessStatusVector(statusVector, Charset, WarningMessage);
	}

	#endregion

	#region Private Methods

	private void DatabaseInfo(byte[] items, byte[] buffer, int bufferLength)
	{
		StatusVectorHelper.ClearStatusVector(_statusVector);

		IBClient.isc_database_info(
			_statusVector,
			ref _handle,
			(short)items.Length,
			items,
			(short)bufferLength,
			buffer);

		ProcessStatusVector();
	}

	private void ProcessStatusVector()
	{
		StatusVectorHelper.ProcessStatusVector(_statusVector, Charset, WarningMessage);
	}

	private void ProcessStatusVector(Charset charset)
	{
		StatusVectorHelper.ProcessStatusVector(_statusVector, charset, WarningMessage);
	}

	#endregion

}
