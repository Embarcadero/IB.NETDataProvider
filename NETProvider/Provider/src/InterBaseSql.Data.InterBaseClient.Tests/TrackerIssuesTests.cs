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
using System.Diagnostics;
using System.Text;
using System.Threading;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class TrackerIssuesTests : IBTestsBase
	{
		#region Constructors

		public TrackerIssuesTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region Unit Tests

		[Test]
		public void DNET217_ReadingALotOfFields()
		{
			var cols = new StringBuilder();
			var separator = string.Empty;
			for (var i = 0; i < 1235; i++)
			{
				if (i % 2 == 0)
					cols.AppendFormat("{0}'r' as col{1}", separator, i);
				else
					cols.AppendFormat("{0}24 as col{1}", separator, i);

				separator = ",";
			}
			using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = "select " + cols.ToString() + " from rdb$database where 'x' = @x or 'x' = @x and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y and current_timestamp = @y";
				cmd.Parameters.Add(new IBParameter() { ParameterName = "@x", Value = "z" });
				cmd.Parameters.Add(new IBParameter() { ParameterName = "@y", Value = DateTime.Now });
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{ }
				}
			}
		}

		[Test]
		public void DNET260_ProcedureWithALotOfParameters()
		{
			using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = "TEST_SP";
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new IBParameter() { Value = 1 });
				cmd.Parameters.Add(new IBParameter() { Value = 1 });
				cmd.Parameters.Add(new IBParameter() { Value = 1 });
				cmd.Parameters.Add(new IBParameter() { Value = 1 });
				cmd.Parameters.Add(new IBParameter() { Value = 1 });
				cmd.Parameters.Add(new IBParameter() { Value = 1 });
				cmd.Parameters.Add(new IBParameter() { Value = DateTime.Today });
				cmd.Parameters.Add(new IBParameter() { Value = DateTime.Today });

				cmd.ExecuteNonQuery();
			}
		}

		[Test]
		public void DNET273_WritingClobAsBinary()
		{
			using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = "insert into test (INT_FIELD, CLOB_FIELD) values (@INT_FIELD, @CLOB_FIELD)";
				cmd.Parameters.Add("@INT_FIELD", IBDbType.Integer).Value = 100;
				cmd.Parameters.Add("@CLOB_FIELD", IBDbType.Binary).Value = new byte[] { 0x00, 0x001 };
				cmd.ExecuteNonQuery();
			}
		}

		//[Test]
		//public void DNET595_ProperConnectionPoolConnectionsClosing()
		//{
		//	IBConnection.ClearAllPools();
		//	const int NumberOfThreads = 15;

		//	var csb = BuildConnectionStringBuilder(IBServerType);
		//	csb.Pooling = true;
		//	csb.ConnectionLifeTime = 1;
		//	csb.MinPoolSize = 0;
		//	var cs = csb.ToString();

		//	var active = GetActiveConnections();

		//	var threads = new List<Thread>();
		//	for (var i = 0; i < NumberOfThreads; i++)
		//	{
		//		var t = new Thread(o =>
		//		{
		//			for (var j = 0; j < 50; j++)
		//			{
		//				GetSomething(cs);
		//			}
		//		});
		//		t.IsBackground = true;
		//		t.Start();
		//		threads.Add(t);
		//	}
		//	foreach (var thread in threads)
		//	{
		//		thread.Join();
		//	}

		//	Assert.Greater(GetActiveConnections(), active);

		//	var sw = new Stopwatch();
		//	sw.Start();
		//	while (sw.Elapsed.TotalSeconds < 60)
		//	{
		//		GetSomething(cs);
		//	}

		//	Thread.Sleep(TimeSpan.FromSeconds(csb.ConnectionLifeTime * 2));
		//	Assert.AreEqual(active, GetActiveConnections());
		//}

		[Test]
		public void DNET313_MultiDimensionalArray()
		{
			using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = @"
CREATE TABLE TABMAT (
    ID INTEGER NOT NULL,
	MATRIX INTEGER[1:3, 1:4]
)";
				cmd.ExecuteNonQuery();
			}
			try
			{
				var sql = "INSERT INTO TabMat (Id,Matrix) Values(@ValId,@ValMat)";
				int[,] mat = { { 1, 2, 3, 4 }, { 10, 20, 30, 40 }, { 101, 102, 103, 104 } };
				var random = new Random();
				using (var tx = Connection.BeginTransaction())
				{
					using (var cmd = new IBCommand(sql, Connection, tx))
					{
						cmd.Parameters.Add("@ValId", IBDbType.Integer).Value = random.Next();
						cmd.Parameters.Add("@ValMat", IBDbType.Array).Value = mat;
						cmd.ExecuteNonQuery();
					}
					tx.Commit();
				}
				using (var cmd = Connection.CreateCommand())
				{
					cmd.CommandText = @"select matrix from tabmat";
					using (var reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							Assert.AreEqual(mat, reader[0]);
						}
						else
						{
							Assert.Fail();
						}
					}
				}
			}
			finally
			{
				using (var cmd = Connection.CreateCommand())
				{
					cmd.CommandText = "drop table tabmat";
					cmd.ExecuteNonQuery();
				}
			}
		}

		[Test]
		public void DNET304_VarcharOctetsParameterRoundtrip()
		{
			var data = new byte[] { 10, 20 };
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Parameters.Add(new IBParameter() { ParameterName = "@x", Value = data });
				cmd.CommandText = "select cast(oct as varchar(10) character set octets) from Octets(@x)";
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Assert.AreEqual(data, reader[0]);
					}
				}
			}
		}

		[Test]
		public void DNET304_CharOctetsParameterRoundtrip()
		{
			var data = new byte[] { 10, 20 };
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Parameters.Add(new IBParameter() { ParameterName = "@x", Value = data });
				cmd.CommandText = "select cast(oct as char(10)) from Octets(@x)";
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Assert.AreEqual(new byte[] { data[0], data[1], 32, 32, 32, 32, 32, 32, 32, 32 }, reader[0]);
					}
				}
			}
		}

		#endregion

		#region Methods

		private static void GetSomething(string connectionString)
		{
			using (var conn = new IBConnection(connectionString))
			{
				conn.Open();
				using (var command = new IBCommand("select current_timestamp from rdb$database", conn))
				{
					command.ExecuteScalar();
				}
			}
		}

		#endregion
	}
	public class TrackerIssuesTestsDialect1 : TrackerIssuesTests
	{
		public TrackerIssuesTestsDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1;
		}
	}
}
