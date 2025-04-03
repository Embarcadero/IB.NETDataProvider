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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Globalization;
using System.Threading;
using InterBaseSql.Data.Common;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests;

public class ConnectionStringTests
{
	[Test]
	[TestCase(3)]
	[TestCase(1)]
	public void ParsingNormalConnectionStringTest(int Dialect)
	{
		string ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=testpwd;dialect=" + Dialect.ToString();
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("testserver", cs.DataSource);
		Assert.AreEqual("testdb.ib", cs.Database);
		Assert.AreEqual("testuser", cs.UserID);
		Assert.AreEqual("testpwd", cs.Password);
		Assert.AreEqual(Dialect, cs.Dialect);
	}

	[Test]
	public void ParsingEUAEnabledandInstanceNameConnectionStringTest()
	{
		const string ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=testpwd;EUAEnabled=true;instance name=IB2017";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual(true, cs.EUAEnabled);
		Assert.AreEqual("IB2017", cs.InstanceName);
	}

	[Test]
	public void ParsingTruncateCharConnectionStringTest()
	{
		string ConnectionString;
		ConnectionString cs;
		ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=testpwd";
		cs = new ConnectionString(ConnectionString);
		Assert.AreEqual(false, cs.TruncateChar);

		ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=testpwd;truncate_char";
		cs = new ConnectionString(ConnectionString);
		Assert.AreEqual(false, cs.TruncateChar);

		ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=testpwd;truncate_char=true";
		cs = new ConnectionString(ConnectionString);
		Assert.AreEqual(true, cs.TruncateChar);

		ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=testpwd;truncate_char=false";
		cs = new ConnectionString(ConnectionString);
		Assert.AreEqual(false, cs.TruncateChar);
	}

	[Test]
	public void ParsingFullDatabaseConnectionStringTest()
	{
		const string ConnectionString = "database=testserver/1234:testdb.ib;user=testuser;password=testpwd";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("testserver", cs.DataSource);
		Assert.AreEqual("testdb.ib", cs.Database);
		Assert.AreEqual("testuser", cs.UserID);
		Assert.AreEqual("testpwd", cs.Password);
		Assert.AreEqual(1234, cs.Port);
	}

	[Test]
	public void ParsingSingleQuotedConnectionStringTest()
	{
		const string ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=test'pwd";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("test'pwd", cs.Password);
	}

	[Test]
	public void ParsingDoubleQuotedConnectionStringTest()
	{
		const string ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=test\"pwd";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("test\"pwd", cs.Password);
	}

	[Test]
	public void ParsingSpacesInKeyConnectionStringTest()
	{
		const string ConnectionString = "data source=testserver";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("testserver", cs.DataSource);
	}

	[Test]
	public void ParsingOneCharValueConnectionStringTest()
	{
		const string ConnectionString = "connection lifetime=6";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual(6, cs.ConnectionLifetime);
	}

	[Test]
	public void ParsingWithEndingSemicolonConnectionStringTest()
	{
		const string ConnectionString = "user=testuser;password=testpwd;";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("testuser", cs.UserID);
		Assert.AreEqual("testpwd", cs.Password);
	}

	[Test]
	public void ParsingWithoutEndingSemicolonConnectionStringTest()
	{
		const string ConnectionString = "user=testuser;password=testpwd";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("testuser", cs.UserID);
		Assert.AreEqual("testpwd", cs.Password);
	}

	[Test]
	public void ParsingMultilineConnectionStringTest()
	{
		const string ConnectionString = @"DataSource=S05-04;
 User=SYSDBA;
 Password=masterkey;
 Role=;
 Database=Termine;
 Port=3050;
 Dialect=3;
 Charset=ISO8859_1;
 Connection lifetime=0;
 Connection timeout=15;
 Pooling=True;
 Packet Size=8192;";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("Termine", cs.Database);
		Assert.AreEqual("", cs.Role);
	}

	[Test]
	public void NormalizedConnectionStringIgnoresCultureTest()
	{
		const string ConnectionString = "datasource=testserver;database=testdb.ib;user=testuser;password=testpwd";
		var cs = new ConnectionString(ConnectionString);
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
		var s1 = cs.NormalizedConnectionString;
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("cs-CZ");
		var s2 = cs.NormalizedConnectionString;

		Assert.AreEqual(s1, s2);
	}

	[Test]
	public void ParsingWithEmptyKeyConnectionStringTest()
	{
		const string ConnectionString = "user=;password=testpwd";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("", cs.UserID);
		Assert.AreEqual("testpwd", cs.Password);
	}

	[Test]
	public void ParsingWithWhiteSpacesKeyConnectionStringTest()
	{
		const string ConnectionString = "user= \t;password=testpwd";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("", cs.UserID);
		Assert.AreEqual("testpwd", cs.Password);
	}

