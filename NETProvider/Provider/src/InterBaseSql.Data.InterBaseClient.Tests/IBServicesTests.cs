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
using System.CodeDom;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using InterBaseSql.Data.Services;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	public class IBServicesTests : IBTestsBase
	{
		#region Constructors

		public IBServicesTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region Setup Method

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			if (Connection != null && Connection.State == ConnectionState.Open)
			{
				Connection.Close();
			}
		}

		#endregion

		#region Unit Tests

		[Test]
		public void BackupRestoreTest()
		{
//			var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var path = AppDomain.CurrentDomain.BaseDirectory;
			var backupName = $"{path}{Guid.NewGuid().ToString()}.bak";
			void BackupPart()
			{
				var backupSvc = new IBBackup();

				backupSvc.ConnectionString = BuildServicesConnectionString(IBServerType, true);
				backupSvc.Options = IBBackupFlags.IgnoreLimbo;
				backupSvc.BackupFiles.Add(new IBBackupFile(backupName));
				backupSvc.Verbose = true;

				backupSvc.ServiceOutput += ServiceOutput;

				backupSvc.Execute();
			}
			void RestorePart()
			{
				var restoreSvc = new IBRestore();

				restoreSvc.ConnectionString = BuildServicesConnectionString(IBServerType, true);
				restoreSvc.Options = IBRestoreFlags.Create | IBRestoreFlags.Replace;
				restoreSvc.PageSize = IBTestsSetup.PageSize;
				restoreSvc.Verbose = true;
				restoreSvc.BackupFiles.Add(new IBBackupFile(backupName));

				restoreSvc.ServiceOutput += ServiceOutput;

				restoreSvc.Execute();
			}
			BackupPart();
			RestorePart();
			// test the database was actually restored fine
			Connection.Open();
			Connection.Close();
			File.Delete(backupName);
		}

		[Test]
		public void ValidationTest()
		{
			var validationSvc = new IBValidation();

			validationSvc.ConnectionString = BuildServicesConnectionString(IBServerType, true);
			validationSvc.Options = IBValidationFlags.ValidateDatabase;

			validationSvc.ServiceOutput += ServiceOutput;

			validationSvc.Execute();
		}

		[Test]
		public void SweepTest()
		{
			var validationSvc = new IBValidation();

			validationSvc.ConnectionString = BuildServicesConnectionString(IBServerType, true);
			validationSvc.Options = IBValidationFlags.SweepDatabase;

			validationSvc.ServiceOutput += ServiceOutput;

			validationSvc.Execute();
		}

		[Test]
		public void SetPropertiesTest()
		{
			var configurationSvc = new IBConfiguration();

			configurationSvc.ConnectionString = BuildServicesConnectionString(IBServerType, true);

			configurationSvc.SetSweepInterval(1000);
			configurationSvc.SetReserveSpace(true);
			configurationSvc.SetForcedWrites(true);
		}

		[Test]
		public void ShutdownOnlineTest()
		{
			var configurationSvc = new IBConfiguration();

			configurationSvc.ConnectionString = BuildServicesConnectionString(IBServerType, true);

			configurationSvc.DatabaseShutdown(IBShutdownMode.Forced, 10);
			configurationSvc.DatabaseOnline();
		}

		[Test]
		public void StatisticsTest()
		{
			var statisticalSvc = new IBStatistical();

			statisticalSvc.ConnectionString = BuildServicesConnectionString(IBServerType, true);
			statisticalSvc.Options = IBStatisticalFlags.SystemTablesRelations;

			statisticalSvc.ServiceOutput += ServiceOutput;

			statisticalSvc.Execute();
		}

		[Test]
		public void IBLogTest()
		{
			var logSvc = new IBLog();

			logSvc.ConnectionString = BuildServicesConnectionString(IBServerType, false);

			logSvc.ServiceOutput += ServiceOutput;

			logSvc.Execute();
		}

		[Test]
		public void AddDeleteUserTest()
		{
			var securitySvc = new IBSecurity();

			securitySvc.ConnectionString = BuildServicesConnectionString(IBServerType, false);

			try
			{  // Cleanup user if left behind, ignore exceptions
				var user = new IBUserData();
				user.UserName = "new_user";
				securitySvc.DeleteUser(user);
			}
			catch
			{ }
			try
			{
				var user = new IBUserData();
				user.UserName = "new_user";
				user.UserPassword = "1";
				securitySvc.AddUser(user);
				user = securitySvc.DisplayUser("NEW_USER");
				Assert.AreEqual(user.UserName, "NEW_USER");
			}
			finally
			{
				var user = new IBUserData();
				user.UserName = "new_user";
				securitySvc.DeleteUser(user);
				user = securitySvc.DisplayUser("NEW_USER");
				Assert.IsNull(user);
			}
		}

		[Test]
		public void DisplayUser()
		{
			var securitySvc = new IBSecurity();

			securitySvc.ConnectionString = BuildServicesConnectionString(IBServerType, false);

			var user = securitySvc.DisplayUser("SYSDBA");
			Assert.AreEqual(user.UserName, "SYSDBA");
		}

		[Test]
		public void DisplayUsers()
		{
			var securitySvc = new IBSecurity();

			securitySvc.ConnectionString = BuildServicesConnectionString(IBServerType, false);

			var users = securitySvc.DisplayUsers();
			Assert.IsNotNull(users);
		}

		[Test]
		public void ServerPropertiesTest()
		{
			var serverProp = new IBServerProperties();

			serverProp.ConnectionString = BuildServicesConnectionString(IBServerType, false);

			foreach (var m in serverProp.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => !x.IsSpecialName))
			{
				Assert.DoesNotThrow(() => m.Invoke(serverProp, null), m.Name);
			}
		}

		#endregion

		#region Methods

		static void ServiceOutput(object sender, ServiceOutputEventArgs e)
		{
			var dummy = e.Message;
		}

		#endregion
	}

	public class IBServicesTestsDialect1 : IBServicesTests
	{
		public IBServicesTestsDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1;
		}
	}
}
