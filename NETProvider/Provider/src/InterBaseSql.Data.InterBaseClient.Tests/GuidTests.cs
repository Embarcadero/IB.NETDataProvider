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
using System.Security.Cryptography;
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]

public class GuidTests : IBTestsBase
{
	#region Constructors

	public GuidTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Non-Async Unit Tests

	[Test]
	public void InsertGuidTest()
	{
		var newGuid = Guid.Empty;
		var guidValue = Guid.NewGuid();

		// Insert the Guid
		var insert = new IBCommand("INSERT INTO GUID_TEST (GUID_FIELD) VALUES (@GuidValue)", Connection);
		insert.Parameters.Add("@GuidValue", IBDbType.Guid).Value = guidValue;
		insert.ExecuteNonQuery();
		insert.Dispose();

		// Select the value
		using (var select = new IBCommand("SELECT * FROM GUID_TEST", Connection))
		using (var r = select.ExecuteReader())
		{
			if (r.Read())
			{
				newGuid = r.GetGuid(1);
			}
		}

		Assert.AreEqual(guidValue, newGuid);
	}

	[Test]
	public void InsertNullGuidTest()
	{
		// Insert the Guid
		var id = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
		var insert = new IBCommand("INSERT INTO GUID_TEST (INT_FIELD, GUID_FIELD) VALUES (@IntField, @GuidValue)", Connection);
		insert.Parameters.Add("@IntField", IBDbType.Integer).Value = id;
		insert.Parameters.Add("@GuidValue", IBDbType.Guid).Value = DBNull.Value;
		insert.ExecuteNonQuery();
		insert.Dispose();

		// Select the value
		using (var select = new IBCommand("SELECT * FROM GUID_TEST WHERE INT_FIELD = @IntField", Connection))
		{
			select.Parameters.Add("@IntField", IBDbType.Integer).Value = id;
			using (var r = select.ExecuteReader())
			{
				if (r.Read())
				{
					if (!r.IsDBNull(1))
					{
						Assert.Fail();
					}
				}
			}
		}
	}

	#endregion

	#region Async Unit Tests

	[Test]
	public async Task InsertGuidTestAsync()
	{
		var newGuid = Guid.Empty;
		var guidValue = Guid.NewGuid();

		await using (var insert = new IBCommand("INSERT INTO GUID_TEST (GUID_FIELD) VALUES (@GuidValue)", Connection))
		{
			insert.Parameters.Add("@GuidValue", IBDbType.Guid).Value = guidValue;
			await insert.ExecuteNonQueryAsync();
		}

		await using (var select = new IBCommand("SELECT * FROM GUID_TEST", Connection))
		{
			await using (var r = await select.ExecuteReaderAsync())
			{
				if (await r.ReadAsync())
				{
					newGuid = r.GetGuid(1);
				}
			}
		}

		Assert.AreEqual(guidValue, newGuid);
	}

	[Test]
	public async Task InsertNullGuidTestAsync()
	{
		var id = GetId();

		await using (var insert = new IBCommand("INSERT INTO GUID_TEST (INT_FIELD, GUID_FIELD) VALUES (@IntField, @GuidValue)", Connection))
		{
			insert.Parameters.Add("@IntField", IBDbType.Integer).Value = id;
			insert.Parameters.Add("@GuidValue", IBDbType.Guid).Value = DBNull.Value;
			await insert.ExecuteNonQueryAsync();
		}

		await using (var select = new IBCommand("SELECT * FROM GUID_TEST WHERE INT_FIELD = @IntField", Connection))
		{
			select.Parameters.Add("@IntField", IBDbType.Integer).Value = id;
			await using (var r = await select.ExecuteReaderAsync())
			{
				if (await r.ReadAsync())
				{
					if (!await r.IsDBNullAsync(1))
					{
						Assert.Fail();
					}
				}
			}
		}
	}
	#endregion
}
public class GuidTestsDialect1 : GuidTests
{
	public GuidTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}

}
