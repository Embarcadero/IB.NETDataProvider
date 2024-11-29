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
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;
using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
class IBExceptionTests : IBTestsBase
{
	public IBExceptionTests(IBServerType serverType)
		: base(serverType)
	{ }

	[Test]
	public async Task SQLSTATETest()
	{
		await using (var cmd = Connection.CreateCommand())
		{
			cmd.CommandText = "drop exception nonexisting";
			try
			{
				await cmd.ExecuteNonQueryAsync();
			}
			catch (IBException ex)
			{
				Assert.AreEqual("42000", ex.SQLSTATE);
			}
		}
	}
}
