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
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class IBTransactionTests : IBTestsBase
	{
		#region Constructors

		public IBTransactionTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region Unit Tests

		[Test]
		public void CommitTest()
		{
			Transaction = Connection.BeginTransaction();
			Transaction.Commit();
		}

		[Test]
		public void RollbackTest()
		{
			Transaction = Connection.BeginTransaction();
			Transaction.Rollback();
		}

		[Test]
		public void SavePointTest()
		{
			using (var command = new IBCommand())
			{
				Transaction = Connection.BeginTransaction("InitialSavePoint");

				command.Connection = Connection;
				command.Transaction = Transaction;

				command.CommandText = "insert into TEST (INT_FIELD) values (200) ";
				command.ExecuteNonQuery();

				Transaction.Save("FirstSavePoint");

				command.CommandText = "insert into TEST (INT_FIELD) values (201) ";
				command.ExecuteNonQuery();
				Transaction.Save("SecondSavePoint");

				command.CommandText = "insert into TEST (INT_FIELD) values (202) ";
				command.ExecuteNonQuery();
				Transaction.Rollback("InitialSavePoint");

				Transaction.Commit();
			}
		}

		[Test]
		public void AbortTransaction()
		{
			IBTransaction transaction = null;
			IBCommand command = null;

			try
			{
				transaction = Connection.BeginTransaction();

				command = new IBCommand("ALTER TABLE \"TEST\" drop \"INT_FIELD\"", Connection, transaction);
				command.ExecuteNonQuery();

				transaction.Commit();
				transaction = null;
			}
			catch (Exception)
			{
				transaction.Rollback();
				transaction = null;
			}
			finally
			{
				if (command != null)
				{
					command.Dispose();
				}
			}
		}

		#endregion
	}
	public class IBTransactionTestsDialect1 : IBTransactionTests
	{
		public IBTransactionTestsDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1;
		}
	}
}