	[Test]
	public void ParsingDatabaseOldStyleHostnameWithoutPortWithoutPath()
	{
		const string ConnectionString = "database=hostname:test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleHostnameWithoutPortRootPath()
	{
		const string ConnectionString = "database=hostname:/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleHostnameWithoutPortDrivePath()
	{
		const string ConnectionString = "database=hostname:C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP4WithoutPortWithoutPath()
	{
		const string ConnectionString = "database=127.0.0.1:test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP4WithoutPortRootPath()
	{
		const string ConnectionString = "database=127.0.0.1:/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP4WithoutPortDrivePath()
	{
		const string ConnectionString = "database=127.0.0.1:C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP6WithoutPortWithoutPath()
	{
		const string ConnectionString = "database=::1:test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP6WithoutPortRootPath()
	{
		const string ConnectionString = "database=::1:/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP6WithoutPortDrivePath()
	{
		const string ConnectionString = "database=::1:C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseOldStyleHostnameWithPortWithoutPath()
	{
		const string ConnectionString = "database=hostname/6666:test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleHostnameWithPortRootPath()
	{
		const string ConnectionString = "database=hostname/6666:/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleHostnameWithPortDrivePath()
	{
		const string ConnectionString = "database=hostname/6666:C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP4WithPortWithoutPath()
	{
		const string ConnectionString = "database=127.0.0.1/6666:test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP4WithPortRootPath()
	{
		const string ConnectionString = "database=127.0.0.1/6666:/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP4WithPortDrivePath()
	{
		const string ConnectionString = "database=127.0.0.1/6666:C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP6WithPortWithoutPath()
	{
		const string ConnectionString = "database=::1/6666:test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP6WithPortRootPath()
	{
		const string ConnectionString = "database=::1/6666:/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseOldStyleIP6WithPortDrivePath()
	{
		const string ConnectionString = "database=::1/6666:C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleHostnameWithoutPortWithoutPath()
	{
		const string ConnectionString = "database=//hostname/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleHostnameWithoutPortRootPath()
	{
		const string ConnectionString = "database=//hostname//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleHostnameWithoutPortDrivePath()
	{
		const string ConnectionString = "database=//hostname/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP4WithoutPortWithoutPath()
	{
		const string ConnectionString = "database=//127.0.0.1/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP4WithoutPortRootPath()
	{
		const string ConnectionString = "database=//127.0.0.1//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP4WithoutPortDrivePath()
	{
		const string ConnectionString = "database=//127.0.0.1/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP6WithoutPortWithoutPath()
	{
		const string ConnectionString = "database=//::1/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP6WithoutPortRootPath()
	{
		const string ConnectionString = "database=//::1//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP6WithoutPortDrivePath()
	{
		const string ConnectionString = "database=//::1/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNewStyleHostnameWithPortWithoutPath()
	{
		const string ConnectionString = "database=//hostname:6666/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleHostnameWithPortRootPath()
	{
		const string ConnectionString = "database=//hostname:6666//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleHostnameWithPortDrivePath()
	{
		const string ConnectionString = "database=//hostname:6666/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP4WithPortWithoutPath()
	{
		const string ConnectionString = "database=//127.0.0.1:6666/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP4WithPortRootPath()
	{
		const string ConnectionString = "database=//127.0.0.1:6666//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP4WithPortDrivePath()
	{
		const string ConnectionString = "database=//127.0.0.1:6666/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP6WithPortWithoutPath()
	{
		const string ConnectionString = "database=//[::1]:6666/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP6WithPortRootPath()
	{
		const string ConnectionString = "database=//[::1]:6666//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseNewStyleIP6WithPortDrivePath()
	{
		const string ConnectionString = "database=//[::1]:6666/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleHostnameWithoutPortWithoutPath()
	{
		const string ConnectionString = "database=inet://hostname/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleHostnameWithoutPortRootPath()
	{
		const string ConnectionString = "database=inet://hostname//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleHostnameWithoutPortDrivePath()
	{
		const string ConnectionString = "database=inet://hostname/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP4WithoutPortWithoutPath()
	{
		const string ConnectionString = "database=inet://127.0.0.1/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP4WithoutPortRootPath()
	{
		const string ConnectionString = "database=inet://127.0.0.1//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP4WithoutPortDrivePath()
	{
		const string ConnectionString = "database=inet://127.0.0.1/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP6WithoutPortWithoutPath()
	{
		const string ConnectionString = "database=inet://::1/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP6WithoutPortRootPath()
	{
		const string ConnectionString = "database=inet://::1//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP6WithoutPortDrivePath()
	{
		const string ConnectionString = "database=inet://::1/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleHostnameWithPortWithoutPath()
	{
		const string ConnectionString = "database=inet://hostname:6666/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleHostnameWithPortRootPath()
	{
		const string ConnectionString = "database=inet://hostname:6666//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleHostnameWithPortDrivePath()
	{
		const string ConnectionString = "database=inet://hostname:6666/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("hostname", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP4WithPortWithoutPath()
	{
		const string ConnectionString = "database=inet://127.0.0.1:6666/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP4WithPortRootPath()
	{
		const string ConnectionString = "database=inet://127.0.0.1:6666//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP4WithPortDrivePath()
	{
		const string ConnectionString = "database=inet://127.0.0.1:6666/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("127.0.0.1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP6WithPortWithoutPath()
	{
		const string ConnectionString = "database=inet://[::1]:6666/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP6WithPortRootPath()
	{
		const string ConnectionString = "database=inet://[::1]:6666//test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleIP6WithPortDrivePath()
	{
		const string ConnectionString = "database=inet://[::1]:6666/C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("::1", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(6666, cs.Port);
	}

	[Test]
	public void ParsingDatabaseURLStyleWithoutHostnameWithoutPath()
	{
		const string ConnectionString = "database=inet:///test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("localhost", cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleWithoutHostnameRootPath()
	{
		const string ConnectionString = "database=inet:////test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("localhost", cs.DataSource);
		Assert.AreEqual("/test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseURLStyleWithoutHostnameDrivePath()
	{
		const string ConnectionString = "database=inet:///C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("localhost", cs.DataSource);
		Assert.AreEqual("C:\\test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseNoStyleWithoutPath()
	{
		const string ConnectionString = "database=test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("test.ib", cs.Database);
		Assert.AreEqual(string.Empty, cs.DataSource);
	}

	[Test]
	public void ParsingDatabaseNoStyleRootPath()
	{
		const string ConnectionString = "database=/test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("/test.ib", cs.Database);
		Assert.AreEqual(string.Empty, cs.DataSource);
	}

	[Test]
	public void ParsingDatabaseNoStyleDrivePath()
	{
		const string ConnectionString = "database=C:\\test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual("C:\\test.ib", cs.Database);
		Assert.AreEqual(string.Empty, cs.DataSource);
	}

	[TestCase("test")]
	[TestCase("test12")]
	[TestCase("32test")]
	[TestCase("test-12")]
	public void ParsingDatabaseHostnames(string hostname)
	{
		var ConnectionString = $"database={hostname}:test.ib";
		var cs = new ConnectionString(ConnectionString);
		Assert.AreEqual(hostname, cs.DataSource);
		Assert.AreEqual("test.ib", cs.Database);
	}

	[Test]
	public void ParsingDatabaseHostnamesWithPort()
	{
		var ConnectionString = $"database=localhost/3050:<DB PATH>;user=TEXDBA;password=texdba;port=3050;charset=None;";
		var cs = new ConnectionString(ConnectionString);
		cs.Validate();
		Assert.AreEqual("localhost", cs.DataSource);
		Assert.AreEqual("<DB PATH>", cs.Database);
		Assert.AreEqual(3050, cs.Port);
		Assert.AreEqual("TEXDBA", cs.UserID);
		Assert.AreEqual("texdba", cs.Password);
		Assert.AreEqual("None", cs.Charset); 
	}

	[Test]
	public void ParsingServerHostnamesWithPort()
	{
		var ConnectionString = $"Server=localhost/3050:<DB PATH>;user=TEXDBA;password=texdba;port=3050;charset=None;";
		var cs = new ConnectionString(ConnectionString);
		cs.Validate();
		Assert.AreEqual("localhost", cs.DataSource);
		Assert.AreEqual("<DB PATH>", cs.Database);
		Assert.AreEqual(3050, cs.Port);
		Assert.AreEqual("TEXDBA", cs.UserID);
		Assert.AreEqual("texdba", cs.Password);
		Assert.AreEqual("None", cs.Charset);
	}

	[Test]
	public void ParseSSLInfo()
	{
		var connectionString = $"data source=dev-machine;initial catalog=c:\\Embarcadero\\IB2017_64\\examples\\database\\employee_Copy.gdb;user id=sysdba;password=masterkey;SSL=True;ServerPublicFile=PF;ClientCertFile=CCF;ClientPassPhraseFile=CPPF";
		var cs = new ConnectionString(connectionString);
		cs.Validate();
		Assert.AreEqual("dev-machine", cs.DataSource);
		Assert.AreEqual("c:\\Embarcadero\\IB2017_64\\examples\\database\\employee_Copy.gdb", cs.Database);
		Assert.AreEqual(true, cs.SSL);
		Assert.AreEqual("PF", cs.ServerPublicFile);
		Assert.AreEqual("CCF", cs.ClientCertFile);
		Assert.AreEqual("CPPF", cs.ClientPassPhraseFile);
		connectionString = $"data source=dev-machine;initial catalog=c:\\Embarcadero\\IB2017_64\\examples\\database\\employee_Copy.gdb;user id=sysdba;password=masterkey;SSL=True;ServerPublicPath=PP;ClientCertFile=CCF;ClientPassPhrase=CPP";
		cs = new ConnectionString(connectionString);
		cs.Validate();
		Assert.AreEqual("PP", cs.ServerPublicPath);
		Assert.AreEqual("CPP", cs.ClientPassPhrase);
	}
	[Test]
	public void ParseSSLMutualExclusivity()
	{
		var connectionString = $"ServerPublicPath=PP;ServerPublicFile=PF";
		var cs = new ConnectionString(connectionString);
		Assert.Throws<ArgumentException>(() => cs.Validate());
		connectionString = $"ServerPublicPath=PP;ClientCertFile=CCF;ClientPassPhraseFile=CPPF";
		cs = new ConnectionString(connectionString);
		Assert.Throws<ArgumentException>(() => cs.Validate());
	}

}
