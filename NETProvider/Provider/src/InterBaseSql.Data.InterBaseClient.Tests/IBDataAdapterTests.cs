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
using System.Threading.Tasks;
using System.Windows.Input;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBDataAdapterTests : IBTestsBase
{
	#region Constructors

	public IBDataAdapterTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void FillTest()
	{
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand("select * from TEST", Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(100, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();
		transaction.Commit();
	}

	[Test]
	public void FillMultipleTest()
	{
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand("select * from TEST", Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		var builder = new IBCommandBuilder(adapter);

		var ds1 = new DataSet();
		var ds2 = new DataSet();

		adapter.Fill(ds1, "TEST");
		adapter.Fill(ds2, "TEST");

		Assert.AreEqual(100, ds1.Tables["TEST"].Rows.Count, "Incorrect row count (ds1)");
		Assert.AreEqual(100, ds2.Tables["TEST"].Rows.Count, "Incorrect row count (ds2)");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();
		transaction.Commit();
	}

	[Test]
	public void FillMultipleWithImplicitTransactionTest()
	{
		var command = new IBCommand("select * from TEST", Connection);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		var builder = new IBCommandBuilder(adapter);

		var ds1 = new DataSet();
		var ds2 = new DataSet();

		adapter.Fill(ds1, "TEST");
		adapter.Fill(ds2, "TEST");

		Assert.AreEqual(100, ds1.Tables["TEST"].Rows.Count, "Incorrect row count (ds1)");
		Assert.AreEqual(100, ds2.Tables["TEST"].Rows.Count, "Incorrect row count (ds2)");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();
	}

	[Test]
	public void InsertTest()
	{
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand("select * from TEST", Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(100, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		var newRow = ds.Tables["TEST"].NewRow();

		newRow["int_field"] = 101;
		newRow["CHAR_FIELD"] = "ONE THOUSAND";
		newRow["VARCHAR_FIELD"] = ":;,.{}`+^*[]\\!|@#$%&/()?_-<>";
		newRow["BIGint_field"] = 100000;
		newRow["SMALLint_field"] = 100;
		newRow["DOUBLE_FIELD"] = 100.01;
		newRow["NUMERIC_FIELD"] = 100.01;
		newRow["DECIMAL_FIELD"] = 100.01;
		newRow["DATE_FIELD"] = new DateTime(100, 10, 10);
		if (IBTestsSetup.Dialect == 3)
			newRow["TIME_FIELD"] = new TimeSpan(10, 10, 10);
		newRow["TIMESTAMP_FIELD"] = new DateTime(100, 10, 10, 10, 10, 10, 10);
		newRow["CLOB_FIELD"] = "ONE THOUSAND";

		ds.Tables["TEST"].Rows.Add(newRow);

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();
		transaction.Commit();
	}

	[Test]
	public void UpdateCharTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["CHAR_FIELD"] = "ONE THOUSAND";

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT char_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (string)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual("ONE THOUSAND", val.Trim(), "char_field has not correct value");
	}

	[Test]
	public void UpdateVarCharTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["VARCHAR_FIELD"] = "ONE VAR THOUSAND";

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT varchar_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (string)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual("ONE VAR THOUSAND", val.Trim(), "varchar_field has not correct value");
	}

	[Test]
	public void UpdateSmallIntTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["SMALLint_field"] = System.Int16.MaxValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT smallint_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (short)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(System.Int16.MaxValue, val, "smallint_field has not correct value");
	}

	[Test]
	public void UpdateBigIntTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["BIGINT_FIELD"] = System.Int32.MaxValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT bigint_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(System.Int32.MaxValue, val, "bigint_field has not correct value");
		if (IBTestsSetup.Dialect == 3)
			Assert.IsTrue(val is System.Int64, "val is really " + val.GetType().ToString());
		else
			Assert.IsTrue(val is System.Double, "val is really " + val.GetType().ToString());
	}

	[Test]
	public void UpdateDoubleTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["DOUBLE_FIELD"] = System.Int32.MaxValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT double_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (double)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(System.Int32.MaxValue, val, "double_field has not correct value");
	}

	[Test]
	public void UpdateFloatTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["FLOAT_FIELD"] = (float)100.20;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT float_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (float)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual((float)100.20, val, "double_field has not correct value");
	}

	[Test]
	public void UpdateNumericTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["NUMERIC_FIELD"] = System.Int32.MaxValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT numeric_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(System.Int32.MaxValue, val, "numeric_field has not correct value");
		if (IBTestsSetup.Dialect == 3)
			Assert.IsTrue(val is System.Decimal, "val is really " + val.GetType().ToString());
		else
			Assert.IsTrue(val is System.Double, "val is really " + val.GetType().ToString());
	}

	[Test]
	public void UpdateDecimalTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["DECIMAL_FIELD"] = System.Int32.MaxValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT decimal_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(System.Int32.MaxValue, val, "decimal_field has not correct value");
		if (IBTestsSetup.Dialect == 3)
			Assert.IsTrue(val is System.Decimal, "val is really " + val.GetType().ToString());
		else
			Assert.IsTrue(val is System.Double, "val is really " + val.GetType().ToString());
	}

	[Test]
	public void UpdateDateTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		var dtValue = DateTime.Now;

		ds.Tables["TEST"].Rows[0]["DATE_FIELD"] = dtValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT date_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (DateTime)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(dtValue.Day, val.Day, "date_field has not correct day");
		Assert.AreEqual(dtValue.Month, val.Month, "date_field has not correct month");
		Assert.AreEqual(dtValue.Year, val.Year, "date_field has not correct year");
	}

	[Test]
	public virtual void UpdateTimeTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		var dtValue = new TimeSpan(5, 6, 7);

		ds.Tables["TEST"].Rows[0]["TIME_FIELD"] = dtValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT time_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (TimeSpan)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(dtValue.Hours, val.Hours, "time_field has not correct hour");
		Assert.AreEqual(dtValue.Minutes, val.Minutes, "time_field has not correct minute");
		Assert.AreEqual(dtValue.Seconds, val.Seconds, "time_field has not correct second");
	}

	[Test]
	public void UpdateTimeStampTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		var dtValue = DateTime.Now;

		ds.Tables["TEST"].Rows[0]["TIMESTAMP_FIELD"] = dtValue;

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		transaction.Commit();

		transaction = Connection.BeginTransaction();

		sql = "SELECT timestamp_field FROM TEST WHERE int_field = @int_field";
		command = new IBCommand(sql, Connection, transaction);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var val = (DateTime)command.ExecuteScalar();

		transaction.Commit();

		Assert.AreEqual(dtValue.Day, val.Day, "timestamp_field has not correct day");
		Assert.AreEqual(dtValue.Month, val.Month, "timestamp_field has not correct month");
		Assert.AreEqual(dtValue.Year, val.Year, "timestamp_field has not correct year");
		Assert.AreEqual(dtValue.Hour, val.Hour, "timestamp_field has not correct hour");
		Assert.AreEqual(dtValue.Minute, val.Minute, "timestamp_field has not correct minute");
		Assert.AreEqual(dtValue.Second, val.Second, "timestamp_field has not correct second");
	}

	[Test]
	public void UpdateClobTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0]["CLOB_FIELD"] = "ONE THOUSAND";

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();
		transaction.Commit();
	}

	[Test]
	public void DeleteTest()
	{
		var sql = "select * from TEST where int_field = @int_field";
		var transaction = Connection.BeginTransaction();
		var command = new IBCommand(sql, Connection, transaction);
		var adapter = new IBDataAdapter(command);
		adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

		adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 10;

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

		ds.Tables["TEST"].Rows[0].Delete();

		adapter.Update(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();
		transaction.Commit();
	}

	[Test]
	public void SubsequentDeletes()
	{
		using (var select = new IBCommand("SELECT * FROM test", Connection))
		{
			using (var delete = new IBCommand("DELETE FROM test WHERE int_field = @id", Connection))
			{
				delete.Parameters.Add("@id", IBDbType.Integer);
				delete.Parameters[0].SourceColumn = "INT_FIELD";
				using (var adapter = new IBDataAdapter(select))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
					adapter.DeleteCommand = delete;
					using (var ds = new DataSet())
					{
						adapter.Fill(ds);

						ds.Tables[0].Rows[0].Delete();
						adapter.Update(ds);

						ds.Tables[0].Rows[0].Delete();
						adapter.Update(ds);

						ds.Tables[0].Rows[0].Delete();
						adapter.Update(ds);
					}
				}
			}
		}
	}

	#endregion

	#region Async Unit Tests

	[Test]
	public async Task FillTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
					using (var ds = new DataSet())
					{
						adapter.Fill(ds, "TEST");
						Assert.AreEqual(100, ds.Tables["TEST"].Rows.Count, "Incorrect row count");
					}
				}
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task FillMultipleTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
					using (var ds1 = new DataSet())
					using (var ds2 = new DataSet())
					{
						adapter.Fill(ds1, "TEST");
						adapter.Fill(ds2, "TEST");

						Assert.AreEqual(100, ds1.Tables["TEST"].Rows.Count, "Incorrect row count (ds1)");
						Assert.AreEqual(100, ds2.Tables["TEST"].Rows.Count, "Incorrect row count (ds2)");
					}
				}
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task FillMultipleWithImplicitTransactionTestAsync()
	{
		await using (var command = new IBCommand("select * from TEST", Connection))
		{
			using (var adapter = new IBDataAdapter(command))
			{
				adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
				using (var ds1 = new DataSet())
				using (var ds2 = new DataSet())
				{
					adapter.Fill(ds1, "TEST");
					adapter.Fill(ds2, "TEST");

					Assert.AreEqual(100, ds1.Tables["TEST"].Rows.Count, "Incorrect row count (ds1)");
					Assert.AreEqual(100, ds2.Tables["TEST"].Rows.Count, "Incorrect row count (ds2)");
				}
			}
		}
	}

	[Test]
	public async Task InsertTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(100, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							var newRow = ds.Tables["TEST"].NewRow();

							newRow["int_field"] = 101;
							newRow["CHAR_FIELD"] = "ONE THOUSAND";
							newRow["VARCHAR_FIELD"] = ":;,.{}`+^*[]\\!|@#$%&/()?_-<>";
							newRow["BIGint_field"] = 100000;
							newRow["SMALLint_field"] = 100;
							newRow["DOUBLE_FIELD"] = 100.01;
							newRow["NUMERIC_FIELD"] = 100.01;
							newRow["DECIMAL_FIELD"] = 100.01;
							newRow["DATE_FIELD"] = new DateTime(100, 10, 10);
							if (IBTestsSetup.Dialect == 3)
								newRow["TIME_FIELD"] = new TimeSpan(10, 10, 10);
							newRow["TIMESTAMP_FIELD"] = new DateTime(100, 10, 10, 10, 10, 10, 10);
							newRow["CLOB_FIELD"] = "ONE THOUSAND";

							ds.Tables["TEST"].Rows.Add(newRow);

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateCharTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["CHAR_FIELD"] = "ONE THOUSAND";

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT char_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (string)await command.ExecuteScalarAsync();
				Assert.AreEqual("ONE THOUSAND", val.Trim(), "char_field has not correct value");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateVarCharTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["VARCHAR_FIELD"] = "ONE VAR THOUSAND";

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT varchar_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (string)await command.ExecuteScalarAsync();
				Assert.AreEqual("ONE VAR THOUSAND", val.Trim(), "varchar_field has not correct value");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateSmallIntTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["SMALLint_field"] = short.MaxValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT smallint_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (short)await command.ExecuteScalarAsync();
				Assert.AreEqual(short.MaxValue, val, "smallint_field has not correct value");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateBigIntTestAsync()
	{
		await using (var transaction = Connection.BeginTransaction())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["BIGINT_FIELD"] = int.MaxValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			transaction.Commit();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT bigint_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = await command.ExecuteScalarAsync();
				Assert.AreEqual(int.MaxValue, val, "bigint_field has not correct value");
				if (IBTestsSetup.Dialect == 3)
					Assert.IsTrue(val is System.Int64, "val is really " + val.GetType().ToString());
				else
					Assert.IsTrue(val is System.Double, "val is really " + val.GetType().ToString());
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateDoubleTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["DOUBLE_FIELD"] = int.MaxValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT double_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (double)await command.ExecuteScalarAsync();
				Assert.AreEqual(int.MaxValue, val, "double_field has not correct value");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateFloatTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["FLOAT_FIELD"] = (float)100.20;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT float_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (float)await command.ExecuteScalarAsync();
				Assert.AreEqual((float)100.20, val, "double_field has not correct value");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateNumericTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["NUMERIC_FIELD"] = int.MaxValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT numeric_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = await command.ExecuteScalarAsync();
				Assert.AreEqual(int.MaxValue, val, "numeric_field has not correct value");
				if (IBTestsSetup.Dialect == 3)
					Assert.IsTrue(val is System.Decimal, "val is really " + val.GetType().ToString());
				else
					Assert.IsTrue(val is System.Double, "val is really " + val.GetType().ToString());
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateDecimalTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["DECIMAL_FIELD"] = int.MaxValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT decimal_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = await command.ExecuteScalarAsync();
				Assert.AreEqual(int.MaxValue, val, "decimal_field has not correct value");
				if (IBTestsSetup.Dialect == 3)
					Assert.IsTrue(val is System.Decimal, "val is really " + val.GetType().ToString());
				else
					Assert.IsTrue(val is System.Double, "val is really " + val.GetType().ToString());

			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateDateTestAsync()
	{
		var dtValue = DateTime.Now;

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");


							ds.Tables["TEST"].Rows[0]["DATE_FIELD"] = dtValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT date_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (DateTime)await command.ExecuteScalarAsync();
				Assert.AreEqual(dtValue.Day, val.Day, "date_field has not correct day");
				Assert.AreEqual(dtValue.Month, val.Month, "date_field has not correct month");
				Assert.AreEqual(dtValue.Year, val.Year, "date_field has not correct year");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public virtual async Task UpdateTimeTestAsync()
	{
		var dtValue = new TimeSpan(5, 6, 7);

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");


							ds.Tables["TEST"].Rows[0]["TIME_FIELD"] = dtValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT time_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (TimeSpan)await command.ExecuteScalarAsync();
				Assert.AreEqual(dtValue.Hours, val.Hours, "time_field has not correct hour");
				Assert.AreEqual(dtValue.Minutes, val.Minutes, "time_field has not correct minute");
				Assert.AreEqual(dtValue.Seconds, val.Seconds, "time_field has not correct second");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateTimeStampTestAsync()
	{
		var dtValue = DateTime.Now;

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["TIMESTAMP_FIELD"] = dtValue;

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}

		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("SELECT timestamp_field FROM TEST WHERE int_field = @int_field", Connection, transaction))
			{
				command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
				var val = (DateTime)await command.ExecuteScalarAsync();
				Assert.AreEqual(dtValue.Day, val.Day, "timestamp_field has not correct day");
				Assert.AreEqual(dtValue.Month, val.Month, "timestamp_field has not correct month");
				Assert.AreEqual(dtValue.Year, val.Year, "timestamp_field has not correct year");
				Assert.AreEqual(dtValue.Hour, val.Hour, "timestamp_field has not correct hour");
				Assert.AreEqual(dtValue.Minute, val.Minute, "timestamp_field has not correct minute");
				Assert.AreEqual(dtValue.Second, val.Second, "timestamp_field has not correct second");
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task UpdateClobTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0]["CLOB_FIELD"] = "ONE THOUSAND";

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task DeleteTestAsync()
	{
		await using (var transaction = await Connection.BeginTransactionAsync())
		{
			await using (var command = new IBCommand("select * from TEST where int_field = @int_field", Connection, transaction))
			{
				using (var adapter = new IBDataAdapter(command))
				{
					using (var builder = new IBCommandBuilder(adapter))
					{
						adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
						adapter.SelectCommand.Parameters.Add("@int_field", IBDbType.Integer).Value = 10;
						using (var ds = new DataSet())
						{
							adapter.Fill(ds, "TEST");

							Assert.AreEqual(1, ds.Tables["TEST"].Rows.Count, "Incorrect row count");

							ds.Tables["TEST"].Rows[0].Delete();

							adapter.Update(ds, "TEST");
						}
					}
				}
			}
			await transaction.CommitAsync();
		}
	}

	[Test]
	public async Task SubsequentDeletesAsync()
	{
		await using (var select = new IBCommand("SELECT * FROM test", Connection))
		{
			await using (var delete = new IBCommand("DELETE FROM test WHERE int_field = @id", Connection))
			{
				delete.Parameters.Add("@id", IBDbType.Integer);
				delete.Parameters[0].SourceColumn = "INT_FIELD";
				using (var adapter = new IBDataAdapter(select))
				{
					adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
					adapter.DeleteCommand = delete;
					using (var ds = new DataSet())
					{
						adapter.Fill(ds);

						ds.Tables[0].Rows[0].Delete();
						adapter.Update(ds);

						ds.Tables[0].Rows[0].Delete();
						adapter.Update(ds);

						ds.Tables[0].Rows[0].Delete();
						adapter.Update(ds);
					}
				}
			}
		}
	}
	#endregion
}
public class IBDataAdapterTestsDialect1 : IBDataAdapterTests
{
	public IBDataAdapterTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}

	[Ignore("Dialect 1 does not support Time")]
	public override void UpdateTimeTest()
	{

	}

	[Ignore("Dialect 1 does not support Time")]
	public override async Task UpdateTimeTestAsync()
	{

	}


}
