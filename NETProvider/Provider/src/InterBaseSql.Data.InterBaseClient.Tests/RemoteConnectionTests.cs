/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/Embarcadero/IB.NETDataProvider?tab=License-1-ov-file#readme.
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

//$Authors = Jeff Overcash

using InterBaseSql.Data.TestsBase;
using NUnit.Framework;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.Services;
using System.Threading.Tasks;

namespace InterBaseSql.Data.InterBaseClient.Tests;

[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]

// These tests will fail without first updating the constants to something that will work on your system.
public class RemoteConnection : IBTestsBase
{
	private string DBPath = "c:\\Embarcadero\\IB2017_64\\examples\\database\\employee_Copy.gdb";
	private string DBDir = "c:\\Embarcadero\\IB2017_64\\examples\\database\\";
	private string Host = "dev-machine";
	private string User = "sysdba";
	private string Password = "masterkey";
	private string Instance = "gds_db";
	private int Port = 3050;
	private int SSLPort = 4000;
	private string CertPath = "c:\\Certs\\ibservercafileserver.pem";

	private IBConnectionStringBuilder connectionString;

	public RemoteConnection(IBServerType serverType)
	: base(serverType)
	{ }

	[SetUp]
	public override void SetUp()
	{
		connectionString = new IBConnectionStringBuilder();
		connectionString.UserID = User;
		connectionString.Password = Password;
	}

	[TearDown]
	public override void TearDown() { }

	static void ServiceOutput(object sender, ServiceOutputEventArgs e)
	{
		var dummy = e.Message;
	}

	#region Non-Async Test

	private void RunTest()
	{
		var options = new ConnectionString(connectionString.ConnectionString);
		var db = new IBConnection(connectionString.ConnectionString);
		db.Open();
		db.Close();
	}

	[Test]
	public void FullInfoInDatabase()
	{
		connectionString.Database = $"{Host}:{DBPath}";
		RunTest();
	}

