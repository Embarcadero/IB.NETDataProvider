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
using System.Data;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class IBCommandBuilderTests : IBTestsBase
	{
		#region Fields

		private IBDataAdapter _adapter;

		#endregion

		#region Constructors

		public IBCommandBuilderTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region SetUp and TearDown methods

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			_adapter = new IBDataAdapter(new IBCommand("select * from TEST where VARCHAR_FIELD = ?", Connection));
		}

		[TearDown]
		public override void TearDown()
		{
			_adapter.Dispose();
			base.TearDown();
		}

		#endregion

		#region Unit Tests

		[Test]
		public void GetInsertCommandTest()
		{
			var builder = new IBCommandBuilder(_adapter);

			StringAssert.StartsWith("INSERT", builder.GetInsertCommand().CommandText);

			builder.Dispose();
		}

		[Test]
		public void GetUpdateCommandTest()
		{
			var builder = new IBCommandBuilder(_adapter);

			StringAssert.StartsWith("UPDATE", builder.GetUpdateCommand().CommandText);

			builder.Dispose();
		}

		[Test]
		public void GetDeleteCommandTest()
		{
			var builder = new IBCommandBuilder(_adapter);

			StringAssert.StartsWith("DELETE", builder.GetDeleteCommand().CommandText);

			builder.Dispose();
		}

		[Test]
		public void RefreshSchemaTest()
		{
			var builder = new IBCommandBuilder(_adapter);

			Assert.DoesNotThrow(() => builder.GetInsertCommand());
			Assert.DoesNotThrow(() => builder.GetUpdateCommand());
			Assert.DoesNotThrow(() => builder.GetDeleteCommand());

			_adapter.SelectCommand.CommandText = "select * from TEST where BIGINT_FIELD = ?";

			builder.RefreshSchema();

			Assert.DoesNotThrow(() => builder.GetInsertCommand());
			Assert.DoesNotThrow(() => builder.GetUpdateCommand());
			Assert.DoesNotThrow(() => builder.GetDeleteCommand());

			builder.Dispose();
		}

		[Test]
		public void CommandBuilderWithExpressionFieldTest()
		{
			_adapter.SelectCommand.CommandText = "select TEST.*, 0 AS VALOR from TEST";

			var builder = new IBCommandBuilder(_adapter);

			StringAssert.DoesNotContain("VALOR", builder.GetUpdateCommand().CommandText);

			builder.Dispose();
		}

		[Test]
		public void DeriveParameters()
		{
			var command = new IBCommand("GETVARCHARFIELD", Connection);

			command.CommandType = CommandType.StoredProcedure;

			IBCommandBuilder.DeriveParameters(command);

			Assert.AreEqual(2, command.Parameters.Count);
		}

		[Test]
		public void DeriveParameters2()
		{
			var transaction = Connection.BeginTransaction();

			var command = new IBCommand("GETVARCHARFIELD", Connection, transaction);

			command.CommandType = CommandType.StoredProcedure;

			IBCommandBuilder.DeriveParameters(command);

			Assert.AreEqual(2, command.Parameters.Count);

			transaction.Commit();
		}

		[Test]
		public void DeriveParametersNonExistingSP()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var transaction = Connection.BeginTransaction();

				var command = new IBCommand("BlaBlaBla", Connection, transaction);

				command.CommandType = CommandType.StoredProcedure;

				IBCommandBuilder.DeriveParameters(command);

				transaction.Commit();
			});
		}

		[Test]
		public void TestWithClosedConnection()
		{
			Connection.Close();

			var builder = new IBCommandBuilder(_adapter);

			Assert.DoesNotThrow(() => builder.GetInsertCommand());
			Assert.DoesNotThrow(() => builder.GetUpdateCommand());
			Assert.DoesNotThrow(() => builder.GetDeleteCommand());

			_adapter.SelectCommand.CommandText = "select * from TEST where BIGINT_FIELD = ?";

			builder.RefreshSchema();

			Assert.DoesNotThrow(() => builder.GetInsertCommand());
			Assert.DoesNotThrow(() => builder.GetUpdateCommand());
			Assert.DoesNotThrow(() => builder.GetDeleteCommand());

			builder.Dispose();
		}

		#endregion
	}

	public class IBCommandBuilderTestsDialect1 : IBCommandBuilderTests
	{
		public IBCommandBuilderTestsDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1; 
		}

	}
}
