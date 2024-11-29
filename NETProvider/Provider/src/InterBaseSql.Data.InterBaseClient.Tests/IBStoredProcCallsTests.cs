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
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBStoredProcCallsTests : IBTestsBase
{
	#region Constructors

	public IBStoredProcCallsTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void InterBaseLikeTest00()
	{
		var command = new IBCommand("EXECUTE PROCEDURE GETVARCHARFIELD(?)", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters.Add("@VARCHAR_FIELD", IBDbType.VarChar).Direction = ParameterDirection.Output;

		command.Parameters[0].Value = 1;

		command.ExecuteNonQuery();

		var value = command.Parameters[1].Value;

		command.Dispose();

		Assert.AreEqual("IRow Number 1", value);
	}

	[Test]
	public void InterBaseLikeTest01()
	{
		var command = new IBCommand("SELECT * FROM GETVARCHARFIELD(?)", Connection);
		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters[0].Value = 1;

		var reader = command.ExecuteReader();
		reader.Read();
		var value = reader[0];
		reader.Close();

		command.Dispose();

		Assert.AreEqual("IRow Number 1", value);
	}

	[Test]
	public void SqlServerLikeTest00()
	{
		var command = new IBCommand("GETVARCHARFIELD", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters.Add("@VARCHAR_FIELD", IBDbType.VarChar).Direction = ParameterDirection.Output;

		command.Parameters[0].Value = 1;

		command.ExecuteNonQuery();

		var value = command.Parameters[1].Value;

		command.Dispose();

		Assert.AreEqual("IRow Number 1", value);
	}

	[Test]
	public void SqlServerLikeTest01()
	{
		var command = new IBCommand("GETRECORDCOUNT", Connection);
		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@RECORDCOUNT", IBDbType.Integer).Direction = ParameterDirection.Output;

		command.ExecuteNonQuery();

		var value = command.Parameters[0].Value;

		command.Dispose();

		Assert.Greater(Convert.ToInt32(value), 0);
	}

	[Test]
	public void SqlServerLikeTest02()
	{
		var command = new IBCommand("GETVARCHARFIELD", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Value = 1;

		var r = command.ExecuteReader();

		var count = 0;

		while (r.Read())
		{
			count++;
		}

		r.Close();

		command.Dispose();

		Assert.AreEqual(1, count);
	}

	#endregion
	#region Async Unit Tests

	[Test]
	public async Task InterBaseLikeTest00Async()
	{
		var command = new IBCommand("EXECUTE PROCEDURE GETVARCHARFIELD(?)", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters.Add("@VARCHAR_FIELD", IBDbType.VarChar).Direction = ParameterDirection.Output;

		command.Parameters[0].Value = 1;

		await command.ExecuteNonQueryAsync();

		var value = command.Parameters[1].Value;

		await command.DisposeAsync();

		Assert.AreEqual("IRow Number 1", value);
	}

	[Test]
	public async Task InterBaseLikeTest01Async()
	{
		var command = new IBCommand("SELECT * FROM GETVARCHARFIELD(?)", Connection);
		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters[0].Value = 1;

		var reader = await command.ExecuteReaderAsync();
		await reader.ReadAsync();
		var value = reader[0];
		await reader.CloseAsync();

		await command.DisposeAsync();

		Assert.AreEqual("IRow Number 1", value);
	}

	[Test]
	public async Task SqlServerLikeTest00Async()
	{
		var command = new IBCommand("GETVARCHARFIELD", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Direction = ParameterDirection.Input;
		command.Parameters.Add("@VARCHAR_FIELD", IBDbType.VarChar).Direction = ParameterDirection.Output;

		command.Parameters[0].Value = 1;

		await command.ExecuteNonQueryAsync();

		var value = command.Parameters[1].Value;

		await command.DisposeAsync();

		Assert.AreEqual("IRow Number 1", value);
	}

	[Test]
	public async Task SqlServerLikeTest01Async()
	{
		var command = new IBCommand("GETRECORDCOUNT", Connection);
		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@RECORDCOUNT", IBDbType.Integer).Direction = ParameterDirection.Output;

		await command.ExecuteNonQueryAsync();

		var value = command.Parameters[0].Value;

		await command.DisposeAsync();

		Assert.Greater(Convert.ToInt32(value), 0);
	}

	[Test]
	public async Task SqlServerLikeTest02Async()
	{
		var command = new IBCommand("GETVARCHARFIELD", Connection);

		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@ID", IBDbType.VarChar).Value = 1;

		var r = await command.ExecuteReaderAsync();

		var count = 0;

		while (await r.ReadAsync())
		{
			count++;
		}

		await r.CloseAsync();

		await command.DisposeAsync();

		Assert.AreEqual(1, count);
	}

	#endregion
}
public class IBStoredProcCallsTestsDialect1 : IBStoredProcCallsTests
{
	public IBStoredProcCallsTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}
}
