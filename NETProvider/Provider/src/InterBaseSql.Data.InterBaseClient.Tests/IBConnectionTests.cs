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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
// because of the long pooling tests don't do it twice
//[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBConnectionTests : IBTestsBase
{
	#region Constructors

	public IBConnectionTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void BeginTransactionILUnspecifiedTest() => BeginTransactionILTestsHelper(IsolationLevel.Unspecified);

	[Test]
	public void BeginTransactionILReadCommittedTest() => BeginTransactionILTestsHelper(IsolationLevel.ReadCommitted);

	[Test]
	public void BeginTransactionILReadUncommittedTest() => BeginTransactionILTestsHelper(IsolationLevel.ReadUncommitted);

	[Test]
	public void BeginTransactionILRepeatableReadTest() => BeginTransactionILTestsHelper(IsolationLevel.RepeatableRead);

	[Test]
	public void BeginTransactionILSerializableTest() => BeginTransactionILTestsHelper(IsolationLevel.Serializable);

	[Test]
	public void BeginTransactionNoWaitTimeoutTest()
	{
		using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			conn.Open();
			var tx = conn.BeginTransaction(new IBTransactionOptions() { WaitTimeout = null });
			Assert.NotNull(tx);
			tx.Rollback();
		}
	}

	[Test]
	public void BeginTransactionWithWaitTimeoutTest()
	{
		using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			conn.Open();
			var tx = conn.BeginTransaction(new IBTransactionOptions() { WaitTimeout = TimeSpan.FromSeconds(10) });
			Assert.NotNull(tx);
			tx.Rollback();
		}
	}

	[Test]
	public void BeginTransactionWithWaitTimeoutInvalidValue1Test()
	{
		using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			conn.Open();
			Assert.Throws<ArgumentException>(() => conn.BeginTransaction(new IBTransactionOptions() { WaitTimeout = TimeSpan.FromDays(9999) }));
		}
	}

	[Test]
	public void BeginTransactionWithWaitTimeoutInvalidValue2Test()
	{
		using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			conn.Open();
			Assert.Throws<ArgumentException>(() => conn.BeginTransaction(new IBTransactionOptions() { WaitTimeout = TimeSpan.FromMilliseconds(1) }));
		}
	}

	[Test]
	public void ConnectionWithTruncateChar()
	{
		var cs = BuildConnectionStringBuilder(IBServerType);
		cs.TruncateChar = true;
		var conn = new IBConnection(cs.ToString());
		Assert.AreEqual(true, conn.ConnectionOptions.TruncateChar, "TruncateChar options not true");
		conn.Open();
		Assert.AreEqual(true, conn.InnerConnection.Database.TruncateChar, "Database's TruncateChar not true");
		conn.Close();
		conn.TruncateChar = false;
		conn.Open();
		Assert.AreEqual(false, conn.ConnectionOptions.TruncateChar, "TruncateChar options not false");
		Assert.AreEqual(false, conn.InnerConnection.Database.TruncateChar, "Database's TruncateChar not false");
	}

	[Test]
	public void CreateCommandTest()
	{
		var command = Connection.CreateCommand();

		Assert.AreEqual(command.Connection, Connection);
	}

	[Test]
	public void ConnectionPoolingOnTest()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 5;
		var cs = csb.ToString();

		var active = GetActiveConnections();

		using (IBConnection
			myConnection1 = new IBConnection(cs),
			myConnection2 = new IBConnection(cs))
		{
			myConnection1.Open();
			myConnection2.Open();

			Assert.AreEqual(active + 2, GetActiveConnections());
		}

		Assert.AreEqual(active + 2, GetActiveConnections());
	}

	[Test]
	public void ConnectionPoolingOffTest()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = false;
		csb.ConnectionLifeTime = 5;
		var cs = csb.ToString();

		var active = GetActiveConnections();

		using (IBConnection
			myConnection1 = new IBConnection(cs),
			myConnection2 = new IBConnection(cs))
		{
			myConnection1.Open();
			myConnection2.Open();

			Assert.AreEqual(active + 2, GetActiveConnections());
		}

		Assert.AreEqual(active, GetActiveConnections());
	}

	[Test]
	public void ConnectionPoolingLifetimeTest()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 5;
		var cs = csb.ToString();

		var active = GetActiveConnections();

		using (IBConnection
			myConnection1 = new IBConnection(cs),
			myConnection2 = new IBConnection(cs))
		{
			myConnection1.Open();
			myConnection2.Open();

			Assert.AreEqual(active + 2, GetActiveConnections());
		}

		Thread.Sleep(TimeSpan.FromSeconds(csb.ConnectionLifeTime * 2));
		Assert.AreEqual(active, GetActiveConnections());
	}

	[Test]
	public void ConnectionPoolingMaxPoolSizeTest()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 120;
		csb.MaxPoolSize = 10;
		var cs = csb.ToString();

		var connections = new List<IBConnection>();
		try
		{
			for (var i = 0; i <= csb.MaxPoolSize; i++)
			{
				var connection = new IBConnection(cs);
				connections.Add(connection);
				if (i == csb.MaxPoolSize)
				{
					Assert.Throws<InvalidOperationException>(() => connection.Open());
				}
				else
				{
					Assert.DoesNotThrow(() => connection.Open());
				}
			}
		}
		finally
		{
			connections.ForEach(x => x.Dispose());
		}
	}

	[Test]
	public void ConnectionPoolingMinPoolSizeTest()
	{
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 5;
		csb.MinPoolSize = 3;
		var cs = csb.ToString();

		var active = GetActiveConnections();

		var connections = new List<IBConnection>();
		try
		{
			for (var i = 0; i < csb.MinPoolSize * 2; i++)
			{
				var connection = new IBConnection(cs);
				connections.Add(connection);
				Assert.DoesNotThrow(() => connection.Open());
			}
		}
		finally
		{
			connections.ForEach(x => x.Dispose());
		}

		Thread.Sleep(TimeSpan.FromSeconds(csb.ConnectionLifeTime * 2));
		Assert.AreEqual(active + csb.MinPoolSize, GetActiveConnections());
	}

	[Test]
	public void ConnectionPoolingFailedNewConnectionIsNotBlockingPool()
	{
		const int Size = 2;

		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 100;
		csb.MaxPoolSize = Size;
		csb.Database = "invalid";
		var cs = csb.ToString();

		var retries = 0;
		while (true)
		{
			using (var connection = new IBConnection(cs))
			{
				try
				{
					connection.Open();
				}
				catch (IBException)
				{
					if (retries++ >= Size)
					{
						Assert.Pass();
						return;
					}
					else
					{
						continue;
					}
				}
				Assert.Fail();
			}
		}
	}

	[Test, Explicit]
	public void DoNotGoBackToPoolAfterBroken()
	{
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		using (var conn = new IBConnection(csb.ToString()))
		{
			conn.Open();
		}
		using (var conn = new IBConnection(csb.ToString()))
		{
			conn.Open();
			try
			{
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select * from rdb$statements union all select * from rdb$statements";
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{ }
					}
				}
			}
			catch (IBException)
			{ }
		}
	}

	// Dialect 1 client to a Dialect 3 database should not accept this as per Sriram
	[Test]
	[TestCase(3)]
	public void DialectAndDATE(int dialect)
	{
		IBConnectionStringBuilder builder = IBTestsBase.BuildConnectionStringBuilder(IBServerType);
		builder.Dialect = dialect;
		builder.Charset = "none";
		var cs = builder.ToString();
		using (var connection = new IBConnection(cs))
		{
			connection.Open();
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "CREATE PROCEDURE TEST_PROCEDURE (TEST_ARG DATE) AS BEGIN EXIT; END";
				Assert.DoesNotThrow(() => cmd.Prepare());
			}
		}
	}

	#endregion

	#region Async Unit Tests

	[Test]
	public Task BeginTransactionILUnspecifiedTestAsync() => BeginTransactionILTestsHelperAsync(IsolationLevel.Unspecified);

	[Test]
	public Task BeginTransactionILReadCommittedTestAsync() => BeginTransactionILTestsHelperAsync(IsolationLevel.ReadCommitted);

	[Test]
	public Task BeginTransactionILReadUncommittedTestAsync() => BeginTransactionILTestsHelperAsync(IsolationLevel.ReadUncommitted);

	[Test]
	public Task BeginTransactionILRepeatableReadTestAsync() => BeginTransactionILTestsHelperAsync(IsolationLevel.RepeatableRead);

	[Test]
	public Task BeginTransactionILSerializableTestAsync() => BeginTransactionILTestsHelperAsync(IsolationLevel.Serializable);

	[Test]
	public async Task BeginTransactionNoWaitTimeoutTestAsync()
	{
		await using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			await conn.OpenAsync();
			await using (var tx = await conn.BeginTransactionAsync(new IBTransactionOptions() { WaitTimeout = null }))
			{
				Assert.NotNull(tx);
				await tx.RollbackAsync();
			}
		}
	}

	[Test]
	public async Task BeginTransactionWithWaitTimeoutTestAsync()
	{
		await using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			await conn.OpenAsync();
			await using (var tx = await conn.BeginTransactionAsync(new IBTransactionOptions() { WaitTimeout = TimeSpan.FromSeconds(10) }))
			{
				Assert.NotNull(tx);
				await tx.RollbackAsync();
			}
		}
	}

	[Test]
	public async Task BeginTransactionWithWaitTimeoutInvalidValue1TestAsync()
	{
		await using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			await conn.OpenAsync();
			Assert.ThrowsAsync<ArgumentException>(() => conn.BeginTransactionAsync(new IBTransactionOptions() { WaitTimeout = TimeSpan.FromDays(9999) }));
		}
	}

	[Test]
	public async Task BeginTransactionWithWaitTimeoutInvalidValue2TestAsync()
	{
		await using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			await conn.OpenAsync();
			Assert.ThrowsAsync<ArgumentException>(() => conn.BeginTransactionAsync(new IBTransactionOptions() { WaitTimeout = TimeSpan.FromMilliseconds(1) }));
		}
	}

	[Test]
	public async Task ConnectionWithTruncateCharAsync()
	{
		var cs = BuildConnectionStringBuilder(IBServerType);
		cs.TruncateChar = true;
		var conn = new IBConnection(cs.ToString());
		Assert.AreEqual(true, conn.ConnectionOptions.TruncateChar, "TruncateChar options not true");
		await conn.OpenAsync();
		Assert.AreEqual(true, conn.InnerConnection.Database.TruncateChar, "Database's TruncateChar not true");
		await conn.CloseAsync();
		conn.TruncateChar = false;
		await conn.OpenAsync();
		Assert.AreEqual(false, conn.ConnectionOptions.TruncateChar, "TruncateChar options not false");
		Assert.AreEqual(false, conn.InnerConnection.Database.TruncateChar, "Database's TruncateChar not false");
	}

	[Test]
	public async Task CreateCommandTestAsync()
	{
		await using (var command = Connection.CreateCommand())
		{
			Assert.AreEqual(command.Connection, Connection);
		}
	}

	[Test]
	public async Task ConnectionPoolingOnTestAsync()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 5;
		var cs = csb.ToString();

		var active = await GetActiveConnectionsAsync();

		await using (IBConnection
			myConnection1 = new IBConnection(cs),
			myConnection2 = new IBConnection(cs))
		{
			await myConnection1.OpenAsync();
			await myConnection2.OpenAsync();

			Assert.AreEqual(active + 2, await GetActiveConnectionsAsync());
		}

		Assert.AreEqual(active + 2, await GetActiveConnectionsAsync());
	}

	[Test]
	public async Task ConnectionPoolingOffTestAsync()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = false;
		csb.ConnectionLifeTime = 5;
		var cs = csb.ToString();

		var active = await GetActiveConnectionsAsync();

		await using (IBConnection
			myConnection1 = new IBConnection(cs),
			myConnection2 = new IBConnection(cs))
		{
			await myConnection1.OpenAsync();
			await myConnection2.OpenAsync();

			Assert.AreEqual(active + 2, await GetActiveConnectionsAsync());
		}

		Assert.AreEqual(active, await GetActiveConnectionsAsync());
	}

	[Test]
	public async Task ConnectionPoolingLifetimeTestAsync()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 5;
		var cs = csb.ToString();

		var active = await GetActiveConnectionsAsync();

		await using (IBConnection
			myConnection1 = new IBConnection(cs),
			myConnection2 = new IBConnection(cs))
		{
			await myConnection1.OpenAsync();
			await myConnection2.OpenAsync();

			Assert.AreEqual(active + 2, await GetActiveConnectionsAsync());
		}

		Thread.Sleep(TimeSpan.FromSeconds(csb.ConnectionLifeTime * 2));
		Assert.AreEqual(active, await GetActiveConnectionsAsync());
	}

	[Test]
	public async Task ConnectionPoolingMaxPoolSizeTestAsync()
	{
		IBConnection.ClearAllPools();
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 120;
		csb.MaxPoolSize = 10;
		var cs = csb.ToString();

		var connections = new List<IBConnection>();
		try
		{
			for (var i = 0; i <= csb.MaxPoolSize; i++)
			{
				var connection = new IBConnection(cs);
				connections.Add(connection);
				if (i == csb.MaxPoolSize)
				{
					Assert.ThrowsAsync<InvalidOperationException>(() => connection.OpenAsync());
				}
				else
				{
					Assert.DoesNotThrowAsync(() => connection.OpenAsync());
				}
			}
		}
		finally
		{
			foreach (var c in connections)
				await c.DisposeAsync();
		}
	}

	[Test]
	public async Task ConnectionPoolingMinPoolSizeTestAsync()
	{
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 5;
		csb.MinPoolSize = 3;
		var cs = csb.ToString();

		var active = await GetActiveConnectionsAsync();

		var connections = new List<IBConnection>();
		try
		{
			for (var i = 0; i < csb.MinPoolSize * 2; i++)
			{
				var connection = new IBConnection(cs);
				connections.Add(connection);
				Assert.DoesNotThrowAsync(() => connection.OpenAsync());
			}
		}
		finally
		{
			foreach (var c in connections)
				await c.DisposeAsync();
		}

		Thread.Sleep(TimeSpan.FromSeconds(csb.ConnectionLifeTime * 2));
		Assert.AreEqual(active + csb.MinPoolSize, await GetActiveConnectionsAsync());
	}

	[Test]
	public async Task ConnectionPoolingFailedNewConnectionIsNotBlockingPoolAsync()
	{
		const int Size = 2;

		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		csb.ConnectionLifeTime = 100;
		csb.MaxPoolSize = Size;
		csb.Database = "invalid";
		var cs = csb.ToString();

		var retries = 0;
		while (true)
		{
			await using (var connection = new IBConnection(cs))
			{
				try
				{
					await connection.OpenAsync();
				}
				catch (IBException)
				{
					if (retries++ >= Size)
					{
						Assert.Pass();
						return;
					}
					else
					{
						continue;
					}
				}
				Assert.Fail();
			}
		}
	}

	[Test, Explicit]
	public async Task DoNotGoBackToPoolAfterBrokenAsync()
	{
		var csb = BuildConnectionStringBuilder(IBServerType);
		csb.Pooling = true;
		await using (var conn = new IBConnection(csb.ToString()))
		{
			await conn.OpenAsync();
		}
		await using (var conn = new IBConnection(csb.ToString()))
		{
			await conn.OpenAsync();
			try
			{
				await using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select * from mon$statements union all select * from mon$statements";
					await using (var reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{ }
					}
				}
			}
			catch (IBException)
			{ }
		}
	}

	// Dialect 1 client to a Dialect 3 database should not accept this as per Sriram
	[Test]
	[TestCase(3)]
	public async Task DialectAndDATEAsync(int dialect)
	{
		IBConnectionStringBuilder builder = IBTestsBase.BuildConnectionStringBuilder(IBServerType);
		builder.Dialect = dialect;
		builder.Charset = "none";
		var cs = builder.ToString();
		using (var connection = new IBConnection(cs))
		{
			await connection.OpenAsync();
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "CREATE PROCEDURE TEST_PROCEDURE (TEST_ARG DATE) AS BEGIN EXIT; END";
				Assert.DoesNotThrowAsync(() => cmd.PrepareAsync());
			}
		}
	}
	#endregion

	#region Methods

	public IBTransaction BeginTransaction(IsolationLevel level)
	{
		switch (level)
		{
			case IsolationLevel.Unspecified:
				return Connection.BeginTransaction();

			default:
				return Connection.BeginTransaction(level);
		}
	}

	private void BeginTransactionILTestsHelper(IsolationLevel level)
	{
		using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			conn.Open();
			var tx = conn.BeginTransaction(level);
			Assert.NotNull(tx);
			tx.Rollback();
		}
	}

	private async Task BeginTransactionILTestsHelperAsync(IsolationLevel level)
	{
		await using (var conn = new IBConnection(BuildConnectionString(IBServerType)))
		{
			await conn.OpenAsync();
			await using (var tx = await conn.BeginTransactionAsync(level))
			{
				Assert.NotNull(tx);
				await tx.RollbackAsync();
			}
		}
	}

	private int GetLogRowsCount(IBConnection conn)
	{
		using (var cmd = conn.CreateCommand())
		{
			cmd.CommandText = "select count(*) from log where text = 'on connect'";
			return Convert.ToInt32(cmd.ExecuteScalar());
		}
	}
	#endregion
}

