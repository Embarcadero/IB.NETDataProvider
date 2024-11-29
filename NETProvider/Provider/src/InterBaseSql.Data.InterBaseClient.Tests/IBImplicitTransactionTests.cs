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
using System.Text;
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBImplicitTransactionTests : IBTestsBase
{
	#region Constructors

	public IBImplicitTransactionTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void DataAdapterFillTest()
	{
		var command = new IBCommand("select * from TEST where DATE_FIELD <> ?", Connection);
		var adapter = new IBDataAdapter(command);

		adapter.SelectCommand.Parameters.Add("@DATE_FIELD", IBDbType.Date, 4, "DATE_FIELD").Value = new DateTime(2003, 1, 5);

		var builder = new IBCommandBuilder(adapter);

		var ds = new DataSet();
		adapter.Fill(ds, "TEST");

		adapter.Dispose();
		builder.Dispose();
		command.Dispose();

		Assert.AreEqual(1, ds.Tables.Count);
		Assert.Greater(ds.Tables[0].Rows.Count, 0);
		Assert.Greater(ds.Tables[0].Columns.Count, 0);
	}

	[Test]
	public void ExecuteScalarTest()
	{
		var command = new IBCommand("select sum(int_field) from TEST", Connection);

		Assert.DoesNotThrow(() => command.ExecuteScalar());

		command.Dispose();
	}

	[Test]
	public void UpdatedClobFieldTest()
	{
		var command = new IBCommand("update TEST set clob_field = @clob_field where int_field = @int_field", Connection);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
		command.Parameters.Add("@clob_field", IBDbType.Text).Value = "Clob field update with implicit transaction";

		var i = command.ExecuteNonQuery();

		Assert.AreEqual(i, 1, "Clob field update with implicit transaction failed");

		command.Dispose();
	}

	[Test]
	public void UpdatedBlobFieldTest()
	{
		var command = new IBCommand("update TEST set blob_field = @blob_field where int_field = @int_field", Connection);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
		command.Parameters.Add("@blob_field", IBDbType.Binary).Value = Encoding.UTF8.GetBytes("Blob field update with implicit transaction");

		var i = command.ExecuteNonQuery();

		Assert.AreEqual(i, 1, "Blob field update with implicit transaction failed");

		command.Dispose();
	}

	[Test]
	public void UpdatedArrayFieldTest()
	{
		var values = new int[4];

		values[0] = 10;
		values[1] = 20;
		values[2] = 30;
		values[3] = 40;

		var command = new IBCommand("update TEST set iarray_field = @iarray_field where int_field = @int_field", Connection);
		command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
		command.Parameters.Add("@iarray_field", IBDbType.Array).Value = values;

		var i = command.ExecuteNonQuery();

		Assert.AreEqual(i, 1, "Array field update with implicit transaction failed");

		command.Dispose();
	}

	#endregion

	#region Async Test

	[Test]
	public async Task DataAdapterFillTestAsync()
	{
		await using (var command = new IBCommand("select * from TEST where DATE_FIELD <> ?", Connection))
		{
			using (var adapter = new IBDataAdapter(command))
			{
				adapter.SelectCommand.Parameters.Add("@DATE_FIELD", IBDbType.Date, 4, "DATE_FIELD").Value = new DateTime(2003, 1, 5);
				using (var ds = new DataSet())
				{
					adapter.Fill(ds, "TEST");

					Assert.AreEqual(1, ds.Tables.Count);
					Assert.Greater(ds.Tables[0].Rows.Count, 0);
					Assert.Greater(ds.Tables[0].Columns.Count, 0);
				}
			}
		}
	}

	[Test]
	public async Task ExecuteScalarTestAsync()
	{
		await using (var command = new IBCommand("select sum(int_field) from TEST", Connection))
		{
			Assert.DoesNotThrowAsync(command.ExecuteScalarAsync);
		}
	}

	[Test]
	public async Task UpdatedClobFieldTestAsync()
	{
		await using (var command = new IBCommand("update TEST set clob_field = @clob_field where int_field = @int_field", Connection))
		{
			command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
			command.Parameters.Add("@clob_field", IBDbType.Text).Value = "Clob field update with implicit transaction";
			var i = await command.ExecuteNonQueryAsync();

			Assert.AreEqual(i, 1, "Clob field update with implicit transaction failed");
		}
	}

	[Test]
	public async Task UpdatedBlobFieldTestAsync()
	{
		await using (var command = new IBCommand("update TEST set blob_field = @blob_field where int_field = @int_field", Connection))
		{
			command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
			command.Parameters.Add("@blob_field", IBDbType.Binary).Value = Encoding.UTF8.GetBytes("Blob field update with implicit transaction");
			var i = await command.ExecuteNonQueryAsync();

			Assert.AreEqual(i, 1, "Blob field update with implicit transaction failed");
		}
	}

	[Test]
	public async Task UpdatedArrayFieldTestAsync()
	{
		var values = new int[] { 10, 20, 30, 40 };
		await using (var command = new IBCommand("update TEST set iarray_field = @iarray_field where int_field = @int_field", Connection))
		{
			command.Parameters.Add("@int_field", IBDbType.Integer).Value = 1;
			command.Parameters.Add("@iarray_field", IBDbType.Array).Value = values;
			var i = await command.ExecuteNonQueryAsync();

			Assert.AreEqual(i, 1, "Array field update with implicit transaction failed");
		}
	}
	#endregion
}

public class IBImplicitTransactionTestsDialect1 : IBImplicitTransactionTests
{
	public IBImplicitTransactionTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}
}
