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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient;

sealed class IBConnectionPoolManager : IDisposable
{
	internal static IBConnectionPoolManager Instance { get; private set; }

	sealed class Item
	{
		public long Created { get; private set; }
		public IBConnectionInternal Connection { get; private set; }

		public Item(long created, IBConnectionInternal connection)
		{
			Created = created;
			Connection = connection;
		}

		public void Release()
		{
			Connection.Disconnect();
		}
	}

	sealed class Pool : IDisposable
	{
		bool _disposed;
		object _syncRoot;
		ConnectionString _connectionString;
		Stack<Item> _available;
		List<IBConnectionInternal> _busy;

		public Pool(ConnectionString connectionString)
		{
			_syncRoot = new object();
			_connectionString = connectionString;
			_available = new Stack<Item>();
			_busy = new List<IBConnectionInternal>();
		}

		public void Dispose()
		{
			lock (_syncRoot)
			{
				if (_disposed)
					return;
				_disposed = true;
				CleanConnectionsImpl();
			}
		}

		public IBConnectionInternal GetConnection(out bool createdNew)
		{
			IBConnectionInternal connection;
			lock (_syncRoot)
			{
				CheckDisposedImpl();

				connection = GetOrCreateConnectionImpl(out createdNew);
				_busy.Add(connection);
			}
			return connection;
		}

		public void ReleaseConnection(IBConnectionInternal connection, bool returnToAvailable)
		{
			lock (_syncRoot)
			{
				CheckDisposedImpl();

				var removed = _busy.Remove(connection);
				if (removed && returnToAvailable)
				{
					_available.Push(new Item(GetTicks(), connection));
				}
			}
		}

		public void PrunePool()
		{
			lock (_syncRoot)
			{
				CheckDisposedImpl();

				var now = GetTicks();
				var available = _available.ToList();
				if (available.Count <= _connectionString.MinPoolSize)
					return;
				var keep = available.Where(x => ConnectionPoolLifetimeHelper.IsAlive(_connectionString.ConnectionLifetime, x.Created, now)).ToList();
				var keepCount = keep.Count;
				if (keepCount < _connectionString.MinPoolSize)
				{
					keep = keep.Concat(available.Except(keep).OrderByDescending(x => x.Created).Take(_connectionString.MinPoolSize - keepCount)).ToList();
				}
				var release = available.Except(keep).ToList();
				Parallel.ForEach(release, x => x.Release());
				_available = new Stack<Item>(keep);
			}
		}

		public void ClearPool()
		{
			lock (_syncRoot)
			{
				CheckDisposedImpl();

				CleanConnectionsImpl();
				_available.Clear();
			}
		}

		void CleanConnectionsImpl()
		{
			Parallel.ForEach(_available, x => x.Release());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void CheckDisposedImpl()
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(Pool));
		}

		IBConnectionInternal GetOrCreateConnectionImpl(out bool createdNew)
		{
			if (_available.Any())
			{
				createdNew = false;
				return _available.Pop().Connection;
			}
			else
			{
				createdNew = true;
				if (_busy.Count + 1 > _connectionString.MaxPoolSize)
					throw new InvalidOperationException("Connection pool is full.");
				return new IBConnectionInternal(_connectionString);
			}
		}

		static long GetTicks()
		{
			var ticks = Environment.TickCount;
			return ticks + -(long)int.MinValue;
		}
	}

	int _disposed;
	ConcurrentDictionary<string, Pool> _pools;
	Timer _cleanupTimer;

	static IBConnectionPoolManager()
	{
		Instance = new IBConnectionPoolManager();
		ShutdownHelper.RegisterPoolCleanup(Instance.Dispose);
	}

	IBConnectionPoolManager()
	{
		_disposed = 0;
		_pools = new ConcurrentDictionary<string, Pool>();
		_cleanupTimer = new Timer(CleanupCallback, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
	}

	internal IBConnectionInternal Get(ConnectionString connectionString, out bool createdNew)
	{
		CheckDisposed();

		return _pools.GetOrAdd(connectionString.NormalizedConnectionString, _ => new Pool(connectionString)).GetConnection(out createdNew);
	}

	internal void Release(IBConnectionInternal connection, bool returnToAvailable)
	{
		CheckDisposed();

		if (_pools.TryGetValue(connection.ConnectionStringOptions.NormalizedConnectionString, out var pool))
		{
			pool.ReleaseConnection(connection, returnToAvailable);
		}
	}

	internal void ClearAllPools()
	{
		CheckDisposed();

		Parallel.ForEach(_pools.Values, x => x.ClearPool());
	}

	internal void ClearPool(ConnectionString connectionString)
	{
		CheckDisposed();

		if (_pools.TryGetValue(connectionString.NormalizedConnectionString, out var pool))
		{
			pool.ClearPool();
		}
	}

	public void Dispose()
	{
		if (Interlocked.Exchange(ref _disposed, 1) == 1)
			return;
		using (var mre = new ManualResetEvent(false))
		{
			_cleanupTimer.Dispose(mre);
			mre.WaitOne();
		}
		Parallel.ForEach(_pools.Values, x => x.Dispose());
	}

	void CleanupCallback(object o)
	{
		Parallel.ForEach(_pools.Values, x => x.PrunePool());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void CheckDisposed()
	{
		if (Volatile.Read(ref _disposed) == 1)
			throw new ObjectDisposedException(nameof(IBConnectionPoolManager));
	}
}
