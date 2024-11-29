using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
public class IBEventTests : IBTestsBase
{
	#region Constructors

	public IBEventTests(IBServerType serverType)
		: base(serverType)
	{ }

	#endregion

	[Test]
	public void RegisterEvents()
	{
		var list = new List<string> { "1", "22" };
		var events = new IBEvents();
		events.Events = list;
		events.RegisterEvents();
	}


}

public class IBEventTestsDialect1 : IBEventTests
{
	public IBEventTestsDialect1(IBServerType serverType)
		: base(serverType)
	{
		IBTestsSetup.Dialect = 1;
	}
}
