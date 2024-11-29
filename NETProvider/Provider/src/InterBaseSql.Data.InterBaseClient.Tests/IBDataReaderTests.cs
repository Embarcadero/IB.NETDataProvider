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
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

public static class Extensions
{
	/// <summary>
	/// A CLSCompliant method to convert a big-endian Guid to little-endian
	/// and vice versa.
	/// The Guid Constructor (UInt32, UInt16, UInt16, Byte, Byte, Byte, Byte,
	///  Byte, Byte, Byte, Byte) is not CLSCompliant.
	/// </summary>
//		[CLSCompliant(true)]
	public static Guid FlipEndian(this Guid guid)
	{
		var newBytes = new byte[16];
		var oldBytes = guid.ToByteArray();

		for (var i = 8; i < 16; i++)
			newBytes[i] = oldBytes[i];

		newBytes[3] = oldBytes[0];
		newBytes[2] = oldBytes[1];
		newBytes[1] = oldBytes[2];
		newBytes[0] = oldBytes[3];
		newBytes[5] = oldBytes[4];
		newBytes[4] = oldBytes[5];
		newBytes[6] = oldBytes[7];
		newBytes[7] = oldBytes[6];

		return new Guid(newBytes);
	}
}

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBDataReaderTests : IBTestsBase
{
	#region Constructors

	public IBDataReaderTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void ReadTest()
	{
		var transaction = Connection.BeginTransaction();

		var command = new IBCommand("select * from TEST", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			for (var i = 0; i < reader.FieldCount; i++)
			{
				reader.GetValue(i);
			}
		}

		reader.Close();
		command.Dispose();
		transaction.Rollback();
	}

	[Test]
	public void ReadClobTest()
	{
		var transaction = Connection.BeginTransaction();

		var command = new IBCommand("select * from TEST", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			reader.GetValue(reader.GetOrdinal("clob_field"));
		}

		reader.Close();
		command.Dispose();
		transaction.Rollback();
	}

	[Test]
	public void BigIntGetStringTest()
	{
		var transaction = Connection.BeginTransaction();

		var command = new IBCommand("select * from TEST", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			reader.GetString(reader.GetOrdinal("bigint_field"));
		}

		reader.Close();
		command.Dispose();
		transaction.Rollback();
	}

	[Test]
	public void GetValuesTest()
	{
		var transaction = Connection.BeginTransaction();

		var command = new IBCommand("select * from TEST", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			var values = new object[reader.FieldCount];
			reader.GetValues(values);
		}

		reader.Close();
		transaction.Rollback();
		command.Dispose();
	}

	[Test]
	public void IndexerByIndexTest()
	{
		var transaction = Connection.BeginTransaction();

		var command = new IBCommand("select * from TEST", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			for (var i = 0; i < reader.FieldCount; i++)
			{
				var dummy = reader[i];
			}
		}

		reader.Close();
		transaction.Rollback();
		command.Dispose();
	}

	[Test]
	public void IndexerByNameTest()
	{
		var transaction = Connection.BeginTransaction();

		var command = new IBCommand("select * from TEST", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			for (var i = 0; i < reader.FieldCount; i++)
			{
				var dummy = reader[reader.GetName(i)];
			}
		}

		reader.Close();
		transaction.Rollback();
		command.Dispose();
	}

	[Test]
	public void GetSchemaTableTest()
	{
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand("select * from TEST", Connection, transaction);

		var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);

		var schema = reader.GetSchemaTable();
		var currRows = schema.Select(null, null, DataViewRowState.CurrentRows);

		reader.Close();
		transaction.Rollback();
		command.Dispose();
	}

	[Test]
	public void GetSchemaTableWithExpressionFieldTest()
	{
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand("select TEST.*, 0 AS VALOR from TEST", Connection, transaction);

		var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);

		var schema = reader.GetSchemaTable();
		var currRows = schema.Select(null, null, DataViewRowState.CurrentRows);

		reader.Close();
		transaction.Rollback();
		command.Dispose();
	}

	[Test]
	public void RecordAffectedTest()
	{
		var sql = "insert into test (int_field) values (100000)";

		var command = new IBCommand(sql, Connection);

		var reader = command.ExecuteReader();

		while (reader.Read())
		{
		}

		reader.Close();

		Assert.AreEqual(1, reader.RecordsAffected, "RecordsAffected value is incorrect");
	}

	[Test]
	public void GetBytesLengthTest()
	{
		var sql = "select blob_field from TEST where int_field = @int_field";

		var command = new IBCommand(sql, Connection);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 2;

		var reader = command.ExecuteReader();

		reader.Read();

		var length = reader.GetBytes(0, 0, null, 0, 0);

		reader.Close();

		Assert.AreEqual(13, length, "Incorrect blob length");
	}

	[Test]
	public void GetCharsLengthTest()
	{
		var sql = "select clob_field from TEST where int_field = @int_field";

		var command = new IBCommand(sql, Connection);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 50;

		var reader = command.ExecuteReader();

		reader.Read();

		var length = reader.GetChars(0, 0, null, 0, 0);

		reader.Close();

		Assert.AreEqual(14, length, "Incorrect clob length");
	}

	[Test]
	public void TruncateCharTest()
	{
		var sql = "select dept_no, phone_ext from employee where emp_no = 9";
		IBCommand command;
		IBDataReader reader;
		command = new IBCommand(sql, Connection);
		reader = command.ExecuteReader();
		reader.Read();
		Assert.AreEqual("6  ", reader.GetString(0), "String wrong expected '6  ' received '" + reader.GetString(0) + "'");
		Assert.AreEqual("2  ", reader.GetString(1), "String wrong expected '2  ' received '" + reader.GetString(1) + "'");

		reader.Close();
		Connection.Close();
		Connection.TruncateChar = true;
		Connection.Open();

		reader = command.ExecuteReader();
		reader.Read();
		Assert.AreEqual("6", reader.GetString(0), "String wrong expected '6' received '" + reader.GetString(0) + "'");
		Assert.AreEqual("2  ", reader.GetString(1), "String wrong expected '2  ' received '" + reader.GetString(1) + "'");
	}

	[Test]
	public virtual void ValidateDecimalSchema()
	{
		var sql = "select decimal_field from test";

		var test = new IBCommand(sql, Connection);
		var r = test.ExecuteReader(CommandBehavior.SchemaOnly);

		var schema = r.GetSchemaTable();

		r.Close();

		// Check schema values
		Assert.AreEqual(8, schema.Rows[0]["ColumnSize"], "Invalid length");
		Assert.AreEqual(15, schema.Rows[0]["NumericPrecision"], "Invalid precision");
		Assert.AreEqual(2, schema.Rows[0]["NumericScale"], "Invalid scale");
	}

	[Test]
	public void DisposeTest()
	{
		using (var command = new IBCommand("DATAREADERTEST", Connection))
		{
			command.CommandType = CommandType.StoredProcedure;

			IBCommandBuilder.DeriveParameters(command);

			using (IDataReader reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
				}
			}
		}
	}

	[Test]
	public virtual void GetOrdinalTest()
	{
		var transaction = Connection.BeginTransaction();

//			var command = new IBCommand("select 0 as fOo, 0 as BAR from rdb$database", Connection, transaction);
		var command = new IBCommand("select 0 as fOo, 0 as \"BaR\", 0 as BAR from rdb$database", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			var foo = reader.GetOrdinal("foo");
			var FOO = reader.GetOrdinal("FOO");
			var fOo = reader.GetOrdinal("fOo");
			Assert.AreEqual(0, foo);
			Assert.AreEqual(0, FOO);
			Assert.AreEqual(0, fOo);

			var bar = reader.GetOrdinal("bar");
			var BaR = reader.GetOrdinal("BaR");
			Assert.AreEqual(1, bar);
			Assert.AreEqual(1, BaR);

			var BAR = reader.GetOrdinal("BAR");
			Assert.AreEqual(2, BAR);
		}

		reader.Close();
		transaction.Rollback();
		command.Dispose();
	}

	[Test]
	public void ReadBinaryTest()
	{
		var transaction = Connection.BeginTransaction();

		var bytes = new byte[1024];
		var random = new Random();
		for (var i = 0; i < bytes.Length; i++)
		{
			bytes[i] = (byte)random.Next(byte.MinValue, byte.MaxValue);
		}
		var binaryString = $"{BitConverter.ToString(bytes).Replace("-", string.Empty)}";

		var command = new IBCommand($"update test set clob_field = '{binaryString}' where int_field = 0", Connection, transaction);
		command.ExecuteNonQuery();

		command = new IBCommand($"select clob_field from test where int_field = 0", Connection, transaction);
		IDataReader reader = command.ExecuteReader();
		if (reader.Read())
		{
			var value = reader.GetString(0);
			Assert.AreEqual(binaryString, value);
		}

		reader.Close();
		command.Dispose();
		transaction.Rollback();
	}

	[Test]
	public void ReadGuidRoundTripTest()
	{
		using (var cmd = Connection.CreateCommand())
		{
			var guid = Guid.NewGuid();
			var commandText = $"select cast(ef_char_to_uuid('{guid}') as Char(16) character set octets) from rdb$database";
			cmd.CommandText = commandText;
			using (var reader = cmd.ExecuteReader())
			{
				if (reader.Read())
				{
					Assert.AreEqual(guid, reader.GetGuid(0).FlipEndian());
				}
				else
				{
					Assert.Fail();
				}
			}
		}
	}

	[Test]
	public void ReadGuidRoundTrip2Test()
	{
		using (var cmd = Connection.CreateCommand())
		{
			var commandText = @"
create procedure guidtest
returns (a char(16) character set octets, b char(38))
as
declare variable guid char(16) character set octets;
begin
	guid = ef_newguid();
	for select :guid, EF_UUID_TO_CHAR(:guid) from rdb$database into a, b do
	begin
		suspend;
	end
end";
			cmd.CommandText = commandText;
			cmd.ExecuteNonQuery();
			try
			{
				cmd.CommandText = $"Select * from guidtest";
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						StringAssert.AreEqualIgnoringCase(reader.GetString(1), reader.GetGuid(0).ToString().Substring(1, 36));
					}
					else
					{
						Assert.Fail();
					}
				}
			}
			catch { }

			cmd.CommandText = $"Drop procedure guidtest";
			cmd.ExecuteNonQuery();
		}
	}

	[Test]
	public void ReadGuidRoundTrip3Test()
	{
		using (var cmd = Connection.CreateCommand())
		{
			var guid = Guid.NewGuid();
			var str = System.Text.Encoding.ASCII.GetString(guid.ToByteArray());
			var commandText = $"select cast('{str}' as char(16) character set octets) from rdb$database";
			cmd.CommandText = commandText;
			using (var reader = cmd.ExecuteReader())
			{
				if (reader.Read())
				{
					Assert.AreEqual(str, System.Text.Encoding.ASCII.GetString(reader.GetGuid(0).FlipEndian().ToByteArray()));
				}
				else
				{
					Assert.Fail();
				}
			}
		}
	}

	[Test]
	public void DNET60_EmptyFieldReadingError()
	{
		using (var command = Connection.CreateCommand())
		{
			command.CommandText = "select '' AS EmptyColumn from rdb$database";

			using (var r = command.ExecuteReader())
			{
				while (r.Read())
				{
				}
			}
		}
	}

	[Test]
	public void DNET183_VarcharSpacesShouldNotBeTrimmed()
	{
		const string value = "foo  ";

		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast('foo  ' as varchar(5)) from rdb$database";
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					Assert.AreEqual(value, (string)reader[0]);
				}
			}
		}
	}

	[Test]
	public void DNET749_CommandBehaviorCloseConnectionStackOverflow()
	{
		using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select * from rdb$database";
			var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			{
				while (reader.Read())
				{

				}
			}
		}
	}

	[Test]
	public virtual void GetFieldValueTest()
	{
		using (var transaction = Connection.BeginTransaction())
		{
			using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						reader.GetFieldValue<int>("int_field");
						reader.GetFieldValue<long>("bigint_field");
						reader.GetFieldValue<string>("varchar_field");
#if NET6_0_OR_GREATER
						reader.GetFieldValue<TimeOnly>("time_field");
#endif
					}
				}
			}
			transaction.Rollback();
		}
	}
	#endregion

	#region Async Unit Tests

	[Test]
	public async Task ReadTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						for (var i = 0; i < reader.FieldCount; i++)
						{
							reader.GetValue(i);
						}
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task ReadClobTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						reader.GetValue("clob_field");
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task BigIntGetStringTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						reader.GetString("bigint_field");
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task GetValuesTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						var values = new object[reader.FieldCount];
						reader.GetValues(values);
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task IndexerByIndexTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						for (var i = 0; i < reader.FieldCount; i++)
						{
							var dummy = reader[i];
						}
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task IndexerByNameTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						for (var i = 0; i < reader.FieldCount; i++)
						{
							var dummy = reader[reader.GetName(i)];
						}
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task GetSchemaTableTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
				{
					using (var schema = await reader.GetSchemaTableAsync())
					{
						var currRows = schema.Select(null, null, DataViewRowState.CurrentRows);
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task GetSchemaTableWithExpressionFieldTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select TEST.*, 0 AS VALOR from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
				{
					using (var schema = await reader.GetSchemaTableAsync())
					{
						var currRows = schema.Select(null, null, DataViewRowState.CurrentRows);
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task RecordAffectedTestAsync()
	{
		await using (var command = new IBCommand("insert into test (int_field) values (100000)", Connection))
		{
			await using (var reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{ }
				Assert.AreEqual(1, reader.RecordsAffected);
			}
		}
	}

	[Test]
	public async Task GetBytesLengthTestAsync()
	{
		await using (var command = new IBCommand("select blob_field from TEST where int_field = @int_field", Connection))
		{
			command.Parameters.Add("@int_field", IBDbType.Integer).Value = 2;
			await using (var reader = await command.ExecuteReaderAsync())
			{
				await reader.ReadAsync();
				var length = reader.GetBytes(0, 0, null, 0, 0);
				Assert.AreEqual(13, length, "Incorrect blob length");
			}
		}
	}

	[Test]
	public async Task GetCharsLengthTestAsync()
	{
		await using (var command = new IBCommand("select clob_field from TEST where int_field = @int_field", Connection))
		{
			command.Parameters.Add("@int_field", IBDbType.Integer).Value = 50;
			await using (var reader = await command.ExecuteReaderAsync())
			{
				await reader.ReadAsync();
				var length = reader.GetChars(0, 0, null, 0, 0);
				Assert.AreEqual(14, length, "Incorrect clob length");
			}
		}
	}

	[Test]
	public async Task TruncateCharTestAsync()
	{
		var sql = "select dept_no, phone_ext from employee where emp_no = 9";
		IBCommand command;
		IBDataReader reader;
		command = new IBCommand(sql, Connection);
		reader = await command.ExecuteReaderAsync();
		await reader.ReadAsync();
		Assert.AreEqual("6  ", reader.GetString(0), "String wrong expected '6  ' received '" + reader.GetString(0) + "'");
		Assert.AreEqual("2  ", reader.GetString(1), "String wrong expected '2  ' received '" + reader.GetString(1) + "'");

		await reader.CloseAsync();
		await Connection.CloseAsync();
		Connection.TruncateChar = true;
		await Connection.OpenAsync();

		reader = await command.ExecuteReaderAsync();
		await reader.ReadAsync();
		Assert.AreEqual("6", reader.GetString(0), "String wrong expected '6' received '" + reader.GetString(0) + "'");
		Assert.AreEqual("2  ", reader.GetString(1), "String wrong expected '2  ' received '" + reader.GetString(1) + "'");
	}

	[Test]
	public virtual async Task ValidateDecimalSchemaAsync()
	{
		await using (var test = new IBCommand("select decimal_field from test", Connection))
		{
			await using (var r = await test.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
			{
				using (var schema = await r.GetSchemaTableAsync())
				{
					Assert.AreEqual(8, schema.Rows[0]["ColumnSize"],"Invalid length");
					Assert.AreEqual(15, schema.Rows[0]["NumericPrecision"], "Invalid precision");
					Assert.AreEqual(2, schema.Rows[0]["NumericScale"], "Invalid scale");
				}
			}
		}
	}

	[Test]
	public async Task DisposeTestAsync()
	{
		await using (var command = new IBCommand("DATAREADERTEST", Connection))
		{
			command.CommandType = CommandType.StoredProcedure;
			IBCommandBuilder.DeriveParameters(command);
			await using (var reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{ }
			}
		}
	}

	[Test]
	public virtual async Task GetOrdinalTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select 0 as fOo, 0 as \"BaR\", 0 as BAR from rdb$database", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						var foo = reader.GetOrdinal("foo");
						var FOO = reader.GetOrdinal("FOO");
						var fOo = reader.GetOrdinal("fOo");
						Assert.AreEqual(0, foo);
						Assert.AreEqual(0, FOO);
						Assert.AreEqual(0, fOo);

						var bar = reader.GetOrdinal("bar");
						var BaR = reader.GetOrdinal("BaR");
						Assert.AreEqual(1, bar);
						Assert.AreEqual(1, BaR);

						var BAR = reader.GetOrdinal("BAR");
						Assert.AreEqual(2, BAR);
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task ReadBinaryTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			var bytes = new byte[1024];
			var random = new Random();
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] = (byte)random.Next(byte.MinValue, byte.MaxValue);
			}
			var binaryString = $"{BitConverter.ToString(bytes).Replace("-", string.Empty)}";

			var command = new IBCommand($"update test set clob_field = '{binaryString}' where int_field = 0", Connection, transaction);
			command.ExecuteNonQuery();

			await using (command = new IBCommand($"select clob_field from test where int_field = 0", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						var value = reader[0];
						Assert.AreEqual(binaryString, value);
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task ReadGuidRoundTripTestAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			var guid = Guid.NewGuid();
			var commandText = $"select cast(ef_char_to_uuid('{guid}') as Char(16) character set octets) from rdb$database";
			cmd.CommandText = commandText;
			await using (var reader = await cmd.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					Assert.AreEqual(guid, reader.GetGuid(0).FlipEndian());
				}
				else
				{
					Assert.Fail();
				}
			}
		}
	}

	[Test]
	public async Task ReadGuidRoundTrip2TestAsync()
	{
		using (var cmd = Connection.CreateCommand())
		{
			var commandText = @"
create procedure guidtest
returns (a char(16) character set octets, b char(38))
as
declare variable guid char(16) character set octets;
begin
	guid = ef_newguid();
	for select :guid, EF_UUID_TO_CHAR(:guid) from rdb$database into a, b do
	begin
		suspend;
	end
end";
			cmd.CommandText = commandText;
			await cmd.ExecuteNonQueryAsync();
			try
			{
				cmd.CommandText = $"Select * from guidtest";
				using (var reader =await  cmd.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						StringAssert.AreEqualIgnoringCase(reader.GetString(1), reader.GetGuid(0).ToString().Substring(1, 36));
					}
					else
					{
						Assert.Fail();
					}
				}
			}
			catch { }

			cmd.CommandText = $"Drop procedure guidtest";
			await cmd.ExecuteNonQueryAsync();
		}
	}

	[Test]
	public async Task ReadGuidRoundTrip3TestAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			var guid = Guid.NewGuid();
			var commandText = $"select cast(@guid as char(16) character set octets) from rdb$database";
			cmd.CommandText = commandText;
			cmd.Parameters.Add("guid", guid);
			await using (var reader = await cmd.ExecuteReaderAsync())
			{
				if (await reader.ReadAsync())
				{
					Assert.AreEqual(guid, reader.GetGuid(0));
				}
				else
				{
					Assert.Fail();
				}
			}
		}
	}

	[Test]
	public async Task DNET60_EmptyFieldReadingErrorAsync()
	{
		await using (var command = Connection.CreateCommand())
		{
			command.CommandText = "select '' AS EmptyColumn from rdb$database";
			await using (var r = await command.ExecuteReaderAsync())
			{
				while (await r.ReadAsync())
				{ }
			}
		}
	}

	[Test]
	public async Task DNET183_VarcharSpacesShouldNotBeTrimmedAsync()
	{
		const string value = "foo  ";

		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select cast(@foo as varchar(5)) from rdb$database";
			cmd.Parameters.Add(new IBParameter() { ParameterName = "@foo", IBDbType = IBDbType.VarChar, Size = 5, Value = value });
			await using (var reader = await cmd.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{
					Assert.AreEqual(value, (string)reader[0]);
				}
			}
		}
	}

	[Test]
	public async Task DNET749_CommandBehaviorCloseConnectionStackOverflowAsync()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "select * from rdb$database";
			await using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
			{
				while (await reader.ReadAsync())
				{ }
			}
		}
	}

	[Test]
	public virtual async Task GetFieldValueTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						await reader.GetFieldValueAsync<int>("int_field");
						await reader.GetFieldValueAsync<long>("bigint_field");
						await reader.GetFieldValueAsync<string>("varchar_field");
