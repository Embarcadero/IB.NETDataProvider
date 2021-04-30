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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class IBConnectionTests : IBTestsBase
	{
		#region Constructors

		public IBConnectionTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region Unit Tests

		[Test]
		public void BeginTransactionILUnspecifiedTest()
		{
			BeginTransactionILTestsHelper(IsolationLevel.Unspecified);
		}

		[Test]
		public void BeginTransactionILReadCommittedTest()
		{
			BeginTransactionILTestsHelper(IsolationLevel.ReadCommitted);
		}

		[Test]
		public void BeginTransactionILReadUncommittedTest()
		{
			BeginTransactionILTestsHelper(IsolationLevel.ReadUncommitted);
		}

		[Test]
		public void BeginTransactionILRepeatableReadTest()
		{
			BeginTransactionILTestsHelper(IsolationLevel.RepeatableRead);
		}

		[Test]
		public void BeginTransactionILSerializableTest()
		{
			BeginTransactionILTestsHelper(IsolationLevel.Serializable);
		}

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
}