	[Test]
	[TestCase(3)]
	[TestCase(1)]
	public void NormalConnectionStringTest(int Dialect)
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.Dialect = Dialect;
		RunTest();
	}

	[Test]
	public void InstanceNameConnectionStringTest()
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.InstanceName = Instance;
		RunTest();
	}

	[Test]
	public void BadInstanceNameConnectionStringTest()
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.InstanceName = Instance + "_";
		Assert.Throws<InterBaseSql.Data.InterBaseClient.IBException> (() => RunTest());
	}

	[Test]
	public void FullDatabaseConnectionStringTest()
	{
		connectionString.DataSource = $"{Host}/{Port}:{DBPath}"; 
		RunTest();
	}

	[Test]
	public void DatabaseHostnamesWithPort()
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.Port = Port;
		RunTest();
	}

	[Test]
	public void SecureConnection()
	{
		connectionString.DataSource = "localhost";
		connectionString.Database = "Employee";
		connectionString.Port = SSLPort;
		connectionString.SSL = true;
		connectionString.ServerPublicFile = CertPath;
		RunTest();

	}

	[Test]
	public void SecureConnectionBadPort()
	{
		connectionString.DataSource = "localhost";
		connectionString.Database = "Employee";
		connectionString.Port = Port;
		connectionString.SSL = true;
		connectionString.ServerPublicFile = CertPath;
		Assert.Throws<InterBaseSql.Data.InterBaseClient.IBException>(() => RunTest());
	}

	[Test]
	public void CreateDatabase()
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBDir + "CreateTest.ib";
		connectionString.Port = Port;
		// Pooling false otherwise the pool will keep the connection active and can't drop the database
		connectionString.Pooling = false;
		IBConnection.CreateDatabase(connectionString.ConnectionString);
		//verify we can connect to the new database
		RunTest();
		IBConnection.DropDatabase(connectionString.ConnectionString);
	}

	[Test]
	public void CreateSecureDatabase()
	{
		connectionString.DataSource = "localhost";
		connectionString.Database = DBDir + "CreateTest.ib";
		connectionString.Port = SSLPort;
		connectionString.SSL = true;
		connectionString.ServerPublicFile = CertPath;
		// Pooling false otherwise the pool will keep the connection active and can't drop the database
		connectionString.Pooling = false;
		IBConnection.CreateDatabase(connectionString.ConnectionString);
		//verify we can connect to the new database
		RunTest();
		IBConnection.DropDatabase(connectionString.ConnectionString);
	}

	[Test]
	public void RemoteService()
	{
		var statisticalSvc = new IBStatistical();
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.Port = Port;
		statisticalSvc.ConnectionString = connectionString.ConnectionString;
		statisticalSvc.Options = IBStatisticalFlags.SystemTablesRelations;
		statisticalSvc.ServiceOutput += ServiceOutput;
		statisticalSvc.Execute();
	}

	[Test]
	public void RemoteServiceInstanceName()
	{
		var statisticalSvc = new IBStatistical();
		connectionString.DataSource = "localhost";
		connectionString.Database = "C:\\Embarcadero\\IB2017_64\\examples\\database\\intlemp.gdb";
		connectionString.InstanceName = "IB2017";
		connectionString.Port = 0;
		statisticalSvc.ConnectionString = connectionString.ConnectionString;
		statisticalSvc.Options = IBStatisticalFlags.SystemTablesRelations;
		statisticalSvc.ServiceOutput += ServiceOutput;
		statisticalSvc.Execute();
	}

	[Test]
	public void RemoteServiceBadInstanceName()
	{
		var statisticalSvc = new IBStatistical();
		connectionString.DataSource = "localhost";
		connectionString.Database = "C:\\Embarcadero\\IB2017_64\\examples\\database\\intlemp.gdb";
		connectionString.InstanceName = "IB2017_";
		connectionString.Port = 0;
		statisticalSvc.ConnectionString = connectionString.ConnectionString;
		statisticalSvc.Options = IBStatisticalFlags.SystemTablesRelations;
		statisticalSvc.ServiceOutput += ServiceOutput;
		Assert.Throws<InterBaseSql.Data.InterBaseClient.IBException>(() => statisticalSvc.Execute());
	}

	#endregion

	#region Async Test

	private async Task RunTestAsync()
	{
		var options = new ConnectionString(connectionString.ConnectionString);
		var db = new IBConnection(connectionString.ConnectionString);
		await db.OpenAsync();
		await db.CloseAsync();
	}

	public async Task FullInfoInDatabaseAsync()
	{
		connectionString.Database = $"{Host}:{DBPath}";
		await RunTestAsync();
	}

	[Test]
	[TestCase(3)]
	[TestCase(1)]
	public async Task NormalConnectionStringTestAsync(int Dialect)
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.Dialect = Dialect;
		await RunTestAsync();
	}

	[Test]
	public async Task InstanceNameConnectionStringTestAsync()
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.InstanceName = Instance;
		await RunTestAsync();
	}

	[Test]
	public async Task FullDatabaseConnectionStringTestAsync()
	{
		connectionString.DataSource = $"{Host}/{Port}:{DBPath}";
		await RunTestAsync();
	}

	[Test]
	public async Task DatabaseHostnamesWithPortAsync()
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.Port = Port;
		await RunTestAsync();
	}

	[Test]
	public async Task SecureConnectionAsync()
	{
		connectionString.DataSource = "localhost";
		connectionString.Database = "Employee";
		connectionString.Port = SSLPort;
		connectionString.SSL = true;
		connectionString.ServerPublicFile = CertPath;
		await RunTestAsync();

	}

	[Test]
	public async Task CreateDatabaseAsync()
	{
		connectionString.DataSource = Host;
		connectionString.Database = DBDir + "CreateTest.ib";
		connectionString.Port = Port;
		// Pooling false otherwise the pool will keep the connection active and can't drop the database
		connectionString.Pooling = false;
		await IBConnection.CreateDatabaseAsync(connectionString.ConnectionString);
		//verify we can connect to the new database
		RunTest();
		await IBConnection.DropDatabaseAsync(connectionString.ConnectionString);
	}

	[Test]
	public async Task CreateSecureDatabaseAsync()
	{
		connectionString.DataSource = "localhost";
		connectionString.Database = DBDir + "CreateTest.ib";
		connectionString.Port = SSLPort;
		connectionString.SSL = true;
		connectionString.ServerPublicFile = CertPath;
		// Pooling false otherwise the pool will keep the connection active and can't drop the database
		connectionString.Pooling = false;
		await IBConnection.CreateDatabaseAsync(connectionString.ConnectionString);
		//verify we can connect to the new database
		RunTest();
		await IBConnection.DropDatabaseAsync(connectionString.ConnectionString);
	}

	[Test]
	public async Task RemoteServiceAsync()
	{
		var statisticalSvc = new IBStatistical();
		connectionString.DataSource = Host;
		connectionString.Database = DBPath;
		connectionString.Port = Port;
		statisticalSvc.ConnectionString = connectionString.ConnectionString;
		statisticalSvc.Options = IBStatisticalFlags.SystemTablesRelations;
		statisticalSvc.ServiceOutput += ServiceOutput;
		await statisticalSvc.ExecuteAsync();
	}

	[Test]
	public async Task RemoteServiceInstanceNameAsync()
	{
		var statisticalSvc = new IBStatistical();
		connectionString.DataSource = "localhost";
		connectionString.Database = "C:\\Embarcadero\\IB2017_64\\examples\\database\\intlemp.gdb";
		connectionString.InstanceName = "IB2017";
		connectionString.Port = 0;
		statisticalSvc.ConnectionString = connectionString.ConnectionString;
		statisticalSvc.Options = IBStatisticalFlags.SystemTablesRelations;
		statisticalSvc.ServiceOutput += ServiceOutput;
		await statisticalSvc.ExecuteAsync();
	}

	#endregion

	public class RemoteConnectionDialect1 : RemoteConnection
	{
		public RemoteConnectionDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1;
		}
	}


}