#if NET6_0_OR_GREATER
						await reader.GetFieldValueAsync<TimeOnly>("time_field");
#endif
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}
#endregion

}
public class IBDataReaderTestsDialect1 : IBDataReaderTests
{
	public IBDataReaderTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}

	[Test]
	public override void GetOrdinalTest()
	{
		var transaction = Connection.BeginTransaction();

		var command = new IBCommand("select 0 as fOo, 0 as BAR from rdb$database", Connection, transaction);

		IDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
			var foo = reader.GetOrdinal("foo");
			var FOO = reader.GetOrdinal("FOO");
			var fOo = reader.GetOrdinal("fOo");
			Assert.AreEqual(0, foo);
			Assert.AreEqual(0, FOO);
			Assert.AreEqual(0, fOo);

			var bar = reader.GetOrdinal("bar");
			var BaR = reader.GetOrdinal("BaR");
			var BAR = reader.GetOrdinal("BAR");
			Assert.AreEqual(1, bar);
			Assert.AreEqual(1, BaR);
			Assert.AreEqual(1, BAR);
		}

		reader.Close();
		transaction.Rollback();
		command.Dispose();
	}

	[Test]
	public override void GetFieldValueTest()
	{
		using (var transaction = Connection.BeginTransaction())
		{
			using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						reader.GetFieldValue<int>("int_field");
						reader.GetFieldValue<long>("bigint_field");
						reader.GetFieldValue<string>("varchar_field");
#if NET6_0_OR_GREATER
						//reader.GetFieldValue<TimeOnly>("time_field");
#endif
					}
				}
			}
			transaction.Rollback();
		}
	}

	[Test]
	public override void ValidateDecimalSchema()
	{
		var sql = "select decimal_field from test";

		var test = new IBCommand(sql, Connection);
		var r = test.ExecuteReader(CommandBehavior.SchemaOnly);

		var schema = r.GetSchemaTable();

		r.Close();

		// Check schema values
		Assert.AreEqual(8, schema.Rows[0]["ColumnSize"], "Invalid length");
		Assert.AreEqual(15, schema.Rows[0]["NumericPrecision"], "Invalid precision");
		Assert.AreEqual(2, schema.Rows[0]["NumericScale"], "Invalid scale");
		if (IBTestsSetup.Dialect == 1)
			Assert.AreEqual("System.Double", schema.Rows[0]["DataType"].ToString(), "Datatype not Double but is " + schema.Rows[0]["DataType"].ToString());
		else
		Assert.AreEqual("System.Decimal", schema.Rows[0]["DataType"].ToString(), "Datatype not Decimal but is " + schema.Rows[0]["DataType"].ToString());
	}

	[Test]
	public override async Task GetOrdinalTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select 0 as fOo, 0 as BAR from rdb$database", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						var foo = reader.GetOrdinal("foo");
						var FOO = reader.GetOrdinal("FOO");
						var fOo = reader.GetOrdinal("fOo");
						Assert.AreEqual(0, foo);
						Assert.AreEqual(0, FOO);
						Assert.AreEqual(0, fOo);

						var bar = reader.GetOrdinal("bar");
						var BaR = reader.GetOrdinal("BaR");
						var BAR = reader.GetOrdinal("BAR");
						Assert.AreEqual(1, bar);
						Assert.AreEqual(1, BaR);
						Assert.AreEqual(1, BAR);
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public async Task ReadOnAlreadyCancelledToken()
	{
		using (var cts = new CancellationTokenSource())
		{
			cts.Cancel();
			await using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = "select 1 from rdb$database";
				Assert.ThrowsAsync<OperationCanceledException>(() => cmd.ExecuteReaderAsync(cts.Token));
			}
		}
	}

	[Test]
	public override async Task GetFieldValueTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						await reader.GetFieldValueAsync<int>("int_field");
						await reader.GetFieldValueAsync<long>("bigint_field");
						await reader.GetFieldValueAsync<string>("varchar_field");
#if NET6_0_OR_GREATER
						//await reader.GetFieldValueAsync<TimeOnly>("time_field");
#endif
					}
				}
			}
			await transaction.RollbackAsync();
		}
	}

	[Test]
	public override async Task ValidateDecimalSchemaAsync()
	{
		await using (var test = new IBCommand("select decimal_field from test", Connection))
		{
			await using (var r = await test.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
			{
				using (var schema = await r.GetSchemaTableAsync())
				{
					Assert.AreEqual(8, schema.Rows[0]["ColumnSize"], "Invalid length");
					Assert.AreEqual(15, schema.Rows[0]["NumericPrecision"], "Invalid precision");
					Assert.AreEqual(2, schema.Rows[0]["NumericScale"], "Invalid scale");
					if (IBTestsSetup.Dialect == 1)
						Assert.AreEqual("System.Double", schema.Rows[0]["DataType"].ToString(), "Datatype not Double but is " + schema.Rows[0]["DataType"].ToString());
					else
						Assert.AreEqual("System.Decimal", schema.Rows[0]["DataType"].ToString(), "Datatype not Decimal but is " + schema.Rows[0]["DataType"].ToString());
				}
			}
		}
	}


}
