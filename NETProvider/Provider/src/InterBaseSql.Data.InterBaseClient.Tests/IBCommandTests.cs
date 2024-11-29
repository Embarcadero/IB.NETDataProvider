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
using InterBaseSql.Data.Common;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBCommandTests : IBTestsBase
{
	#region Constructors

	public IBCommandTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void ExecuteNonQueryTest()
	{
		Transaction = Connection.BeginTransaction();

		var command = Connection.CreateCommand();

		command.Transaction = Transaction;
		command.CommandText = "insert into TEST	(INT_FIELD)	values (?) ";

		command.Parameters.Add("@INT_FIELD", 100);

		var affectedRows = command.ExecuteNonQuery();

		Assert.AreEqual(affectedRows, 1);

		Transaction.Rollback();

		command.Dispose();
	}

	[Test]
	public void ExecuteReaderTest()
	{
		var command = Connection.CreateCommand();

		command.CommandText = "select *	from TEST";

		var reader = command.ExecuteReader();
		reader.Close();

		command.Dispose();
	}

	[Test]
	public void ExecuteMultipleReaderTest()
	{
		var command1 = Connection.CreateCommand();
		var command2 = Connection.CreateCommand();

		command1.CommandText = "select * from test where int_field = 1";
		command2.CommandText = "select * from test where int_field = 2";

		var r1 = command1.ExecuteReader();
		var r2 = command2.ExecuteReader();

		r2.Close();

		try
		{
			// Try to call ExecuteReader in	command1
			// it should throw an exception
			r2 = command1.ExecuteReader();

			throw new InvalidProgramException();
		}
		catch
		{
			r1.Close();
		}
	}

	[Test]
	public void ExecuteReaderWithBehaviorTest()
	{
		var command = new IBCommand("select *	from TEST", Connection);

		var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
		reader.Close();

		command.Dispose();
	}

	[Test]
	public void ExecuteScalarTest()
	{
		var command = Connection.CreateCommand();

		command.CommandText = "select CHAR_FIELD from TEST where INT_FIELD = ?";
		command.Parameters.Add("@INT_FIELD", 2);

		var charFieldValue = command.ExecuteScalar().ToString();

		Assert.AreEqual("IRow 2", charFieldValue.TrimEnd(' '));

		command.Dispose();
	}

	[Test]
	public void ExecuteScalarWithStoredProcedureTest()
	{
		var command = Connection.CreateCommand();

		command.CommandText = "SimpleSP";
		command.CommandType = CommandType.StoredProcedure;

		var result = (int)command.ExecuteScalar();

		Assert.AreEqual(1000, result);

		command.Dispose();
	}

	[Test]
	public void PrepareTest()
	{
		var command = new IBCommand("insert into PrepareTest(test_field) values(@test_field);", Connection);

		command.Parameters.Add("@test_field", IBDbType.VarChar).Value = DBNull.Value;
		command.Prepare();

		for (var i = 0; i < 5; i++)
		{
			if (i < 1)
			{
				command.Parameters[0].Value = DBNull.Value;
			}
			else
			{
				command.Parameters[0].Value = i.ToString();
			}
			command.ExecuteNonQuery();
		}

		command.Dispose();

		var select = new IBCommand("select * from PrepareTest", Connection);
		var reader = select.ExecuteReader();
		var count = 0;
		while (reader.Read())
		{
			if (count == 0)
			{
				Assert.AreEqual(DBNull.Value, reader[0], "Invalid value.");
			}
			else
			{
				Assert.AreEqual(count, reader.GetInt32(0), "Invalid	value.");
			}

			count++;
		}
		reader.Close();
		select.Dispose();
	}

	[Test]
	public void NamedParametersTest()
	{
		var command = Connection.CreateCommand();

		command.CommandText = "select CHAR_FIELD from TEST where INT_FIELD = @int_field	or CHAR_FIELD =	@char_field";

		command.Parameters.Add("@int_field", 2);
		command.Parameters.Add("@char_field", "TWO");

		var reader = command.ExecuteReader();

		var count = 0;

		while (reader.Read())
		{
			count++;
		}

		Assert.AreEqual(1, count, "Invalid number of records fetched.");

		reader.Close();
		command.Dispose();
	}

	[Test]
	public void NamedParametersAndLiterals()
	{
		var sql = "update test set char_field = 'carlos@firebird.org', bigint_field = @bigint, varchar_field	= 'carlos@ado.net' where int_field = @integer";

		var command = new IBCommand(sql, Connection);
		command.Parameters.Add("@bigint", IBDbType.BigInt).Value = 200;
		command.Parameters.Add("@integer", IBDbType.Integer).Value = 1;

		var recordsAffected = command.ExecuteNonQuery();

		command.Dispose();

		Assert.AreEqual(recordsAffected, 1, "Invalid number	of records affected.");
	}

	[Test]
	public void NamedParametersReuseTest()
	{
		var sql = "select * from	test where int_field >=	@lang and int_field	<= @lang";

		var command = new IBCommand(sql, Connection);
		command.Parameters.Add("@lang", IBDbType.Integer).Value = 10;

		var reader = command.ExecuteReader();

		var count = 0;
		var intValue = 0;

		while (reader.Read())
		{
			if (count == 0)
			{
				intValue = reader.GetInt32(0);
			}
			count++;
		}

		Assert.AreEqual(1, count, "Invalid number of records fetched.");
		Assert.AreEqual(10, intValue, "Invalid record fetched.");

		reader.Close();
		command.Dispose();
	}

	[Test]
	public void ExecuteStoredProcTest()
	{
		var command = new IBCommand("EXECUTE PROCEDURE GETVARCHARFIELD(?)", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters.Add("@VARCHAR_FIELD", IBDbType.VarChar).Direction = ParameterDirection.Output;

		command.Parameters[0].Value = 1;

		command.ExecuteNonQuery();

		Assert.AreEqual("IRow Number 1", command.Parameters[1].Value);
	}

	[Test]
	public void RecordAffectedTest()
	{
		var sql = "insert into test (int_field) values (100000)";

		var command = new IBCommand(sql, Connection);

		var reader = command.ExecuteReader();

		Assert.AreEqual(1, reader.RecordsAffected);

		while (reader.Read())
		{
		}

		reader.Close();

		Assert.AreEqual(1, reader.RecordsAffected);
	}

	[Test]
	public void ExecuteNonQueryWithOutputParameters()
	{
		var command = new IBCommand("EXECUTE PROCEDURE GETASCIIBLOB(?)", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters.Add("@CLOB_FIELD", IBDbType.Text).Direction = ParameterDirection.Output;

		command.Parameters[0].Value = 1;

		command.ExecuteNonQuery();

		Assert.AreEqual("IRow Number 1", command.Parameters[1].Value, "Output parameter value is not valid");

		command.Dispose();
	}

	[Test]
	public void InvalidParameterFormat()
	{
		var sql = "update test set timestamp_field =	@timestamp where int_field = @integer";

		var transaction = Connection.BeginTransaction();
		try
		{
			var command = new IBCommand(sql, Connection, transaction);
			command.Parameters.Add("@timestamp", IBDbType.TimeStamp).Value = 1;
			command.Parameters.Add("@integer", IBDbType.Integer).Value = 1;

			command.ExecuteNonQuery();

			command.Dispose();

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
		}
	}

	[Test]
	public void UnicodeTest()
	{
		try
		{
			using (var create = new IBCommand("CREATE TABLE VARCHARTEST (VARCHAR_FIELD  VARCHAR(10))", Connection))
			{
				create.ExecuteNonQuery();
			}

			var l = new List<string>
			{
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('1')",
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('11')",
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('111')",
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('1111')"
			};

			foreach (string statement in l)
			{
				using (var insert = new IBCommand(statement, Connection))
				{
					insert.ExecuteNonQuery();
				}
			}

			var sql = "select * from	varchartest";

			using (var cmd = new IBCommand(sql, Connection))
			{
				using (var r = cmd.ExecuteReader())
				{
					while (r.Read())
					{
						var dummy = r[0];
					}
				}
			}
		}
		finally
		{
			using (var drop = new IBCommand("DROP TABLE VARCHARTEST", Connection))
			{
				drop.ExecuteNonQuery();
			}
		}
	}

	[Test]
	public void SimplifiedChineseTest()
	{
		const string value = "中文";
		try
		{
			using (var cmd = new IBCommand("create table table1 (field1 varchar(20))", Connection))
			{
				cmd.ExecuteNonQuery();
			}

			using (var cmd = new IBCommand("insert into table1 values (@value)", Connection))
			{
				cmd.Parameters.Add("@value", IBDbType.VarChar).Value = value;
				cmd.ExecuteNonQuery();
			}
			using (var cmd = new IBCommand($"insert into table1 values ('{value}')", Connection))
			{
				cmd.ExecuteNonQuery();
			}

			using (var cmd = new IBCommand("select * from table1", Connection))
			{
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Assert.AreEqual(value, reader[0]);
					}
				}
			}
		}
		finally
		{
			using (var cmd = new IBCommand("drop table table1", Connection))
			{
				cmd.ExecuteNonQuery();
			}
		}
	}

	[Test]
	public void InsertDateTest()
	{
		var sql = "insert into TEST (int_field, date_field) values (1002, @date)";

		var command = new IBCommand(sql, Connection);

		command.Parameters.Add("@date", IBDbType.Date).Value = DateTime.Now.ToString();

		var ra = command.ExecuteNonQuery();

		Assert.AreEqual(ra, 1);
	}

	[Test]
	public void InsertNullTest()
	{
		var sql = "insert into TEST (int_field) values (@value)";

		var command = new IBCommand(sql, Connection);
		command.Parameters.Add("@value", IBDbType.Integer).Value = null;

		try
		{
			command.ExecuteNonQuery();
			Assert.Fail("The command was executed without throwing an exception");
		}
		catch
		{ }
	}

	[Test]
	public void InsertDateTimeTest()
	{
		var value = DateTime.Now;

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, timestamp_field) values (1002, @dt)";
			cmd.Parameters.Add("@dt", IBDbType.TimeStamp).Value = value;

			var ra = cmd.ExecuteNonQuery();

			Assert.AreEqual(ra, 1);
		}

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select timestamp_field from test where int_field = 1002";
			var result = (DateTime)cmd.ExecuteScalar();

			Assert.AreEqual(value.ToString(), result.ToString());
		}
	}

	[Test]
	public void InsertTimeStampTest()
	{
		var value = DateTime.Now.ToString();

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, timestamp_field) values (1002, @ts)";
			cmd.Parameters.Add("@ts", IBDbType.TimeStamp).Value = value;

			var ra = cmd.ExecuteNonQuery();

			Assert.AreEqual(ra, 1);
		}

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select timestamp_field from test where int_field = 1002";
			var result = (DateTime)cmd.ExecuteScalar();

			Assert.AreEqual(value, result.ToString());
		}
	}

	[Test]
	public virtual void InsertTimeTest()
	{
		var t = new TimeSpan(0, 5, 6, 7, 231);

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, time_field) values (2245, @t)";
			if (IBTestsSetup.Dialect == 3)
				cmd.Parameters.Add("@t", IBDbType.Time).Value = t;
			else
				cmd.Parameters.Add("@t", IBDbType.TimeStamp).Value = t;

			var ra = cmd.ExecuteNonQuery();

			Assert.AreEqual(ra, 1);
		}

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select time_field from test where int_field = 2245";
			TimeSpan result;
			if (IBTestsSetup.Dialect == 3)
				result = (TimeSpan)cmd.ExecuteScalar();
			else
				result = ((DateTime)cmd.ExecuteScalar()).TimeOfDay;

			Assert.AreEqual(t.Hours, result.Hours, "hours are not same");
			Assert.AreEqual(t.Minutes, result.Minutes, "minutes are not same");
			Assert.AreEqual(t.Seconds, result.Seconds, "seconds are not same");
			Assert.AreEqual(t.Milliseconds, result.Milliseconds, "milliseconds are not same");
		}
	}

	[Test]
	public void InsertTimeOldTest()
	{
		var t = DateTime.Today;
		t = t.AddHours(5);
		t = t.AddMinutes(6);
		t = t.AddSeconds(7);
		t = t.AddMilliseconds(231);

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, time_field) values (2245, @t)";
			if (IBTestsSetup.Dialect == 3)
				cmd.Parameters.Add("@t", IBDbType.Time).Value = t;
			else
				cmd.Parameters.Add("@t", IBDbType.TimeStamp).Value = t;

			var ra = cmd.ExecuteNonQuery();

			Assert.AreEqual(ra, 1);
		}

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select time_field from test where int_field = 2245";

			TimeSpan result;
			if (IBTestsSetup.Dialect == 3)
				result = (TimeSpan)cmd.ExecuteScalar();
			else
				result = ((DateTime)cmd.ExecuteScalar()).TimeOfDay;


			Assert.AreEqual(t.Hour, result.Hours, "hours are not same");
			Assert.AreEqual(t.Minute, result.Minutes, "minutes are not same");
			Assert.AreEqual(t.Second, result.Seconds, "seconds are not same");
			Assert.AreEqual(t.Millisecond, result.Milliseconds, "milliseconds are not same");
		}
	}

	[Test]
	public void ParameterDescribeTest()
	{
		var sql = "insert into TEST (int_field) values (@value)";

		var command = new IBCommand(sql, Connection);
		command.Prepare();
		command.Parameters.Add("@value", IBDbType.Integer).Value = 100000;

		command.ExecuteNonQuery();

		command.Dispose();
	}

	[Test]
	public void ReadOnlyTransactionTest()
	{
		using (IDbCommand command = Connection.CreateCommand())
		{
			using (IDbTransaction transaction = Connection.BeginTransaction(new IBTransactionOptions() { TransactionBehavior = IBTransactionBehavior.Read }))
			{
				try
				{
					command.Transaction = transaction;
					command.CommandType = System.Data.CommandType.Text;
					command.CommandText = "CREATE TABLE	X_TABLE_1(FIELD	VARCHAR(50));";
					command.ExecuteNonQuery();
					transaction.Commit();
				}
				catch (IBException)
				{
				}
			}
		}
	}

	[Test]
	public void ReadingVarcharOctetsTest()
	{
		using (var cmd = Connection.CreateCommand())
		{
			const string data = "1234";
			byte[] read = null;

			cmd.CommandText = string.Format("select cast('{0}' as varchar(10) character set octets) from rdb$database", data);
			using (var reader = cmd.ExecuteReader())
			{
				if (reader.Read())
				{
					read = (byte[])reader[0];
				}
			}

			var expected = Encoding.ASCII.GetBytes(data);
			Assert.AreEqual(expected, read);
		}
	}

	[Test]
	public void ReadingCharOctetsTest()
	{
		using (var cmd = Connection.CreateCommand())
		{
			const string data = "1234";
			byte[] read = null;

			cmd.CommandText = string.Format("select cast('{0}' as char(10) character set octets) from rdb$database", data);
			using (var reader = cmd.ExecuteReader())
			{
				if (reader.Read())
				{
					read = (byte[])reader[0];
				}
			}

			var expected = new byte[10];
			Encoding.ASCII.GetBytes(data).CopyTo(expected, 0);
			Assert.AreEqual(expected, read);
		}
	}

	[Test]
	public void GetCommandPlanTest()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select * from test";
			cmd.Prepare();
			var plan = default(string);
			Assert.DoesNotThrow(() => { plan = cmd.GetCommandPlan(); });
			Assert.IsNotEmpty(plan);
		}
	}

	[Test]
	public void GetCommandPlanNoPlanTest()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "create table NoCommandPlanTest (id int)";
			cmd.Prepare();
			var plan = default(string);
			Assert.DoesNotThrow(() => { plan = cmd.GetCommandPlan(); });
			Assert.IsEmpty(plan);
		}
	}

	[Test]
	public virtual void ReadsTimeWithProperPrecision()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast('01/12/2023 00:00:01.4321' as timestamp) from rdb$database";

			TimeSpan result;
			result = ((DateTime)cmd.ExecuteScalar()).TimeOfDay;

			Assert.AreEqual(TimeSpan.FromTicks(14321000), result);
		}
	}

	[Test]
	public void ReadsDateTimeWithProperPrecision()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast('1.2.2015 05:06:01.4321' as timestamp) from rdb$database";
			var result = (DateTime)cmd.ExecuteScalar();
			Assert.AreEqual(new DateTime(635583639614321000), result);
		}
	}

	[Test]
	public void PassesDateTimeWithProperPrecision()
	{
		var dt = new DateTime(635583639614321000);
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@value as timestamp) from rdb$database";
			cmd.Parameters.Add("value", dt);
			var result = (DateTime)cmd.ExecuteScalar();
			Assert.AreEqual(dt, result);
		}
	}

	[Test]
	public virtual void PassesTimeSpanWithProperPrecision()
	{
		var ts = TimeSpan.FromTicks(14321000);
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@value as time) from rdb$database";
			cmd.Parameters.Add("value", ts);
			var result = (TimeSpan)cmd.ExecuteScalar();
			Assert.AreEqual(ts, result);
		}
	}

	[Test]
	public void ExecuteNonQueryReturnsMinusOneOnNonInsertUpdateDelete()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select 1 from rdb$database";
			var ra = cmd.ExecuteNonQuery();
			Assert.AreEqual(-1, ra);
		}
	}

	internal void FetchRow(object sender, FetchEventArgs e)
	{
		Assert.AreEqual(2, e.Values.Length);
		Assert.AreEqual(1, e.Values[0].GetInt32(), "First value wrong should be 1 was " + e.Values[0].GetInt32().ToString());
		Assert.AreEqual("hello", e.Values[1].GetString(), "First value wrong should be 'hello' was " + e.Values[1].GetString());
	}

	[Test]
	public void ExecuteNonQueryOnFetch()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select 1, 'hello' from rdb$database";
			cmd.FetchEvent += FetchRow;
			var ra = cmd.ExecuteNonQuery();
		}
	}

