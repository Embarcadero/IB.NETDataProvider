using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InterBaseSql.Data.InterBaseClient;
using System.IO;
using InterBaseSql.Data.Common;

namespace ChangeViews_Demo
{
	public partial class MainForm : Form
	{
		private IBConnection _rwConnection;
		private IBConnection _subConnection;
		private IBTransaction _snaptransaction;
		private IBDataAdapter dataAdapter;

		public MainForm()
		{
			InitializeComponent();
			CreateAndConnectDatabase();
			SetupDataTables();
		}

		#region Setup
		private static IBConnectionStringBuilder BuildConnectionStringBuilder()
		{
			var builder = new IBConnectionStringBuilder();
			builder.UserID = "SYSDBA";
			builder.Password = "masterkey";
			builder.DataSource = "localhost";
			builder.Database = AppDomain.CurrentDomain.BaseDirectory + "ChangeViewDemo.ib";
			builder.Port = 3050;
			return builder;
		}

		private void CreateAndConnectDatabase()
		{
			var cs = BuildConnectionStringBuilder().ToString();
			if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "ChangeViewDemo.ib"))
			{
				IBConnection.CreateDatabase(cs, 8192, true, true);
				CreateConnections();
				CreateDomains();
				CreateTables();
				CreateSubscriptions();
				InsertData();
			}
			else
			{
				CreateConnections();
			}

			void CreateConnections()

