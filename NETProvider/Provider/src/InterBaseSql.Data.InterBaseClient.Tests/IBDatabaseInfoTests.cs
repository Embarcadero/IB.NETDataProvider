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

using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBDatabaseInfoTests : IBTestsBase
{
	#region Constructors

	public IBDatabaseInfoTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	#region Unit Tests

	[Test]
	public void CompleteDatabaseInfoTest()
	{
		var dbInfo = new IBDatabaseInfo(Connection);

		foreach (var m in dbInfo.GetType()
			.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
			.Where(x => !x.IsSpecialName)
			.Where(x => !x.Name.EndsWith("Async")))
		{
			Assert.DoesNotThrow(() => m.Invoke(dbInfo, null), m.Name);
		}
	}

	[Test]
	public void CompleteDatabaseInfoTestAsync()
	{
		var dbInfo = new IBDatabaseInfo(Connection);

		foreach (var m in dbInfo.GetType()
			.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
			.Where(x => !x.IsSpecialName)
			.Where(x => x.Name.EndsWith("Async")))
		{
			Assert.DoesNotThrowAsync(() => (Task) m.Invoke(dbInfo, new object[] { CancellationToken.None }), m.Name);
		}
	}

	[Test]
	public void DBSQLDialect()
	{
		var dbInfo = new IBDatabaseInfo(Connection);
		Assert.AreEqual(IBTestsSetup.Dialect, dbInfo.GetDBSQLDialect());
	}

	#endregion
}

public class IBDatabaseInfoTestsDialect1 : IBDatabaseInfoTests
{
	public IBDatabaseInfoTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}

}