#if NET6_0_OR_GREATER
	[Test]
	public void PassDateOnly()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@x as date) from rdb$database";
			cmd.Parameters.Add("x", new DateOnly(2021, 11, 29));
			var value = (DateTime)cmd.ExecuteScalar();
			Assert.AreEqual(2021, value.Year);
			Assert.AreEqual(11, value.Month);
			Assert.AreEqual(29, value.Day);
		}
	}

	[Test]
	public void PassTimeOnly()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@x as time) from rdb$database";
			cmd.Parameters.Add("x", new TimeOnly(501940213000));
			var value = (TimeSpan)cmd.ExecuteScalar();
			Assert.AreEqual(501940213000, value.Ticks);
		}
	}
#endif

	#endregion

	#region Async Unit Tests

	[Test]
	public async Task ExecuteNonQueryTestAsync()
	{
		await using (var Transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = Connection.CreateCommand())
			{
				command.Transaction = Transaction;
				command.CommandText = "insert into TEST (INT_FIELD) values (?)";
				command.Parameters.Add("@INT_FIELD", 100);
				var affectedRows = await command.ExecuteNonQueryAsync();
				Assert.AreEqual(affectedRows, 1);
				await Transaction.RollbackAsync();
			}
		}
	}

	[Test]
	public async Task ExecuteReaderTestAsync()
	{
		await using (var command = Connection.CreateCommand())
		{
			command.CommandText = "select * from TEST";
			await using (var reader = await command.ExecuteReaderAsync())
			{ }
		}
	}

	[Test]
	public async Task ExecuteMultipleReaderTestAsync()
	{
		await using (IBCommand
			command1 = Connection.CreateCommand(),
			command2 = Connection.CreateCommand())
		{
			command1.CommandText = "select * from test where int_field = 1";
			command2.CommandText = "select * from test where int_field = 2";

			await using (var r1 = await command1.ExecuteReaderAsync())
			{
				await using (var r2 = await command2.ExecuteReaderAsync())
				{ }

				// Try to call ExecuteReader in	command1
				// it should throw an exception
				Assert.ThrowsAsync<InvalidOperationException>(() => command1.ExecuteReaderAsync());
			}
		}
	}

	[Test]
	public async Task ExecuteReaderWithBehaviorTestAsync()
	{
		await using (var command = new IBCommand("select * from TEST", Connection))
		{
			await using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
			{ }
		}
	}

	[Test]
	public async Task ExecuteScalarTestAsync()
	{
		await using (var command = Connection.CreateCommand())
		{
			command.CommandText = "select CHAR_FIELD from TEST where INT_FIELD = ?";
			command.Parameters.Add("@INT_FIELD", 2);
			var charFieldValue = (await command.ExecuteScalarAsync()).ToString();
			Assert.AreEqual("IRow 2", charFieldValue.TrimEnd(' '));
		}
	}

	[Test]
	public async Task ExecuteScalarWithStoredProcedureTestAsync()
	{
		await using (var command = Connection.CreateCommand())
		{
			command.CommandText = "SimpleSP";
			command.CommandType = CommandType.StoredProcedure;
			var result = (int)await command.ExecuteScalarAsync();
			Assert.AreEqual(1000, result);
		}
	}

	[Test]
	public async Task PrepareTestAsync()
	{
		var command = new IBCommand("insert into PrepareTest(test_field) values(@test_field);", Connection);

		command.Parameters.Add("@test_field", IBDbType.VarChar).Value = DBNull.Value;
		command.Prepare();

		for (var i = 0; i < 5; i++)
		{
			if (i < 1)
			{
				command.Parameters[0].Value = DBNull.Value;
			}
			else
			{
				command.Parameters[0].Value = i.ToString();
			}
			await command.ExecuteNonQueryAsync();
		}

		await command.DisposeAsync();

		var select = new IBCommand("select * from	PrepareTest", Connection);
		var reader = await select.ExecuteReaderAsync();
		var count = 0;
		while (await reader.ReadAsync())
		{
			if (count == 0)
			{
				Assert.AreEqual(DBNull.Value, reader[0], "Invalid value.");
			}
			else
			{
				Assert.AreEqual(count, reader.GetInt32(0), "Invalid	value.");
			}

			count++;
		}
		await reader.CloseAsync();
		await select.DisposeAsync();
	}

	[Test]
	public async Task NamedParametersTestAsync()
	{
		await using (var command = Connection.CreateCommand())
		{
			command.CommandText = "select CHAR_FIELD from TEST where INT_FIELD = @int_field or CHAR_FIELD = @char_field";
			command.Parameters.Add("@int_field", 2);
			command.Parameters.Add("@char_field", "TWO");
			await using (var reader = await command.ExecuteReaderAsync())
			{
				var count = 0;

				while (await reader.ReadAsync())
				{
					count++;
				}
				Assert.AreEqual(1, count, "Invalid number of records fetched.");
			}
		}
	}

	[Test]
	public async Task NamedParametersAndLiteralsAsync()
	{
		await using (var command = new IBCommand("update test set char_field = 'carlos@firebird.org', bigint_field = @bigint, varchar_field = 'carlos@ado.net' where int_field = @integer", Connection))
		{
			command.Parameters.Add("@bigint", IBDbType.BigInt).Value = 200;
			command.Parameters.Add("@integer", IBDbType.Integer).Value = 1;
			var recordsAffected = await command.ExecuteNonQueryAsync();
			Assert.AreEqual(recordsAffected, 1, "Invalid number of records affected.");
		}
	}

	[Test]
	public async Task NamedParametersReuseTestAsync()
	{
		await using (var command = new IBCommand("select * from test where int_field >= @lang and int_field <= @lang", Connection))
		{
			command.Parameters.Add("@lang", IBDbType.Integer).Value = 10;
			await using (var reader = await command.ExecuteReaderAsync())
			{
				var count = 0;
				var intValue = 0;
				while (await reader.ReadAsync())
				{
					if (count == 0)
					{
						intValue = reader.GetInt32(0);
					}
					count++;
				}
				Assert.AreEqual(1, count, "Invalid number of records fetched.");
				Assert.AreEqual(10, intValue, "Invalid record fetched.");
			}
		}
	}

	[Test]
	public async Task NamedParametersPublicAccessorAsync()
	{
		await using (var command = new IBCommand("select * from test where int_field >= @x1 and int_field <= @x2", Connection))
		{
			Assert.IsNotNull(command.NamedParameters, "Unexpected null reference.");
			Assert.IsTrue(command.NamedParameters.Count == 0, "Expected count 0 of named parameters before command prepare.");

			await command.PrepareAsync();

			Assert.IsTrue(command.NamedParameters.Count == 2, "Expected count 2 of named parameters after command prepare.");
			Assert.AreEqual(command.NamedParameters[0], "@x1");
			Assert.AreEqual(command.NamedParameters[1], "@x2");
		}
	}

	[Test]
	public async Task ExecuteStoredProcTestAsync()
	{
		await using (var command = new IBCommand("EXECUTE PROCEDURE GETVARCHARFIELD(?)", Connection))
		{
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
			command.Parameters.Add("@VARCHAR_FIELD", IBDbType.VarChar).Direction = ParameterDirection.Output;
			command.Parameters[0].Value = 1;
			await command.ExecuteNonQueryAsync();
			Assert.AreEqual("IRow Number 1", command.Parameters[1].Value);
		}
	}

	[Test]
	public async Task RecordAffectedTestAsync()
	{
		await using (var command = new IBCommand("insert into test (int_field) values (100000)", Connection))
		{
			await using (var reader = await command.ExecuteReaderAsync())
			{
				Assert.AreEqual(1, reader.RecordsAffected);
				while (await reader.ReadAsync())
				{ }
				Assert.AreEqual(1, reader.RecordsAffected);
			}
		}
	}

	[Test]
	public async Task ExecuteNonQueryWithOutputParametersAsync()
	{
		await using (var command = new IBCommand("EXECUTE PROCEDURE GETASCIIBLOB(?)", Connection))
		{
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
			command.Parameters.Add("@CLOB_FIELD", IBDbType.Text).Direction = ParameterDirection.Output;
			command.Parameters[0].Value = 1;
			await command.ExecuteNonQueryAsync();
			Assert.AreEqual("IRow Number 1", command.Parameters[1].Value, "Output parameter value is not valid");
		}
	}

	[Test]
	public async Task InvalidParameterFormatAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			try
			{
				await using (var command = new IBCommand("update test set timestamp_field = @timestamp where int_field = @integer", Connection, transaction))
				{
					command.Parameters.Add("@timestamp", IBDbType.TimeStamp).Value = 1;
					command.Parameters.Add("@integer", IBDbType.Integer).Value = 1;
					await command.ExecuteNonQueryAsync();
				}
				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
			}
		}
	}

	[Test]
	public async Task UnicodeTestAsync()
	{
		try
		{
			await using (var create = new IBCommand("CREATE TABLE VARCHARTEST (VARCHAR_FIELD  VARCHAR(10))", Connection))
			{
				await create.ExecuteNonQueryAsync();
			}
			var statements = new[]
			{
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('1')",
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('11')",
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('111')",
				"INSERT INTO VARCHARTEST (VARCHAR_FIELD) VALUES ('1111')"
			};
			foreach (string statement in statements)
			{
				await using (var insert = new IBCommand(statement, Connection))
				{
					await insert.ExecuteNonQueryAsync();
				}
			}
			await using (var cmd = new IBCommand("select * from varchartest", Connection))
			{
				await using (var r = await cmd.ExecuteReaderAsync())
				{
					while (await r.ReadAsync())
					{
						var dummy = r[0];
					}
				}
			}
		}
		finally
		{
			await using (var drop = new IBCommand("DROP TABLE VARCHARTEST", Connection))
			{
				await drop.ExecuteNonQueryAsync();
			}
		}
	}

	[Test]
	public async Task SimplifiedChineseTestAsync()
	{
		const string Value = "中文";
		try
		{
			await using (var cmd = new IBCommand("CREATE TABLE TABLE1 (FIELD1 varchar(20))", Connection))
			{
				await cmd.ExecuteNonQueryAsync();
			}
			await using (var cmd = new IBCommand("INSERT INTO TABLE1 VALUES (@value)", Connection))
			{
				cmd.Parameters.Add("@value", IBDbType.VarChar).Value = Value;
				await cmd.ExecuteNonQueryAsync();
			}
			await using (var cmd = new IBCommand($"INSERT INTO TABLE1 VALUES ('{Value}')", Connection))
			{
				await cmd.ExecuteNonQueryAsync();
			}
			await using (var cmd = new IBCommand("SELECT * FROM TABLE1", Connection))
			{
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						Assert.AreEqual(Value, reader[0]);
					}
				}
			}
		}
		finally
		{
			await using (var cmd = new IBCommand("DROP TABLE TABLE1", Connection))
			{
				await cmd.ExecuteNonQueryAsync();
			}
		}
	}

	[Test]
	public async Task InsertDateTestAsync()
	{
		await using (var command = new IBCommand("insert into TEST (int_field, date_field) values (1002, @date)", Connection))
		{
			command.Parameters.Add("@date", IBDbType.Date).Value = DateTime.Now.ToString();
			var ra = await command.ExecuteNonQueryAsync();
			Assert.AreEqual(ra, 1);
		}
	}

	[Test]
	public async Task InsertNullTestAsync()
	{
		await using (var command = new IBCommand("insert into TEST (int_field) values (@value)", Connection))
		{
			command.Parameters.Add("@value", IBDbType.Integer).Value = null;
			try
			{
				await command.ExecuteNonQueryAsync();
				Assert.Fail("The command was executed without throwing an exception.");
			}
			catch
			{ }
		}
	}

	[Test]
	public async Task InsertDateTimeTestAsync()
	{
		var value = DateTime.Now;

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, timestamp_field) values (1002, @dt)";
			cmd.Parameters.Add("@dt", IBDbType.TimeStamp).Value = value;
			var ra = await cmd.ExecuteNonQueryAsync();
			Assert.AreEqual(ra, 1);
		}

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select timestamp_field from test where int_field = 1002";
			var result = (DateTime)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(value.ToString(), result.ToString());
		}
	}

	[Test]
	public async Task InsertTimeStampTestAsync()
	{
		var value = DateTime.Now.ToString();

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, timestamp_field) values (1002, @ts)";
			cmd.Parameters.Add("@ts", IBDbType.TimeStamp).Value = value;
			var ra = await cmd.ExecuteNonQueryAsync();
			Assert.AreEqual(ra, 1);
		}

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select timestamp_field from test where int_field = 1002";
			var result = (DateTime)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(value, result.ToString());
		}
	}

	[Test]
	public virtual async Task InsertTimeTestAsync()
	{
		var t = new TimeSpan(0, 5, 6, 7, 231);

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, time_field) values (2245, @t)";
			if (IBTestsSetup.Dialect == 3)
				cmd.Parameters.Add("@t", IBDbType.Time).Value = t;
			else
				cmd.Parameters.Add("@t", IBDbType.TimeStamp).Value = t;
			var ra = await cmd.ExecuteNonQueryAsync();
			Assert.AreEqual(ra, 1);
		}

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select time_field from test where int_field = 2245";
			TimeSpan result;
			if (IBTestsSetup.Dialect == 3)
				result = (TimeSpan)await cmd.ExecuteScalarAsync();
			else
				result = ((DateTime)await cmd.ExecuteScalarAsync()).TimeOfDay;
			Assert.AreEqual(t.Hours, result.Hours, "hours are not same");
			Assert.AreEqual(t.Minutes, result.Minutes, "minutes are not same");
			Assert.AreEqual(t.Seconds, result.Seconds, "seconds are not same");
			Assert.AreEqual(t.Milliseconds, result.Milliseconds, "milliseconds are not same");
		}
	}

	[Test]
	public async Task InsertTimeOldTestAsync()
	{
		var t = DateTime.Today;
		t = t.AddHours(5);
		t = t.AddMinutes(6);
		t = t.AddSeconds(7);
		t = t.AddMilliseconds(231);

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "insert into test (int_field, time_field) values (2245, @t)";
			if (IBTestsSetup.Dialect == 3)
				cmd.Parameters.Add("@t", IBDbType.Time).Value = t;
			else
				cmd.Parameters.Add("@t", IBDbType.TimeStamp).Value = t;
			var ra = await cmd.ExecuteNonQueryAsync();
			Assert.AreEqual(ra, 1);
		}

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select time_field from test where int_field = 2245";
			TimeSpan result;

			if (IBTestsSetup.Dialect == 3)
				result = (TimeSpan)await cmd.ExecuteScalarAsync();
			else
				result = ((DateTime)await cmd.ExecuteScalarAsync()).TimeOfDay;
			Assert.AreEqual(t.Hour, result.Hours, "hours are not same");
			Assert.AreEqual(t.Minute, result.Minutes, "minutes are not same");
			Assert.AreEqual(t.Second, result.Seconds, "seconds are not same");
			Assert.AreEqual(t.Millisecond, result.Milliseconds, "milliseconds are not same");
		}
	}

	[Test]
	public async Task ParameterDescribeTestAsync()
	{
		await using (var command = new IBCommand("insert into TEST (int_field) values (@value)", Connection))
		{
			await command.PrepareAsync();
			command.Parameters.Add("@value", IBDbType.Integer).Value = 100000;
			await command.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task ReadOnlyTransactionTestAsync()
	{
		await using (var command = Connection.CreateCommand())
		{
			await using (var transaction = await Connection.BeginTransactionAsync(new IBTransactionOptions() { TransactionBehavior = IBTransactionBehavior.Read, WaitTimeout = null }))
			{
				command.Transaction = transaction;
				command.CommandType = CommandType.Text;
				command.CommandText = "CREATE TABLE X_TABLE_1(FIELD VARCHAR(50));";
				Assert.ThrowsAsync<IBException>(() => command.ExecuteNonQueryAsync());
				await transaction.CommitAsync();
			}
		}
	}

	[Test]
	public async Task ReadingVarcharOctetsTestAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			const string data = "1234";
			byte[] read = null;

			cmd.CommandText = string.Format("select cast('{0}' as varchar(10) character set octets) from rdb$database", data);
			await using (var reader = await cmd.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					read = (byte[])reader[0];
				}
			}

			var expected = Encoding.ASCII.GetBytes(data);
			Assert.AreEqual(expected, read);
		}
	}

	[Test]
	public async Task ReadingCharOctetsTestAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			const string data = "1234";
			byte[] read = null;

			cmd.CommandText = string.Format("select cast('{0}' as char(10) character set octets) from rdb$database", data);
			await using (var reader = await cmd.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					read = (byte[])reader[0];
				}
			}

			var expected = new byte[10];
			Encoding.ASCII.GetBytes(data).CopyTo(expected, 0);
			Assert.AreEqual(expected, read);
		}
	}

	[Test]
	public async Task GetCommandPlanTestAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select * from test";
			await cmd.PrepareAsync();
			var plan = default(string);
			Assert.DoesNotThrowAsync(async () => { plan = await cmd.GetCommandPlanAsync(); });
			Assert.IsNotEmpty(plan);
		}
	}

	[Test]
	public async Task GetCommandPlanNoPlanTestAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "alter table NoPlan alter id type int";
			await cmd.PrepareAsync();
			var plan = default(string);
			Assert.DoesNotThrowAsync(async () => { plan = await cmd.GetCommandPlanAsync(); });
			Assert.IsEmpty(plan);
		}
	}

	[Test]
	public virtual async Task ReadsTimeWithProperPrecisionAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast('00:00:01.4321' as time) from rdb$database";
			var result = (TimeSpan)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(TimeSpan.FromTicks(14321000), result);
		}
	}

	[Test]
	public virtual async Task PassesTimeSpanWithProperPrecisionAsync()
	{
		var ts = TimeSpan.FromTicks(14321000);
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@value as time) from rdb$database";
			cmd.Parameters.Add("value", ts);
			var result = (TimeSpan)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(ts, result);
		}
	}

	[Test]
	public async Task ReadsDateTimeWithProperPrecisionAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast('1.2.2015 05:06:01.4321' as timestamp) from rdb$database";
			var result = (DateTime)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(new DateTime(635583639614321000), result);
		}
	}

	[Test]
	public async Task PassesDateTimeWithProperPrecisionAsync()
	{
		var dt = new DateTime(635583639614321000);
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@value as timestamp) from rdb$database";
			cmd.Parameters.Add("value", dt);
			var result = (DateTime)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(dt, result);
		}
	}

	[Test]
	public async Task ExecuteNonQueryReturnsMinusOneOnNonInsertUpdateDeleteAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select 1 from rdb$database";
			var ra = await cmd.ExecuteNonQueryAsync();
			Assert.AreEqual(-1, ra);
		}
	}

	[Test]
	public async Task ExecuteNonQueryOnFetchAsync()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select 1, 'hello' from rdb$database";
			cmd.FetchEvent += FetchRow;
			var ra = await cmd.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task ExecuteNonQueryOnAlreadyCancelledTokenAsync()
	{
		using (var cts = new CancellationTokenSource())
		{
			cts.Cancel();
			await using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = "select 1 from rdb$database";
				Assert.ThrowsAsync<OperationCanceledException>(() => cmd.ExecuteNonQueryAsync(cts.Token));
			}
		}
	}