public class IBConnectionTestsDialect1 : IBConnectionTests
{
	private IBServerType _serverType;
	private bool eventFired;

	public IBConnectionTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
		_serverType = serverType;
	}

	[Test]
	[TestCase(1)]
	[TestCase(3)]
	public void DialectScaleback(int dialect)
	{
		IBConnectionStringBuilder builder = IBTestsBase.BuildConnectionStringBuilder(_serverType);
		builder.Dialect = dialect;
		builder.Charset = "none";
		var cs = builder.ToString();
		using (var connection = new IBConnection(cs))
		{
			connection.Open();
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "select cast(int_field as numeric(10,2)) from test";
				Assert.DoesNotThrow(() => cmd.Prepare());
			}
		}
	}


	[Test]
	public void DialectScalebackEvent()
	{
		IBConnectionStringBuilder builder = IBTestsBase.BuildConnectionStringBuilder(_serverType);
		builder.Dialect = 3;
		builder.Charset = "none";
		var cs = builder.ToString();
		using (var connection = new IBConnection(cs))
		{
			eventFired = false;
			connection.DialectDowngradeWarning += Connection_DialectDowngradeWarning;
			connection.Open();
			Assert.AreEqual(true, eventFired);
		}

	}

	[Test]
	[TestCase(1)]
	[TestCase(3)]
	public async Task DialectScalebackAsync(int dialect)
	{
		IBConnectionStringBuilder builder = IBTestsBase.BuildConnectionStringBuilder(_serverType);
		builder.Dialect = dialect;
		builder.Charset = "none";
		var cs = builder.ToString();
		using (var connection = new IBConnection(cs))
		{
			await connection.OpenAsync();
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "select cast(int_field as numeric(10,2)) from test";
				Assert.DoesNotThrowAsync(() => cmd.PrepareAsync());
			}
		}
	}


	[Test]
	public async Task DialectScalebackEventAsync()
	{
		IBConnectionStringBuilder builder = IBTestsBase.BuildConnectionStringBuilder(_serverType);
		builder.Dialect = 3;
		builder.Charset = "none";
		var cs = builder.ToString();
		using (var connection = new IBConnection(cs))
		{
			eventFired = false;
			connection.DialectDowngradeWarning += Connection_DialectDowngradeWarning;
			await connection.OpenAsync();
			Assert.AreEqual(true, eventFired);
		}

	}
	private void Connection_DialectDowngradeWarning(object sender, int dbDialect)
	{
		eventFired = true;
	}
}
