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
using System.Security.Cryptography;
using System.Text;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.Services;
using NUnit.Framework;

namespace InterBaseSql.Data.TestsBase
{
	public abstract class IBTestsBase
	{
		#region	Fields

		private readonly bool _insertTestData;
		private IBConnection _connection;
		private IBTransaction _transaction;

		#endregion

		#region	Properties

		public IBServerType IBServerType { get; }

		public IBConnection Connection
		{
			get { return _connection; }
		}

		public IBTransaction Transaction
		{
			get { return _transaction; }
			set { _transaction = value; }
		}

		#endregion

		#region	Constructors

		public IBTestsBase(IBServerType serverType, bool insertTestData = true)
		{
			IBServerType = serverType;
			_insertTestData = insertTestData;
		}

		#endregion

		#region	SetUp and TearDown Methods

		[SetUp]
		public virtual void SetUp()
		{
			IBTestsSetup.SetUp(IBServerType);

			var cs = BuildConnectionString(IBServerType);
			if (_insertTestData)
			{
				InsertTestData(cs);
			}
			_connection = new IBConnection(cs);
			_connection.Open();
		}

		[TearDown]
		public virtual void TearDown()
		{
			var cs = BuildConnectionString(IBServerType);
			_connection.Dispose();
			if (_insertTestData)
			{
				DeleteAllData(cs);
			}
			IBConnection.ClearAllPools();
		}

		#endregion

		#region	Database Creation Methods

		private static void InsertTestData(string connectionString)
		{
			using (var connection = new IBConnection(connectionString))
			{
				connection.Open();

				var commandText = @"
insert into test (int_field, char_field, varchar_field, bigint_field, smallint_field, float_field, double_field, numeric_field, date_field, time_field, timestamp_field, clob_field, blob_field)
values(@int_field, @char_field, @varchar_field, @bigint_field, @smallint_field, @float_field, @double_field, @numeric_field, @date_field, @time_field, @timestamp_field, @clob_field, @blob_field)";

				using (var transaction = connection.BeginTransaction())
				{
					using (var command = new IBCommand(commandText, connection, transaction))
					{
						command.Parameters.Add("@int_field", IBDbType.Integer);
						command.Parameters.Add("@char_field", IBDbType.Char);
						command.Parameters.Add("@varchar_field", IBDbType.VarChar);
						command.Parameters.Add("@bigint_field", IBDbType.BigInt);
						command.Parameters.Add("@smallint_field", IBDbType.SmallInt);
						command.Parameters.Add("@float_field", IBDbType.Double);
						command.Parameters.Add("@double_field", IBDbType.Double);
						command.Parameters.Add("@numeric_field", IBDbType.Numeric);
						command.Parameters.Add("@date_field", IBDbType.Date);
						command.Parameters.Add("@time_Field", IBDbType.Time);
						command.Parameters.Add("@timestamp_field", IBDbType.TimeStamp);
						command.Parameters.Add("@clob_field", IBDbType.Text);
						command.Parameters.Add("@blob_field", IBDbType.Binary);

						command.Prepare();

						for (var i = 0; i < 100; i++)
						{
							command.Parameters["@int_field"].Value = i;
							command.Parameters["@char_field"].Value = "IRow " + i.ToString();
							command.Parameters["@varchar_field"].Value = "IRow Number " + i.ToString();
							command.Parameters["@bigint_field"].Value = i;
							command.Parameters["@smallint_field"].Value = i;
							command.Parameters["@float_field"].Value = (float)(i + 10) / 5;
							command.Parameters["@double_field"].Value = (double)(i + 10) / 5;
							command.Parameters["@numeric_field"].Value = (decimal)(i + 10) / 5;
							command.Parameters["@date_field"].Value = DateTime.Now;
							command.Parameters["@time_field"].Value = DateTime.Now;
							command.Parameters["@timestamp_field"].Value = DateTime.Now;
							command.Parameters["@clob_field"].Value = "IRow Number " + i.ToString();
							command.Parameters["@blob_field"].Value = Encoding.UTF8.GetBytes("IRow Number " + i.ToString());

							command.ExecuteNonQuery();
						}

						transaction.Commit();
					}
				}
			}
		}

		private static void DeleteAllData(string connectionString)
		{
			using (var connection = new IBConnection(connectionString))
			{
				connection.Open();
				using (var transaction = connection.BeginTransaction())
				{
					var commandText = @"execute procedure DeleteAllData";
					using (var command = new IBCommand(commandText, connection, transaction))
					{
						command.ExecuteNonQuery();
					}
					transaction.Commit();
				}
			}
		}

		#endregion

		#region	ConnectionString Building methods

		public static string BuildConnectionString(IBServerType serverType)
		{
			return BuildConnectionStringBuilder(serverType).ToString();
		}

		public static string BuildServicesConnectionString(IBServerType serverType, bool includeDatabase)
		{
			return BuildServicesConnectionStringBuilder(serverType, includeDatabase).ToString();
		}

		public static IBConnectionStringBuilder BuildServicesConnectionStringBuilder(IBServerType serverType, bool includeDatabase)
		{
			var builder = new IBConnectionStringBuilder();
			builder.UserID = IBTestsSetup.UserID;
			builder.Password = IBTestsSetup.Password;
			builder.DataSource = IBTestsSetup.DataSource;
			if (includeDatabase)
			{
				builder.Database = IBTestsSetup.Database(serverType);
			}
			builder.ServerType = serverType;
			return builder;
		}

		public static IBConnectionStringBuilder BuildConnectionStringBuilder(IBServerType serverType)
		{
			var builder = new IBConnectionStringBuilder();
			builder.UserID = IBTestsSetup.UserID;
			builder.Password = IBTestsSetup.Password;
			builder.DataSource = IBTestsSetup.DataSource;
			builder.Database = IBTestsSetup.Database(serverType);
			builder.Port = IBTestsSetup.Port;
			builder.Charset = IBTestsSetup.Charset;
			builder.Pooling = IBTestsSetup.Pooling;
			builder.ServerType = serverType;
			return builder;
		}

		#endregion

		#region	Methods

		protected int GetActiveConnections()
		{
			var csb = BuildConnectionStringBuilder(IBServerType);
			csb.Pooling = false;
			using (var conn = new IBConnection(csb.ToString()))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select count(*) from tmp$attachments";
					return Convert.ToInt32(cmd.ExecuteScalar());
				}
			}
		}

		protected Version GetServerVersion()
		{
			var server = new IBServerProperties();
			server.ConnectionString = BuildServicesConnectionString(IBServerType, false);
			return IBServerProperties.ParseServerVersion(server.GetServerVersion());
		}

		protected bool EnsureVersion(Version version)
		{
			if (GetServerVersion() >= version)
				return true;
			Assert.Inconclusive("Not supported on this version.");
			return false;
		}

		protected bool EnsureServerType(IBServerType type)
		{
			if (IBServerType == type)
				return true;
			Assert.Inconclusive("Not supported on this server type.");
			return false;
		}

		protected static int GetId()
		{
			var rng = new RNGCryptoServiceProvider();
			var buffer = new byte[4];
			rng.GetBytes(buffer);
			return BitConverter.ToInt32(buffer, 0);
		}

		#endregion
	}
}