			{
				_rwConnection = new IBConnection(cs);
				_rwConnection.Open();
				_subConnection = new IBConnection(cs);
				_subConnection.Open();
			}
		}
		private void CreateDomains()
		{
			using (var cmd = _rwConnection.CreateCommand())
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

		private void CreateTables()
		{

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
)", _rwConnection))
			{
				command.ExecuteNonQuery();
			}
		}

		private void CreateSubscriptions()
		{
			using (var cmd = _rwConnection.CreateCommand())
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

		private void InsertData()
		{
			using (var cmd = _rwConnection.CreateCommand())
			{
				if (_snaptransaction != null)
				{
					_snaptransaction.Rollback();
					_snaptransaction = null;
				}
				cmd.CommandText = "delete from employee";
				cmd.ExecuteNonQuery();
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

				PrimeThePump();
				DoInsertUpdateDelete();
			}
		}

		#endregion

		private void SetupDataTables()
		{
			// Primary Data
			try
			{  // Create a new data adapter based on the specified query.
				dataAdapter = new IBDataAdapter("select * from employee", _rwConnection);

				// Create a command builder to generate SQL update, insert, and
				// delete commands based on selectCommand.
				IBCommandBuilder commandBuilder = new IBCommandBuilder(dataAdapter);

				// Populate a new data table and bind it to the BindingSource.
				DataTable table = new DataTable();
				dataAdapter.Fill(table);
				bsReadWrite.DataSource = table;
				dataGridView1.DataSource = bsReadWrite;

				// Resize the DataGridView columns to fit the newly loaded content.
				dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void ActivateSubscription(string Name, string Dest)
		{
			using (var cmd = _subConnection.CreateCommand())
			{
				if (_snaptransaction != null)
					_snaptransaction.Rollback();
				_snaptransaction = _subConnection.BeginTransaction(IsolationLevel.Snapshot);
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "SET SUBSCRIPTION " + Name + " AT '" + Dest + "' ACTIVE";
				cmd.ExecuteNonQuery();
			}
		}

		private void DoInsertUpdateDelete()
		{
			using (var cmd = _rwConnection.CreateCommand())
			{
				cmd.CommandText = "delete from employee where emp_no = 65";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "INSERT INTO EMPLOYEE (EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES (145, 'Mark', 'Guckenheimer', '221', '04/26/2018 00:00:00', '622', 'Eng', 5, 'USA', 32000)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "update employee set last_name = 'Nel' where emp_no = 2";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "update employee set phone_ext = '234' where emp_no = 4";
				cmd.ExecuteNonQuery();
				// Deletes sometimes do not show if everything is done too fast
				System.Threading.Thread.Sleep(10);
			}
		}

		private void ReadEmployeeCommit()
		{
			using (var cmd = _subConnection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "Select * from employee";
				cmd.ExecuteReader();
				_snaptransaction.Commit();
				_snaptransaction = null;
			}
		}

		private void PrimeThePump()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			ReadEmployeeCommit();
			ActivateSubscription("EMPLOYEE_DELETE", "TESTBED");
			ReadEmployeeCommit();
			ActivateSubscription("EMPLOYEE_INSERT", "TESTBED");
			ReadEmployeeCommit();
			ActivateSubscription("EMPLOYEE_INSERTUPDATE", "TESTBED");
			ReadEmployeeCommit();
			ActivateSubscription("EMPLOYEE_UPDATE", "TESTBED");
			ReadEmployeeCommit();
			ActivateSubscription("EMPLOYEE_UPDATECOL", "TESTBED");
			ReadEmployeeCommit();
			ActivateSubscription("EMPLOYEE_UPDATECOL", "TESTBED");
			ReadEmployeeCommit();
			ActivateSubscription("EMPLOYEE_INSERTALLUPDATECOL", "TESTBED");
			ReadEmployeeCommit();
		}

		private void btnActivate_Click(object sender, EventArgs e)
		{
			ActivateSubscription(comboBox1.SelectedItem.ToString(), "TESTBED");
			using (var cmd = _subConnection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				IBDataTable table = new IBDataTable();
				dataAdapter.FillWithChangeState(table);

				bsSubscription.DataSource = table;
				dataGridView2.DataSource = bsSubscription;
				dataGridView2.AutoGenerateColumns = true;

				// Resize the DataGridView columns to fit the newly loaded content.
				if (chkChangeColumns.Checked)
					chkChangeColumns_CheckedChanged(chkChangeColumns, null);
				else
    				dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
			}
		}

        private void btnCommit_Click(object sender, EventArgs e)
        {
			if (_snaptransaction != null)
            {
				_snaptransaction.Commit();
				_snaptransaction = null;
				bsSubscription.DataSource = null;
            }
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
			if (_snaptransaction != null)
			{
				_snaptransaction.Rollback();
				_snaptransaction = null;
				bsSubscription.DataSource = null;
			}
		}

        private void btnEndChanges_Click(object sender, EventArgs e)
        {
			bsReadWrite.EndEdit();
			dataAdapter.Update((DataTable) bsReadWrite.DataSource);
        }

		private void chkChangeColumns_CheckedChanged(object sender, EventArgs e)
		{
			dataGridView2.DataSource = null;
			((IBDataTable)bsSubscription.DataSource).ChangeStateColumns = chkChangeColumns.Checked;
			dataGridView2.DataSource = bsSubscription;
			dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
		}

		private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			var table = (IBDataTable) bsSubscription.DataSource;
			lblValue.Text = "Cell Value = " + table[e.RowIndex][e.ColumnIndex];
			lblState.Text = "Change State = " + table[e.RowIndex].ChangeState(e.ColumnIndex).ToString();
		}

		private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			var table = (IBDataTable)bsSubscription.DataSource;
			switch (table[e.RowIndex].ChangeState(e.ColumnIndex))
			{
				case IBChangeState.csUpdate :
					dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.Orange, BackColor = Color.White };
					break;
				case IBChangeState.csDelete:
					dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.Red, BackColor = Color.White };
					break;
				case IBChangeState.csInsert:
					dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.Green, BackColor = Color.White };
					break;
				default:
					dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = dataGridView2.DefaultCellStyle;
					break;
			}
		}

        private void btnRecreate_Click(object sender, EventArgs e)
        {
			bsSubscription.DataSource = null;
			InsertData();
			SetupDataTables();
		}
    }
}
