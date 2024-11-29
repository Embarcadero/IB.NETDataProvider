﻿/*
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
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient;

public sealed class IBRemoteEvent : IDisposable
#if !(NET48 || NETSTANDARD2_0)
	, IAsyncDisposable
#endif
{
	private IBConnectionInternal _connection;
	private RemoteEvent _revent;
	private SynchronizationContext _synchronizationContext;

	public event EventHandler<IBRemoteEventCountsEventArgs> RemoteEventCounts;
	public event EventHandler<IBRemoteEventErrorEventArgs> RemoteEventError;

	public string this[int index] => _revent != null ? _revent.Events[index] : throw new InvalidOperationException();
	public int RemoteEventId => _revent != null ? _revent.RemoteId : throw new InvalidOperationException();

	public IBRemoteEvent(string connectionString)
	{
		_connection = new IBConnectionInternal(new ConnectionString(connectionString));
	}

	public void Open()
	{
		if (_revent != null)
			throw new InvalidOperationException($"{nameof(IBRemoteEvent)} already open.");

		_connection.Connect();
		_revent = new RemoteEvent(_connection.Database);
		_revent.EventCountsCallback = OnRemoteEventCounts;
		_revent.EventErrorCallback = OnRemoteEventError;
		_synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
	}
	public async Task OpenAsync(CancellationToken cancellationToken = default)
	{
		if (_revent != null)
			throw new InvalidOperationException($"{nameof(IBRemoteEvent)} already open.");

		await _connection.ConnectAsync(cancellationToken).ConfigureAwait(false);
		_revent = new RemoteEvent(_connection.Database);
		_revent.EventCountsCallback = OnRemoteEventCounts;
		_revent.EventErrorCallback = OnRemoteEventError;
		_synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
	}

	public void Dispose()
	{
		_connection.Disconnect();
	}
#if !(NET48 || NETSTANDARD2_0)
	public ValueTask DisposeAsync()
	{
		return new ValueTask(_connection.DisconnectAsync(CancellationToken.None));
	}
#endif

	public void QueueEvents(ICollection<string> events)
	{
		if (_revent == null)
			throw new InvalidOperationException($"{nameof(IBRemoteEvent)} must be opened.");

		try
		{
			_revent.QueueEvents(events);
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}
	public async Task QueueEventsAsync(ICollection<string> events, CancellationToken cancellationToken = default)
	{
		if (_revent == null)
			throw new InvalidOperationException($"{nameof(IBRemoteEvent)} must be opened.");

		try
		{
			await _revent.QueueEventsAsync(events, cancellationToken).ConfigureAwait(false);
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}

	public void CancelEvents()
	{
		try
		{
			_revent.CancelEvents();
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}
	public async Task CancelEventsAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			await _revent.CancelEventsAsync(cancellationToken).ConfigureAwait(false);
		}
		catch (IscException ex)
		{
			throw IBException.Create(ex);
		}
	}

	private void OnRemoteEventCounts(string name, int count)
	{
		var args = new IBRemoteEventCountsEventArgs(name, count);
		_synchronizationContext.Post(_ =>
		{
			RemoteEventCounts?.Invoke(this, args);
		}, null);
	}

	private void OnRemoteEventError(Exception error)
	{
		var args = new IBRemoteEventErrorEventArgs(error);
		_synchronizationContext.Post(_ =>
		{
			RemoteEventError?.Invoke(this, args);
		}, null);
	}
}