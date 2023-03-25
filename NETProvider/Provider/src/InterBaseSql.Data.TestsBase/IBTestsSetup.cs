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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Text;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;
using System.Reflection;
using InterBaseSql.Data.Isql;
using InterBaseSql.Data.Services;

[SetUpFixture]
public class IBTestsSetup
{
	private const string DatabaseBase = "netprovider_tests";
	internal const string UserID = "SYSDBA";
	internal const string Password = "masterkey";
	internal const string DataSource = "localhost";
	internal const int Port = 3050;
	internal const string Charset = "utf8";
	internal const bool Pooling = false;
	internal const int PageSize = 4096;
	internal const bool ForcedWrite = false;

	public static int Dialect {get; set;}

	static HashSet<Tuple<IBServerType, int>> _initalized = new HashSet<Tuple<IBServerType, int>>();

	public static void SetUp(IBServerType serverType)
	{
		var item = Tuple.Create(serverType, Dialect);
		if (!_initalized.Contains(item))
		{
			var cs = IBTestsBase.BuildConnectionString(serverType);
			IBConnection.CreateDatabase(cs, PageSize, ForcedWrite, true);
			CreateDomains(cs);
			CreateTables(cs);
			CreateProcedures(cs);
			CreateTriggers(cs);
			CreateUDF(cs);
			CreateSubscriptions(cs);
			DoGrant(cs);
			_initalized.Add(item);
		}
	}