#if NET6_0_OR_GREATER

	[Test]
	public virtual async Task PassDateOnlyAsync()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@x as date) from rdb$database";
			cmd.Parameters.Add("x", new DateOnly(2021, 11, 29));
			var value = (DateTime)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(2021, value.Year);
			Assert.AreEqual(11, value.Month);
			Assert.AreEqual(29, value.Day);
		}
	}

	[Test]
	public virtual async Task PassTimeOnlyAsync()
	{
		if (IBTestsSetup.Dialect == 1)
			return;
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@x as time) from rdb$database";
			cmd.Parameters.Add("x", new TimeOnly(501940213000));
			var value = (TimeSpan)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(501940213000, value.Ticks);
		}
	}
#endif


	#endregion
}

public class IBCommandTestsDialect1 : IBCommandTests
{
	public IBCommandTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}

	[Test]
	public override void ReadsTimeWithProperPrecision()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast('01/12/2023 00:00:01.4321' as timestamp) from rdb$database";
			TimeSpan result;
			result = ((DateTime)cmd.ExecuteScalar()).TimeOfDay;
			Assert.AreEqual(TimeSpan.FromTicks(14321000), result);
		}
	}
	public override async Task ReadsTimeWithProperPrecisionAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast('00:00:01.4321' as timestamp) from rdb$database";
			var result = (TimeSpan)await cmd.ExecuteScalarAsync();
			Assert.AreEqual(TimeSpan.FromTicks(14321000), result);
		}
	}

	[Ignore("No Dialect 1 time datatype so just ignore")]
	[Test]
	public override void InsertTimeTest()
	{

	}

	[Ignore("No Dialect 1 time datatype so just ignore")]
	[Test]
	public override void PassesTimeSpanWithProperPrecision()
	{
	}

	[Ignore("No Dialect 1 time datatype so just ignore")]
	[Test]
	public override async Task InsertTimeTestAsync()
	{
	}

	[Ignore("No Dialect 1 time datatype so just ignore")]
	[Test]
	public override async Task PassesTimeSpanWithProperPrecisionAsync()
	{
	}

}
