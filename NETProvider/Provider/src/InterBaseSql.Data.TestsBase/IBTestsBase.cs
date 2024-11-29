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
using System.Text;
using System.Threading.Tasks;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.Services;
using NUnit.Framework;

namespace InterBaseSql.Data.TestsBase
{
	public abstract class IBTestsBase
	{
		#region	Fields

		protected readonly bool _insertTestData;
		private IBConnection _connection;
		private IBTransaction _transaction;

		#endregion

		#region	Properties

		public IBServerType IBServerType { get; }

		public static int Dialect {
			get { return IBTestsSetup.Dialect; }
			set { IBTestsSetup.Dialect = value; }
		}

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
			IBTestsSetup.Dialect = 3;
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
						if (Dialect == 3)
						    command.Parameters.Add("@bigint_field", IBDbType.BigInt);
					        else
						    command.Parameters.Add("@bigint_field", IBDbType.Numeric);
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
				using (var cmd = connection.CreateCommand())
				{

					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(2, 'Robert', 'Nelson', '250', '12/22/2012 00:00:00', '600', 'VP', 2, 'USA', 105900)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(4, 'Bruce', 'Young', '233', '12/22/2012 00:00:00', '621', 'Eng', 2, 'USA', 97500)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(5, 'Kim', 'Lambert', '22', '01/31/2013 00:00:00', '130', 'Eng', 2, 'USA', 102750)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(8, 'Leslie', 'Johnson', '410', '03/30/2013 00:00:00', '180', 'Mktg', 3, 'USA', 64635)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(9, 'Phil', 'Forest', '2  ', '04/11/2013 00:00:00', '6', 'Mngr', 3, 'USA', 75060)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(11, 'K. J.', 'Weston', '34', '01/11/2014 00:00:00', '130', 'SRep', 4, 'USA', 86292.94)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(12, 'Terri', 'Lee', '256', '04/25/2014 00:00:00', '000', 'Admin', 4, 'USA', 53793)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(14, 'Stewart', 'Hall', '227', '05/29/2014 00:00:00', '900', 'Finan', 3, 'USA', 69482.63)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(15, 'Katherine', 'Young', '231', '06/08/2014 00:00:00', '623', 'Mngr', 3, 'USA', 67241.25)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(20, 'Chris', 'Papadopoulos', '887', '12/26/2013 00:00:00', '671', 'Mngr', 3, 'USA', 89655)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(24, 'Pete', 'Fisher', '888', '09/06/2014 00:00:00', '671', 'Eng', 3, 'USA', 81810.19)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(28, 'Ann', 'Bennet', '5', '01/26/2015 00:00:00', '120', 'Admin', 5, 'England', 22935)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(29, 'Roger', 'De Souza', '288', '02/12/2015 00:00:00', '623', 'Eng', 3, 'USA', 69482.63)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(34, 'Janet', 'Baldwin', '2', '03/15/2015 00:00:00', '110', 'Sales', 3, 'USA', 61637.81)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(36, 'Roger', 'Reeves', '6', '04/19/2015 00:00:00', '120', 'Sales', 3, 'England', 33620.63)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(37, 'Willie', 'Stansbury', '7', '04/19/2015 00:00:00', '120', 'Eng', 4, 'England', 39224.06)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(44, 'Leslie', 'Phong', '216', '05/28/2015 00:00:00', '623', 'Eng', 4, 'USA', 56034.38)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(45, 'Ashok', 'Ramanathan', '209', '07/26/2015 00:00:00', '621', 'Eng', 3, 'USA', 80689.5)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(46, 'Walter', 'Steadman', '210', '08/03/2015 00:00:00', '900', 'CFO', 1, 'USA', 116100)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(52, 'Carol', 'Nordstrom', '420', '09/26/2015 00:00:00', '180', 'PRel', 4, 'USA', 42742.5)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(61, 'Luke', 'Leung', '3', '02/12/2016 00:00:00', '110', 'SRep', 4, 'USA', 68805)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(65, 'Sue Anne', 'O''Brien', '877', '03/17/2016 00:00:00', '670', 'Admin', 5, 'USA', 31275)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(71, 'Jennifer M.', 'Burbank', '289', '04/09/2016 00:00:00', '622', 'Eng', 3, 'USA', 53167.5)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(72, 'Claudia', 'Sutherland', NULL, '04/14/2016 00:00:00', '140', 'SRep', 4, 'Canada', 100914)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(83, 'Dana', 'Bishop', '290', '05/26/2016 00:00:00', '621', 'Eng', 3, 'USA', 62550)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(85, 'Mary S.', 'MacDonald', '477', '05/26/2016 00:00:00', '100', 'VP', 2, 'USA', 111262.5)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(94, 'Randy', 'Williams', '892', '08/02/2016 00:00:00', '672', 'Mngr', 4, 'USA', 56295)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(105, 'Oliver H.', 'Bender', '255', '10/02/2016 00:00:00', '000', 'CEO', 1, 'USA', 212850)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(107, 'Kevin', 'Cook', '894', '01/26/2017 00:00:00', '670', 'Dir', 2, 'USA', 111262.5)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(109, 'Kelly', 'Brown', '202', '01/29/2017 00:00:00', '600', 'Admin', 5, 'USA', 27000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(110, 'Yuki', 'Ichida', '22', '01/29/2017 00:00:00', '115', 'Eng', 3, 'Japan', 6000000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(113, 'Mary', 'Page', '845', '04/06/2017 00:00:00', '671', 'Eng', 4, 'USA', 48000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(114, 'Bill', 'Parker', '247', '05/26/2017 00:00:00', '623', 'Eng', 5, 'USA', 35000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(118, 'Takashi', 'Yamamoto', '23', '06/25/2017 00:00:00', '115', 'SRep', 4, 'Japan', 7480000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(121, 'Roberto', 'Ferrari', '1', '07/06/2017 00:00:00', '125', 'SRep', 4, 'Italy', 51129.1)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(127, 'Michael', 'Yanowski', '492', '08/03/2017 00:00:00', '100', 'SRep', 4, 'USA', 44000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(134, 'Jacques', 'Glon', NULL, '08/17/2017 00:00:00', '123', 'SRep', 4, 'France', 59530.9)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(136, 'Scott', 'Johnson', '265', '09/07/2017 00:00:00', '623', 'Doc', 3, 'USA', 60000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(138, 'T.J.', 'Green', '218', '10/26/2017 00:00:00', '621', 'Eng', 4, 'USA', 36000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(141, 'Pierre', 'Osborne', NULL, '12/28/2017 00:00:00', '121', 'SRep', 4, 'Switzerland', 110000)";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "INSERT INTO EMPLOYEE(EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES(144, 'John', 'Montgomery', '820', '03/24/2018 00:00:00', '672', 'Eng', 5, 'USA', 35000)";
					cmd.ExecuteNonQuery();
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
			builder.Dialect = IBTestsSetup.Dialect;
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
		protected async Task<int> GetActiveConnectionsAsync()
		{
			var csb = BuildConnectionStringBuilder(IBServerType);
			csb.Pooling = false;
			using (var conn = new IBConnection(csb.ToString()))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select count(*) from tmp$attachments";
					return Convert.ToInt32(await cmd.ExecuteScalarAsync());
				}
			}
		}

		protected async Task<Version> GetServerVersion()
		{
			var server = new IBServerProperties();
			server.ConnectionString = BuildServicesConnectionString(IBServerType, false);
			return IBServerProperties.ParseServerVersion(await server.GetServerVersionAsync());
		}

		protected async Task<bool> EnsureVersion(Version version)
		{
			if (await GetServerVersion() >= version)
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
			#if NET6_0_OR_GREATER
			byte[] buffer = RandomNumberGenerator.GetBytes(4);
			#else
			var rng = new RNGCryptoServiceProvider();
			var buffer = new byte[4];
			rng.GetBytes(buffer);
			#endif
			return BitConverter.ToInt32(buffer, 0);
		}

#endregion
	}
}
