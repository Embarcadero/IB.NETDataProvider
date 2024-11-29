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
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Client.Native;

namespace InterBaseSql.Data.Common;

internal abstract class DatabaseBase
{
	public Action<IscException> WarningMessage { get; set; }

	public Encoding ParameterBufferEncoding => Encoding2.Default;
	public abstract int Handle { get; }
	public Charset Charset { get; }
	public int PacketSize { get; }
	public short Dialect { get; set; }
	public int TransactionCount { get; set; }
	public string ServerVersion { get; protected set; }
	public bool TruncateChar { get; set; }
	public IIBClient IBClient { get; set; }

	public abstract bool HasRemoteEventSupport { get; }
	public abstract bool ConnectionBroken { get; }
	public DatabaseBase(ConnectionString options)
	{
		if (!Charset.TryGetByName(options.Charset, out var charset))
			throw InvalidCharsetException();
		Charset = charset;
		PacketSize = options.PacketSize;
		Dialect = options.Dialect;
		TruncateChar = options.TruncateChar;
	}

	public abstract void Attach(DatabaseParameterBuffer dpb, string database);
	public abstract ValueTask AttachAsync(DatabaseParameterBuffer dpb, string database, CancellationToken cancellationToken = default);
	public abstract void Detach();
	public abstract ValueTask DetachAsync(CancellationToken cancellationToken = default);

	public abstract void CreateDatabase(DatabaseParameterBuffer dpb, string database);
	public abstract ValueTask CreateDatabaseAsync(DatabaseParameterBuffer dpb, string database,CancellationToken cancellationToken = default);

	public abstract void DropDatabase();
	public abstract ValueTask DropDatabaseAsync(CancellationToken cancellationToken = default);

	public abstract TransactionBase BeginTransaction(TransactionParameterBuffer tpb);
	public abstract ValueTask<TransactionBase> BeginTransactionAsync(TransactionParameterBuffer tpb, CancellationToken cancellationToken = default);

	public abstract StatementBase CreateStatement();
	public abstract StatementBase CreateStatement(TransactionBase transaction);

	public abstract DatabaseParameterBuffer CreateDatabaseParameterBuffer();
	public abstract EventParameterBuffer CreateEventParameterBuffer();
	public abstract TransactionParameterBuffer CreateTransactionParameterBuffer();

	public abstract List<object> GetDatabaseInfo(byte[] items);
	public abstract ValueTask<List<object>> GetDatabaseInfoAsync(byte[] items, CancellationToken cancellationToken = default);

	public abstract List<object> GetDatabaseInfo(byte[] items, int bufferLength);
	public abstract ValueTask<List<object>> GetDatabaseInfoAsync(byte[] items, int bufferLength, CancellationToken cancellationToken = default);

	public abstract void CloseEventManager();
	public abstract ValueTask CloseEventManagerAsync(CancellationToken cancellationToken = default);

	public abstract void QueueEvents(RemoteEvent events);
	public abstract ValueTask QueueEventsAsync(RemoteEvent events, CancellationToken cancellationToken = default);

	public abstract void CancelEvents(RemoteEvent events);
	public abstract ValueTask CancelEventsAsync(RemoteEvent events, CancellationToken cancellationToken = default);

	public abstract void CancelOperation(short kind);
	public abstract ValueTask CancelOperationAsync(short kind, CancellationToken cancellationToken = default);

	public string GetServerVersion()
	{
		var items = new byte[]
		{
			IscCodes.isc_info_version,
			IscCodes.isc_info_end
		};
		var info = GetDatabaseInfo(items, IscCodes.BUFFER_SIZE_256);
		return (string)info[info.Count -1];
	}
	public async ValueTask<string> GetServerVersionAsync(CancellationToken cancellationToken = default)
	{
		var items = new byte[]
		{
			IscCodes.isc_info_version,
			IscCodes.isc_info_end
		};
		var info = await GetDatabaseInfoAsync(items, IscCodes.BUFFER_SIZE_256, cancellationToken).ConfigureAwait(false);
		return (string)info[info.Count - 1];
	}
	private static Exception InvalidCharsetException() => new ArgumentException("Invalid character set specified.");

}