	private static void DoGrant(string connectionString)
	{
		using (var connection = new IBConnection(connectionString))
		{
			connection.Open();
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "GRANT ALL on Employee to jeff";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "GRANT SUBSCRIBE ON SUBSCRIPTION EMPLOYEE_INSERT TO Jeff";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "GRANT SUBSCRIBE ON SUBSCRIPTION EMPLOYEE_UPDATECOL TO Jeff";
				cmd.ExecuteNonQuery();
			}
		}
	}

	private static void CreateDomains(string connectionString)
	{
		using (var connection = new IBConnection(connectionString))
		{
			connection.Open();
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = "CREATE DOMAIN COUNTRYNAME AS VARCHAR(15)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE DOMAIN DEPTNO AS CHAR(3) CHECK(VALUE = '000' OR(VALUE > '0' AND VALUE <= '999') OR VALUE IS NULL)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE DOMAIN EMPNO AS SMALLINT;";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE DOMAIN FIRSTNAME AS VARCHAR(15)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE DOMAIN JOBCODE AS VARCHAR(5) CHECK(VALUE > '99999')";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE DOMAIN JOBGRADE AS SMALLINT CHECK(VALUE BETWEEN 0 AND 6)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE DOMAIN LASTNAME AS VARCHAR(20)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE DOMAIN SALARY AS NUMERIC(10, 2) DEFAULT 0 CHECK(VALUE > 0)";
				cmd.ExecuteNonQuery();
			}
		}
	}

	public static string Database(IBServerType serverType)
	{
		var path = AppDomain.CurrentDomain.BaseDirectory;
		if (IBTestsSetup.Dialect == 1)
			return $"{path}{DatabaseBase}_{serverType}_D1.ib";
		else
			return $"{path}{DatabaseBase}_{serverType}.ib";
	}

	[OneTimeTearDown]
	public void TearDown()
	{
		IBConnection.ClearAllPools();
		foreach (var item in _initalized)
		{
			var cs = IBTestsBase.BuildConnectionString(item.Item1);
			IBConnection.DropDatabase(cs);
		}
		_initalized.Clear();
	}

	private static void CreateUDF(string connectionString)
	{
		using (var connection = new IBConnection(connectionString))
		{
			connection.Open();
			const string text =
@"/* Use these 'wrappers' to put functionality into
   any of your databases. */

/* Date/Time routines */

declare external function EF_UTCCurrentTime
  returns
  timestamp /* free_it */
  entry_point 'UTCCurrentTime' module_name 'EntityFrameworkUDF';

declare external function EF_DateAdd
  cstring(7),
  numeric(18, 0),
	timestamp
  returns
  timestamp /* free_it */
  entry_point 'DateAdd' module_name 'EntityFrameworkUDF';

declare external function EF_DateDiff
  cstring(12),
  timestamp,
	timestamp
  returns
  numeric(18,0) by value /* free_it */
  entry_point 'DateDiff' module_name 'EntityFrameworkUDF';

/* Mathematical functions */

declare external function EF_Abs
  double precision
  returns
  double precision by value
  entry_point 'fn_Abs' module_name 'EntityFrameworkUDF';

declare external function EF_Ceiling
  double precision
  returns
  numeric(18,0) by value
  entry_point 'fn_Ceiling' module_name 'EntityFrameworkUDF';

declare external function EF_Floor
  double precision
  returns
  numeric(18, 0) by value
  entry_point 'fn_Floor' module_name 'EntityFrameworkUDF';

declare external function EF_Round
  double precision,
	integer
  returns
  double precision by value
  entry_point 'fn_Round' module_name 'EntityFrameworkUDF';

declare external function EF_Power
  double precision,
	double precision
  returns
  double precision by value
  entry_point 'fn_Power' module_name 'EntityFrameworkUDF';
	
declare external function EF_Trunc
  double precision,
	integer
  returns
  double precision by value
  entry_point 'fn_Truncate' module_name 'EntityFrameworkUDF';

declare external function EF_BitAnd
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitAnd' module_name 'EntityFrameworkUDF';

declare external function EF_BitNot
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitNot' module_name 'EntityFrameworkUDF';

declare external function EF_BitOr
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitOr' module_name 'EntityFrameworkUDF';

declare external function EF_BitXor
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitXor' module_name 'EntityFrameworkUDF';

/* String functions */

declare external function EF_Reverse
  cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'Reverse' module_name 'EntityFrameworkUDF';

declare external function EF_Position
  cstring(2048),
	cstring(2048),
	integer
  returns integer by value /* free_it */
  entry_point 'Position' module_name 'EntityFrameworkUDF';

declare external function EF_Length
  cstring(2048)
  returns integer by value /* free_it */
  entry_point 'StringLength' module_name 'EntityFrameworkUDF';

declare external function EF_Lower
  cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'ToLower' module_name 'EntityFrameworkUDF';

declare external function EF_Trim
  cstring(8),
  cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'Trim' module_name 'EntityFrameworkUDF';

declare external function EF_Left
  cstring(2048),
	integer
  returns cstring(2048) /* free_it */
  entry_point 'Left' module_name 'EntityFrameworkUDF';

declare external function EF_Right
  cstring(2048),
	integer
  returns cstring(2048) /* free_it */
  entry_point 'Right' module_name 'EntityFrameworkUDF';

declare external function EF_Replace
  cstring(2048),
	cstring(2048),
	cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'Replace' module_name 'EntityFrameworkUDF';

declare external function EF_SubStr
  cstring(2048),
	integer,
	integer
  returns cstring(2048) /* free_it */
  entry_point 'SubStr' module_name 'EntityFrameworkUDF';

declare external function EF_NewGUID
  returns cstring(16) character set OCTETS /* free_it */
  entry_point 'NewGuid' module_name 'EntityFrameworkUDF';

declare external function EF_UUID_TO_CHAR
  cstring(16) character set OCTETS
  returns cstring(38)  /* free_it */
  entry_point 'UUID_TO_CHAR' module_name 'EntityFrameworkUDF';

declare external function EF_CHAR_TO_UUID
  cstring(36) 
  returns cstring(16) character set OCTETS /* free_it */
  entry_point 'CHAR_TO_UUID' module_name 'EntityFrameworkUDF';	

";
			var script = new IBScript(text);
			script.Parse();
			var transaction = connection.BeginTransaction();

			var command = connection.CreateCommand();

			command.Transaction = transaction;
			foreach (IBStatement element in script.Results)
			{
				command.CommandText = element.Text;
				command.ExecuteNonQuery();
			}
			transaction.Commit();
		}
	}

	private static void CreateTables(string connectionString)
	{
		using (var connection = new IBConnection(connectionString))
		{
			connection.Open();

			var commandText = new StringBuilder();

			commandText.Append("CREATE TABLE TEST (");
			commandText.Append("INT_FIELD		 INTEGER DEFAULT 0 NOT NULL	PRIMARY	KEY,");
			commandText.Append("CHAR_FIELD		 CHAR(30),");
			commandText.Append("VARCHAR_FIELD	 VARCHAR(100),");
			commandText.Append("BIGINT_FIELD	 numeric(18, 0),");
			commandText.Append("SMALLINT_FIELD	 SMALLINT,");
			commandText.Append("DOUBLE_FIELD	 DOUBLE	PRECISION,");
			commandText.Append("FLOAT_FIELD		 FLOAT,");
			commandText.Append("NUMERIC_FIELD	 NUMERIC(15,2),");
			commandText.Append("DECIMAL_FIELD	 DECIMAL(15,2),");
			if (Dialect == 3)
			{
				commandText.Append("DATE_FIELD		 DATE,");
				commandText.Append("TIME_FIELD		 TIME,");
			}
			else
			{
				commandText.Append("DATE_FIELD		 TIMESTAMP,");
				commandText.Append("TIME_FIELD		 TIMESTAMP,");
			}
			commandText.Append("TIMESTAMP_FIELD	 TIMESTAMP,");
			commandText.Append("CLOB_FIELD		 BLOB SUB_TYPE 1 SEGMENT SIZE 80,");
			commandText.Append("BLOB_FIELD		 BLOB SUB_TYPE 0 SEGMENT SIZE 80,");
			commandText.Append("IARRAY_FIELD	 INTEGER [1:4],");
			commandText.Append("SARRAY_FIELD	 SMALLINT [1:5],");
			commandText.Append("LARRAY_FIELD	 numeric(18, 0)	[1:6],");
			commandText.Append("FARRAY_FIELD	 FLOAT [1:4],");
			commandText.Append("BARRAY_FIELD	 DOUBLE	PRECISION [1:4],");
			commandText.Append("NARRAY_FIELD	 NUMERIC(10,6) [1:4],");
			if (Dialect == 3)
			{
				commandText.Append("DARRAY_FIELD	 DATE [1:4],");
				commandText.Append("TARRAY_FIELD	 TIME [1:4],");
			}
			else
			{
				commandText.Append("DARRAY_FIELD	 TIMESTAMP [1:4],");
				commandText.Append("TARRAY_FIELD	 TIMESTAMP [1:4],");
			}
			commandText.Append("TSARRAY_FIELD	 TIMESTAMP [1:4],");
			commandText.Append("CARRAY_FIELD	 CHAR(21) [1:4],");
			commandText.Append("VARRAY_FIELD	 VARCHAR(30) [1:4],");
			commandText.Append("BIG_ARRAY		 INTEGER [1:32767],");
			commandText.Append("EXPR_FIELD		 COMPUTED BY (smallint_field * 1000),");
			commandText.Append("CS_FIELD		 CHAR(1) CHARACTER SET UNICODE_FSS,");
			commandText.Append("UCCHAR_ARRAY	 CHAR(10) [1:10] CHARACTER SET UNICODE_FSS);");

			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			using (var command = new IBCommand("create table PrepareTest(test_field varchar(20));", connection))
			{
				command.ExecuteNonQuery();
			}

			using (var command = new IBCommand("create table log(occured timestamp, text varchar(20));", connection))
			{
				command.ExecuteNonQuery();
			}

			using (var command = new IBCommand("CREATE TABLE GUID_TEST (INT_FIELD INTEGER, GUID_FIELD CHAR(16) CHARACTER SET OCTETS)", connection))
			{
				command.ExecuteNonQuery();
			}

			using (var command = new IBCommand(@"CREATE TABLE EMPLOYEE
(		EMP_NO  EMPNO NOT NULL,
		FIRST_NAME  FIRSTNAME,
		LAST_NAME   LASTNAME,
		PHONE_EXT   VARCHAR(4),
		HIRE_DATE   TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
		DEPT_NO DEPTNO NOT NULL,
		JOB_CODE    JOBCODE NOT NULL,
		JOB_GRADE   JOBGRADE NOT NULL,
		JOB_COUNTRY COUNTRYNAME NOT NULL,
		SALARY  SALARY NOT NULL,
 PRIMARY KEY(EMP_NO)
)", connection))
			{
				command.ExecuteNonQuery();
			}
		}
	}

	private static void CreateSubscriptions(string connectionString)
	{
		using (var connection = new IBConnection(connectionString))
		{
			connection.Open();
			using (var cmd = connection.CreateCommand())
			{
				cmd.CommandText = @"CREATE SUBSCRIPTION EMPLOYEE_ALL ON
    EMPLOYEE for row (INSERT, UPDATE, DELETE)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"CREATE SUBSCRIPTION EMPLOYEE_INSERTALLUPDATECOL ON
    EMPLOYEE FOR ROW (INSERT),
    EMPLOYEE (FIRST_NAME, LAST_NAME)  FOR ROW (UPDATE)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"CREATE SUBSCRIPTION EMPLOYEE_DELETE ON
    EMPLOYEE FOR ROW (DELETE)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"CREATE SUBSCRIPTION EMPLOYEE_INSERT ON
    EMPLOYEE FOR ROW (INSERT)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"CREATE SUBSCRIPTION EMPLOYEE_INSERTUPDATE ON
    EMPLOYEE FOR ROW (INSERT, UPDATE)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"CREATE SUBSCRIPTION EMPLOYEE_UPDATE ON
    EMPLOYEE FOR ROW (UPDATE)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"CREATE SUBSCRIPTION EMPLOYEE_UPDATECOL ON
    EMPLOYEE (FIRST_NAME, LAST_NAME)  FOR ROW (UPDATE)";
				cmd.ExecuteNonQuery();
			}
		}
	}


	private static void CreateProcedures(string connectionString)
	{
		using (var connection = new IBConnection(connectionString))
		{
			connection.Open();

			var commandText = new StringBuilder();

			commandText.Clear();
			commandText.Append("CREATE PROCEDURE SELECT_DATA  \r\n");
			commandText.Append("RETURNS	( \r\n");
			commandText.Append("INT_FIELD INTEGER, \r\n");
			commandText.Append("VARCHAR_FIELD VARCHAR(100),	\r\n");
			commandText.Append("DECIMAL_FIELD DECIMAL(15,2)) \r\n");
			commandText.Append("AS \r\n");
			commandText.Append("begin \r\n");
			commandText.Append("FOR	SELECT INT_FIELD, VARCHAR_FIELD, DECIMAL_FIELD FROM	TEST INTO :INT_FIELD, :VARCHAR_FIELD, :DECIMAL_FIELD \r\n");
			commandText.Append("DO \r\n");
			commandText.Append("SUSPEND; \r\n");
			commandText.Append("end;");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("CREATE PROCEDURE GETRECORDCOUNT	\r\n");
			commandText.Append("RETURNS	( \r\n");
			commandText.Append("RECCOUNT SMALLINT) \r\n");
			commandText.Append("AS \r\n");
			commandText.Append("begin \r\n");
			commandText.Append("for	select count(*)	from test into :reccount \r\n");
			commandText.Append("do \r\n");
			commandText.Append("suspend; \r\n");
			commandText.Append("end\r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("CREATE PROCEDURE GETVARCHARFIELD (\r\n");
			commandText.Append("ID INTEGER)\r\n");
			commandText.Append("RETURNS	(\r\n");
			commandText.Append("VARCHAR_FIELD VARCHAR(100))\r\n");
			commandText.Append("AS\r\n");
			commandText.Append("begin\r\n");
			commandText.Append("for	select varchar_field from test where int_field = :id into :varchar_field\r\n");
			commandText.Append("do\r\n");
			commandText.Append("suspend;\r\n");
			commandText.Append("end\r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("CREATE PROCEDURE GETASCIIBLOB (\r\n");
			commandText.Append("ID INTEGER)\r\n");
			commandText.Append("RETURNS	(\r\n");
			commandText.Append("ASCII_BLOB BLOB	SUB_TYPE 1)\r\n");
			commandText.Append("AS\r\n");
			commandText.Append("begin\r\n");
			commandText.Append("for	select clob_field from test	where int_field	= :id into :ascii_blob\r\n");
			commandText.Append("do\r\n");
			commandText.Append("suspend;\r\n");
			commandText.Append("end\r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("CREATE PROCEDURE DATAREADERTEST\r\n");
			commandText.Append("RETURNS	(\r\n");
			commandText.Append("content	VARCHAR(128))\r\n");
			commandText.Append("AS\r\n");
			commandText.Append("begin\r\n");
			commandText.Append("content	= 'test';\r\n");
			commandText.Append("suspend;\r\n");
			commandText.Append("end\r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("create procedure SimpleSP\r\n");
			commandText.Append("returns ( result integer ) as\r\n");
			commandText.Append("begin\r\n");
			commandText.Append("result = 1000;\r\n");
			commandText.Append("end \r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("create procedure Octets (inp VarChar(10) character set octets)\r\n");
			commandText.Append("returns ( oct Varchar(10) character set octets) as\r\n");
			commandText.Append("begin\r\n");
			commandText.Append("  oct = inp;\r\n");
			commandText.Append("  suspend;\r\n");
			commandText.Append("end \r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("create PROCEDURE DELETEALLDATA\r\n");
			commandText.Append("AS\r\n");
			commandText.Append("declare variable RELNAME VARCHAR(68);\r\n");
			commandText.Append("begin\r\n");
			commandText.Append("	for select rdb$relation_name\r\n");
			commandText.Append("		  from rdb$relations\r\n");
			commandText.Append("		 where coalesce(rdb$system_flag, 0) = 0\r\n");
			commandText.Append("		  into :relname do\r\n");
			commandText.Append("	begin\r\n");
			commandText.Append("		execute statement 'delete from ' || :relname;\r\n");
			commandText.Append("	end\r\n");
			commandText.Append("end\r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("create  PROCEDURE EVENTTEST\r\n");
			commandText.Append("(\r\n");
			commandText.Append("  EVENT VARCHAR(40)\r\n");
			commandText.Append(")\r\n");
			commandText.Append("AS\r\n");
			commandText.Append("declare variable RELNAME VARCHAR(68);\r\n");
			commandText.Append("begin\r\n");
			commandText.Append("  post_event :event;\r\n");
			commandText.Append("end\r\n");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			using (var command = new IBCommand("", connection))
			{
				if (Dialect == 3)
				{
					command.CommandText = @"
CREATE PROCEDURE TEST_SP (
  P01 SMALLINT,  P02 INTEGER,  P03 INTEGER,  P04 FLOAT,  P05 INTEGER,  P06 INTEGER,  P07 DATE,  P08 DATE )
RETURNS (  R01 FLOAT,  R02 FLOAT,  R03 FLOAT,  R04 FLOAT,  R05 FLOAT,  R06 FLOAT,  R07 FLOAT,  R08 FLOAT,  R09 FLOAT,  R10 FLOAT,
  R11 FLOAT,  R12 FLOAT,  R13 FLOAT,  R14 FLOAT,  R15 FLOAT,  R16 FLOAT,  R17 FLOAT,  R18 FLOAT,  R19 FLOAT,  R20 FLOAT,  R21 FLOAT,
  R22 FLOAT,  R23 FLOAT,  R24 FLOAT,  R25 FLOAT,  R26 FLOAT,  R27 FLOAT,  R28 FLOAT,  R29 FLOAT,  R30 FLOAT,  R31 FLOAT,  R32 FLOAT,
  R33 FLOAT,  R34 FLOAT,  R35 FLOAT,  R36 FLOAT,  R37 FLOAT,  R38 FLOAT,  R39 FLOAT,  R40 FLOAT,  R41 FLOAT,  R42 FLOAT,  R43 FLOAT,
  R44 FLOAT,  R45 FLOAT,  R46 FLOAT,  R47 FLOAT,  R48 FLOAT,  R49 FLOAT,  R50 FLOAT,  R51 FLOAT,  R52 FLOAT,  R53 FLOAT,  R54 FLOAT,
  R55 FLOAT,  R56 FLOAT,  R57 FLOAT,  R58 FLOAT,  R59 FLOAT,  R60 FLOAT,  R61 FLOAT,  R62 FLOAT,  R63 FLOAT,  R64 FLOAT,  R65 FLOAT,
  R66 FLOAT,  R67 FLOAT,  R68 FLOAT,  R69 FLOAT,  R70 FLOAT,  R71 FLOAT,  R72 FLOAT,  R73 FLOAT,  R74 FLOAT,  R75 FLOAT,  R76 FLOAT,
  R77 FLOAT,  R78 FLOAT,  R79 FLOAT,  R80 FLOAT,  R81 FLOAT,  R82 FLOAT,  R83 FLOAT,  R84 FLOAT,  R85 FLOAT,  R86 FLOAT,  R87 FLOAT,
  R88 FLOAT,  R89 FLOAT,  R90 FLOAT,  R91 FLOAT,  R92 FLOAT,  R93 FLOAT,  R94 FLOAT,  R95 FLOAT ) AS
BEGIN
  SUSPEND;
END
";
				}
				else
				{
					command.CommandText = @"
CREATE PROCEDURE TEST_SP (
  P01 SMALLINT,  P02 INTEGER,  P03 INTEGER,  P04 FLOAT,  P05 INTEGER,  P06 INTEGER,  P07 TIMESTAMP,  P08 TIMESTAMP )
RETURNS (  R01 FLOAT,  R02 FLOAT,  R03 FLOAT,  R04 FLOAT,  R05 FLOAT,  R06 FLOAT,  R07 FLOAT,  R08 FLOAT,  R09 FLOAT,  R10 FLOAT,
  R11 FLOAT,  R12 FLOAT,  R13 FLOAT,  R14 FLOAT,  R15 FLOAT,  R16 FLOAT,  R17 FLOAT,  R18 FLOAT,  R19 FLOAT,  R20 FLOAT,  R21 FLOAT,
  R22 FLOAT,  R23 FLOAT,  R24 FLOAT,  R25 FLOAT,  R26 FLOAT,  R27 FLOAT,  R28 FLOAT,  R29 FLOAT,  R30 FLOAT,  R31 FLOAT,  R32 FLOAT,
  R33 FLOAT,  R34 FLOAT,  R35 FLOAT,  R36 FLOAT,  R37 FLOAT,  R38 FLOAT,  R39 FLOAT,  R40 FLOAT,  R41 FLOAT,  R42 FLOAT,  R43 FLOAT,
  R44 FLOAT,  R45 FLOAT,  R46 FLOAT,  R47 FLOAT,  R48 FLOAT,  R49 FLOAT,  R50 FLOAT,  R51 FLOAT,  R52 FLOAT,  R53 FLOAT,  R54 FLOAT,
  R55 FLOAT,  R56 FLOAT,  R57 FLOAT,  R58 FLOAT,  R59 FLOAT,  R60 FLOAT,  R61 FLOAT,  R62 FLOAT,  R63 FLOAT,  R64 FLOAT,  R65 FLOAT,
  R66 FLOAT,  R67 FLOAT,  R68 FLOAT,  R69 FLOAT,  R70 FLOAT,  R71 FLOAT,  R72 FLOAT,  R73 FLOAT,  R74 FLOAT,  R75 FLOAT,  R76 FLOAT,
  R77 FLOAT,  R78 FLOAT,  R79 FLOAT,  R80 FLOAT,  R81 FLOAT,  R82 FLOAT,  R83 FLOAT,  R84 FLOAT,  R85 FLOAT,  R86 FLOAT,  R87 FLOAT,
  R88 FLOAT,  R89 FLOAT,  R90 FLOAT,  R91 FLOAT,  R92 FLOAT,  R93 FLOAT,  R94 FLOAT,  R95 FLOAT ) AS
BEGIN
  SUSPEND;
END
";
				}
				command.ExecuteNonQuery();
			}
		}
	}

	private static void CreateTriggers(string connectionString)
	{
		using (var connection = new IBConnection(connectionString))
		{
			connection.Open();

			var commandText = new StringBuilder();

			commandText.Clear();
			commandText.Append("CREATE TRIGGER new_row FOR test	\r\n");
			commandText.Append("AFTER INSERT POSITION 0\r\n");
			commandText.Append("AS\r\n");
			commandText.Append("BEGIN\r\n");
			commandText.Append("POST_EVENT 'new	row';\r\n");
			commandText.Append("END");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}

			commandText.Clear();
			commandText.Append("CREATE TRIGGER update_row FOR test \r\n");
			commandText.Append("AFTER UPDATE POSITION 0\r\n");
			commandText.Append("AS\r\n");
			commandText.Append("BEGIN\r\n");
			commandText.Append("POST_EVENT 'updated	row';\r\n");
			commandText.Append("END");
			using (var command = new IBCommand(commandText.ToString(), connection))
			{
				command.ExecuteNonQuery();
			}
		}
	}
}
