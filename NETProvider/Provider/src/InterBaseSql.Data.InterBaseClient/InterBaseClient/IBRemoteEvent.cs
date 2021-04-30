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
using System.Linq;
using System.Threading;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient
{
	public sealed class IBRemoteEvent : IDisposable
	{
		private IBConnectionInternal _connection;
		private RemoteEvent _revent;
		private SynchronizationContext _synchronizationContext;

		public event EventHandler<IBRemoteEventCountsEventArgs> RemoteEventCounts;
		public event EventHandler<IBRemoteEventErrorEventArgs> RemoteEventError;

		public string this[int index] => _revent.Events[index];
		public int RemoteEventId => _revent?.RemoteId ?? -1;

		public IBRemoteEvent(string connectionString)
		{
			_connection = new IBConnectionInternal(new ConnectionString(connectionString));
			_connection.Connect();
			_revent = new RemoteEvent(_connection.Database);
			_revent.EventCountsCallback = OnRemoteEventCounts;
			_revent.EventErrorCallback = OnRemoteEventError;
			_synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
		}

		public void Dispose()
		{
			_connection.Dispose();
		}

		public void QueueEvents(params string[] events)
		{
			try
			{
				_revent.QueueEvents(events);
			}
			catch (IscException ex)
			{
				throw new IBException(ex.Message, ex);
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
				throw new IBException(ex.Message, ex);
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
}
