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
using System.Data;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
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

		#region Unit Tests

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
		public void ValidateDecimalSchema()
		{
			var sql = "select decimal_field from test";

			var test = new IBCommand(sql, Connection);
			var r = test.ExecuteReader(CommandBehavior.SchemaOnly);

			var schema = r.GetSchemaTable();

			r.Close();

			// Check schema values
			Assert.AreEqual(schema.Rows[0]["ColumnSize"], 8, "Invalid length");
			Assert.AreEqual(schema.Rows[0]["NumericPrecision"], 15, "Invalid precision");
			Assert.AreEqual(schema.Rows[0]["NumericScale"], 2, "Invalid scale");
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
		public void GetOrdinalTest()
		{
			var transaction = Connection.BeginTransaction();

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
			var binaryString = $"'{BitConverter.ToString(bytes).Replace("-", string.Empty)}'";

			var command = new IBCommand($"select {binaryString} from rdb$database", Connection, transaction);

			IDataReader reader = command.ExecuteReader();
			if (reader.Read())
			{
				reader.GetValue(0);
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
				var commandText = $"select cast(ef_char_to_uuid('{guid}') as VarChar(16) character set octets) from rdb$database";
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
returns (a varchar(16) character set octets, b varchar(38))
as
declare variable guid varchar(16) character set octets;
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
				var commandText = $"select cast('{str}' as varchar(16) character set octets) from rdb$database";
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

		#endregion
	}
}
