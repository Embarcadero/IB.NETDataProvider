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

//$Authors = Jeff Overcash

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class ChangeViewTests : IBTestsBase
	{
		#region	Fields

		private IBTransaction _snaptransaction;

		#endregion

		#region Constructors

		public ChangeViewTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		public override void SetUp()
		{
			base.SetUp();
			PrimeThePump();
			DoInsertUpdateDelete();
		}

		#region Helpers

		private void ActivateSubscription(string Name, string Dest)
		{
			using (var cmd = Connection.CreateCommand())
			{
				_snaptransaction = Connection.BeginTransaction(IsolationLevel.Snapshot);
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "SET SUBSCRIPTION " + Name + " AT '" + Dest + "' ACTIVE";
				cmd.ExecuteNonQuery();
			}
		}

		private void DoInsertUpdateDelete()
		{
			using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = "delete from employee where emp_no = 65";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "INSERT INTO EMPLOYEE (EMP_NO, FIRST_NAME, LAST_NAME, PHONE_EXT, HIRE_DATE, DEPT_NO, JOB_CODE, JOB_GRADE, JOB_COUNTRY, SALARY) VALUES (145, 'Mark', 'Guckenheimer', '221', '04/26/2018 00:00:00', '622', 'Eng', 5, 'USA', 32000)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "update employee set last_name = 'Nel' where emp_no = 2";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "update employee set phone_ext = '234' where emp_no = 4";
				cmd.ExecuteNonQuery();
				System.Threading.Thread.Sleep(10);
			}
		}

		private void ReadEmployeeCommit()
		{
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "Select * from employee";
				cmd.ExecuteReader();
				_snaptransaction.Commit();
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
		#endregion

		#region Tests

		[Test]
		public void ChangeAll()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee";
				var reader = cmd.ExecuteReader();

				int i;
				int j = 0;
				while (reader.Read())
				{
					j++;
					switch (reader.GetInt32(0))
					{
						case 2:
							i = reader.GetOrdinal("last_name");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "Emp_no 2 not csUpdate, received " + reader.GetChangeState(i).ToString());
							Assert.AreEqual("Nel", reader.GetString(i), "Emp_no 2 not 'Nel', received " + reader.GetString(i));
							break;
						case 4:
							i = reader.GetOrdinal("phone_ext");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "Emp_no 4 not csUpdate, received " + reader.GetChangeState(i).ToString());
							break;
						case 65:
							i = reader.GetOrdinal("Emp_no");
							Assert.AreEqual(IBChangeState.csDelete, reader.GetChangeState(i), "Emp_no 65 not csDelete, received " + reader.GetChangeState(i).ToString());
							break;
						case 145:
							i = reader.GetOrdinal("Emp_no");
							Assert.AreEqual(IBChangeState.csInsert, reader.GetChangeState(i), "Emp_no 145 not csInsert, received " + reader.GetChangeState(i).ToString());
							break;
						default:
							Assert.Fail("Unexpected Emp_no " + reader.GetInt32(0));
							break;
					}
				}
				Assert.AreEqual(4, j, "Wrong count, expected 4 records found " + j.ToString());
			}
		}

		[Test]
		public void ChangeAllStringAccessor()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee";
				var reader = cmd.ExecuteReader();

				int j = 0;
				while (reader.Read())
				{
					j++;
					switch (reader.GetInt32(0))
					{
						case 2:
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState("last_name"), "Emp_no 2 not csUpdate, received " + reader.GetChangeState("last_name").ToString());
							Assert.AreEqual("Nel", reader["last_name"], "Emp_no 2 not 'Nel', received " + reader["last_name"]);
							break;
						case 4:
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState("phone_ext"), "Emp_no 4 not csUpdate, received " + reader.GetChangeState("phone_ext").ToString());
							break;
						case 65:
							Assert.AreEqual(IBChangeState.csDelete, reader.GetChangeState("Emp_no"), "Emp_no 65 not csDelete, received " + reader.GetChangeState("Emp_no").ToString());
							break;
						case 145:
							Assert.AreEqual(IBChangeState.csInsert, reader.GetChangeState("Emp_no"), "Emp_no 145 not csInsert, received " + reader.GetChangeState("Emp_no").ToString());
							break;
						default:
							Assert.Fail("Unexpected Emp_no " + reader.GetInt32(0));
							break;
					}
				}
				Assert.AreEqual(4, j, "Wrong count, expected 4 records found " + j.ToString());
			}
		}

		[Test]
		public void ChangeDelete()
		{
			ActivateSubscription("EMPLOYEE_DELETE", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";
				var reader = cmd.ExecuteReader();

				int j = 0;
				while (reader.Read())
				{
					j++;
					Assert.AreEqual(IBChangeState.csDelete, reader.GetChangeState(0), "Emp_no 65 not csDelete, received " + reader.GetChangeState(0).ToString());
				}
				Assert.AreEqual(1, j, "Record Count wrong, expected 1 found " + j.ToString());
			}
		}

		[Test]
		public void ChangeInsert()
		{
			ActivateSubscription("EMPLOYEE_INSERT", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";
				var reader = cmd.ExecuteReader();

				int j = 0;
				while (reader.Read())
				{
					j++;
					Assert.AreEqual(IBChangeState.csInsert, reader.GetChangeState(0), "Emp_no 145 not csDelete, received " + reader.GetChangeState(0).ToString());
					Assert.AreEqual(145, reader.GetInt32(0), "Emp_no wrong expected 145, received " + reader.GetInt32(0).ToString());
				}
				Assert.AreEqual(1, j, "Record Count wrong, expected 1 found " + j.ToString());
			}
		}

		[Test]
		public void ChangeUpdate()
		{
			ActivateSubscription("EMPLOYEE_UPDATE", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";
				var reader = cmd.ExecuteReader();

				int i;
				int j = 0;
				while (reader.Read())
				{
					j++;
					switch (reader.GetInt32(0))
					{
						case 2:
							i = reader.GetOrdinal("last_name");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "Emp_no 2 not csUpdate, received " + reader.GetChangeState(i).ToString());
							Assert.AreEqual("Nel", reader.GetString(i), "Emp_no 2 not 'Nel', received " + reader.GetString(i));
							Assert.AreEqual(IBChangeState.csSame, reader.GetChangeState(0), "Emp_no 2 not csSame, received " + reader.GetChangeState(0).ToString());
							break;
						case 4:
							i = reader.GetOrdinal("phone_ext");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "Emp_no 4 not csUpdate, received " + reader.GetChangeState(i).ToString());
							Assert.AreEqual("234", reader.GetString(i), "Emp_no 2 not '234', received " + reader.GetString(i));
							Assert.AreEqual(IBChangeState.csSame, reader.GetChangeState(0), "Emp_no 2 not csSame, received " + reader.GetChangeState(0).ToString());
							break;
					}
				}
				Assert.AreEqual(2, j, "Record Count wrong, expected 2 found " + j.ToString());
			}

		}

		[Test]
		public void ChangeUpdateCol()
		{
			ActivateSubscription("EMPLOYEE_UPDATECOL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";
				var reader = cmd.ExecuteReader();

				int i;
				int j = 0;
				while (reader.Read())
				{
					j++;
					switch (reader.GetInt32(0))
					{
						case 2:
							i = reader.GetOrdinal("last_name");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "last_name not csUpdate, received " + reader.GetChangeState(i).ToString());
							// subscription not tracking emp_no so it should be unknown
							Assert.AreEqual(IBChangeState.csUnknown, reader.GetChangeState(0), "Emp_no 2 not csUnknown, received " + reader.GetChangeState(0).ToString());
							Assert.AreEqual("Nel", reader.GetString(i), "Last_name 2 not 'Nel', received " + reader.GetString(i));
							break;
						case 4:
							i = reader.GetOrdinal("phone_ext");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "Emp_no 4 not csUpdate, received " + reader.GetChangeState(i).ToString());
							Assert.AreEqual("234", reader.GetString(i), "Emp_no 2 not '234', received " + reader.GetString(i));
							break;
					}
				}
				Assert.AreEqual(1, j, "Record Count wrong, expected 2 found " + j.ToString());
			}
		}

		[Test]
		public void ChangeInsertUpdate()
		{
			ActivateSubscription("EMPLOYEE_INSERTUPDATE", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";
				var reader = cmd.ExecuteReader();

				int i;
				int j = 0;
				while (reader.Read())
				{
					j++;
					switch (reader.GetInt32(0))
					{
						case 2:
							i = reader.GetOrdinal("last_name");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "Emp_no 2 not csUpdate, received " + reader.GetChangeState(i).ToString());
							Assert.AreEqual("Nel", reader.GetString(i), "Emp_no 2 not 'Nel', received " + reader.GetString(i));
							break;
						case 4:
							i = reader.GetOrdinal("phone_ext");
							Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i), "Emp_no 4 not csUpdate, received " + reader.GetChangeState(i).ToString());
							break;
						case 65:
							i = reader.GetOrdinal("Emp_no");
							Assert.AreEqual(IBChangeState.csDelete, reader.GetChangeState(i), "Emp_no 65 not csDelete, received " + reader.GetChangeState(i).ToString());
							break;
						case 145:
							i = reader.GetOrdinal("Emp_no");
							Assert.AreEqual(IBChangeState.csInsert, reader.GetChangeState(i), "Emp_no 145 not csInsert, received " + reader.GetChangeState(i).ToString());
							break;
						default:
							Assert.Fail("Unexpected Emp_no " + reader.GetInt32(0));
							break;
					}
				}
				Assert.AreEqual(3, j, "Wrong count, expected 3 records found " + j.ToString());
			}
		}

		[Test]
		public void ChangeToNull()
		{
			ActivateSubscription("EMPLOYEE_UPDATE", "TESTBED");
			// Get rid of seeing the changes made during the setup phase
			ReadEmployeeCommit();
			using (var cmd = Connection.CreateCommand())
			{
				cmd.CommandText = "update employee set last_name = null, phone_ext = null where emp_no = 2";
				cmd.ExecuteNonQuery();
			}
			ActivateSubscription("EMPLOYEE_UPDATE", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee";
				var reader = cmd.ExecuteReader();

				reader.Read();
				int i;
				i = reader.GetOrdinal("last_name");
				Assert.AreEqual(IBChangeState.csUpdate, reader.GetChangeState(i),
					"Last_name not csUpdate, received " + reader.GetChangeState(i).ToString());
				Assert.AreEqual(true, reader.IsDBNull(i), "last_name not NULL Found " + reader.GetValue(i).ToString());

				i = reader.GetOrdinal("first_name");
				Assert.AreEqual(IBChangeState.csSame, reader.GetChangeState(i),
					"First_name not csSame, received " + reader.GetChangeState(i).ToString());
				Assert.AreEqual(false, reader.IsDBNull(i), "first_name not NULL Found " + reader.GetValue(i).ToString());
				Assert.AreEqual("Robert", reader.GetString(i), "First_name is not 'Robert' found " + reader.GetString(i));

				_snaptransaction.Commit();
			}
		}

		[Test]
		public void NewRowDefaults()
		{
			var dt = new IBDataTable();
			dt.Columns.Add("extra", typeof(string));
			dt.Columns.Add("extra2", typeof(Int32));

			var row = dt.GetNewRow();
			dt.Add(row);

			foreach (DataColumn column in dt.Columns)
			{
				Assert.AreEqual(IBChangeState.csUnknown, dt[0].ChangeState(column), "csUnknown not the default, found " + dt[0].ChangeState(column).ToString());
			}
		}

		[Test]
		public void DataTableAddCustomCol()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				IBDataTable table = new IBDataTable();
				dataAdapter.FillWithChangeState(table);

				var col = table.Columns.Add("Change State", typeof(IBChangeState));
				Assert.AreEqual(IBChangeState.csUnknown, table[0].ChangeState("Change State"), "Added column not csUnknown, returned " + table[0].ChangeState("Change State").ToString());
				cmd.Transaction.Rollback();
			}
		}
		[Test]
		public void DataTableChangeColumns()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				IBDataTable table = new IBDataTable();
				table.ChangeStateColumns = true;
				Assert.AreEqual(false, table.ChangeStateColumns, "ChangeStateColumns true when no columns");

				dataAdapter.FillWithChangeState(table);
				var cCount = table.Columns.Count;
				table.ChangeStateColumns = true;
				Assert.AreEqual(cCount * 2, table.Columns.Count, "Column count wrong expected " + (cCount * 2).ToString() + "received " + table.Columns.Count.ToString());
				table.ChangeStateColumns = true;
				Assert.AreEqual(cCount * 2, table.Columns.Count, "Setting true when true Column count wrong expected " + (cCount * 2).ToString() + "received " + table.Columns.Count.ToString());

				for (int i = 0; i < 4; i++)
				{
					switch (Convert.ToInt32(table[i]["emp_no"]))
					{
						case 2:
							Assert.AreEqual(IBChangeState.csUpdate, (IBChangeState)table[i]["last_name ChangeState"], "'last_name ChangeState' for Emp_no 2 not csUpdate, received " + ((IBChangeState)table[i]["last_name ChangeState"]).ToString());
							break;
						case 4:
							Assert.AreEqual(IBChangeState.csUpdate, (IBChangeState)table[i]["phone_ext ChangeState"], "'phone_ext ChangeState' for Emp_no 4 not csUpdate, received " + ((IBChangeState)table[i]["phone_ext ChangeState"]).ToString());
							break;
						case 65:
							Assert.AreEqual(IBChangeState.csDelete, (IBChangeState)table[i]["emp_no ChangeState"], "'emp_no ChangeState' for Emp_no 65 not csDelete, received " + ((IBChangeState)table[i]["emp_no ChangeState"]).ToString());
							break;
						case 145:
							Assert.AreEqual(IBChangeState.csInsert, (IBChangeState)table[i]["emp_no ChangeState"], "'emp_no ChangeState' for Emp_no 145 not csInsert, received " + ((IBChangeState)table[i]["emp_no ChangeState"]).ToString());
							break;
					}
				}

				table.ChangeStateColumns = false;
				Assert.AreEqual(cCount, table.Columns.Count, "Column count wrong expected " + (cCount).ToString() + "received " + table.Columns.Count.ToString());

				cmd.Transaction.Rollback();
			}
		}

		[Test]
		public void ChangeAllDataTable()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				IBDataTable table = new IBDataTable();
				dataAdapter.FillWithChangeState(table);


				Assert.AreEqual(4, table.Rows.Count, "Wrong count, expected 4 records found " + table.Rows.Count.ToString());
				for (int i = 0; i < 4; i++)
				{
					switch (table.Rows[i]["emp_no"].ToString())
					{
						case "2":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("last_name"), "Emp_no 2 not csUpdate, received " + table[i].ChangeState("last_name").ToString());
							Assert.AreEqual("Nel", table[i]["last_name"].ToString(), "Emp_no 2 not 'Nel', received " + table.Rows[i]["last_name"].ToString());
							break;
						case "4":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("phone_ext"), "Emp_no 4 not csUpdate, received " + table[i].ChangeState("phone_ext").ToString());
							Assert.AreEqual("234", table[i]["phone_ext"].ToString(), "Emp_no 4 not '234', received " + table.Rows[i]["phone_ext"].ToString());
							break;
						case "65":
							Assert.AreEqual(IBChangeState.csDelete, table[i].ChangeState("emp_no"), "Emp_no 65 not csDelete, received " + table[i].ChangeState("phone_ext").ToString());
							break;
						case "145":
							Assert.AreEqual(IBChangeState.csInsert, table[i].ChangeState("emp_no"), "Emp_no 145 not csInsert, received " + table[i].ChangeState("emp_no").ToString());
							break;
					}
				}
				cmd.Transaction.Rollback();
			}
		}

		[Test]
		public void ChangeAllDataTableStartEnd()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				IBDataTable table = new IBDataTable { TableName = "employee" };
				dataAdapter.FillWithChangeState(table, 1, 3);

				Assert.AreEqual(3, table.Rows.Count, "Wrong count, expected 3 records found " + table.Rows.Count.ToString());
				for (int i = 0; i < table.Rows.Count; i++)
				{
					switch (table.Rows[i]["emp_no"].ToString())
					{
						case "2":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("last_name"), "Emp_no 2 not csUpdate, received " + table[i].ChangeState("last_name").ToString());
							Assert.AreEqual("Nel", table[i]["last_name"].ToString(), "Emp_no 2 not 'Nel', received " + table.Rows[i]["last_name"].ToString());
							break;
						case "4":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("phone_ext"), "Emp_no 4 not csUpdate, received " + table[i].ChangeState("phone_ext").ToString());
							Assert.AreEqual("234", table[i]["phone_ext"].ToString(), "Emp_no 4 not '234', received " + table.Rows[i]["phone_ext"].ToString());
							break;
						case "65":
							Assert.AreEqual(IBChangeState.csDelete, table[i].ChangeState("emp_no"), "Emp_no 65 not csDelete, received " + table[i].ChangeState("phone_ext").ToString());
							break;
						case "145":
							Assert.AreEqual(IBChangeState.csInsert, table[i].ChangeState("emp_no"), "Emp_no 145 not csInsert, received " + table[i].ChangeState("emp_no").ToString());
							break;
					}
				}
				cmd.Transaction.Rollback();
			}
		}

		[Test]
		public void ChangeAllDataSet()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				DataSet ds = new DataSet();
				dataAdapter.FillWithChangeState(ds);
				IBDataTable table = (IBDataTable) ds.Tables[0];


				Assert.AreEqual(4, table.Rows.Count, "Wrong count, expected 4 records found " + table.Rows.Count.ToString());
				for (int i = 0; i < 4; i++)
				{
					switch (table.Rows[i]["emp_no"].ToString())
					{
						case "2":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("last_name"), "Emp_no 2 not csUpdate, received " + table[i].ChangeState("last_name").ToString());
							Assert.AreEqual("Nel", table[i]["last_name"].ToString(), "Emp_no 2 not 'Nel', received " + table.Rows[i]["last_name"].ToString());
							break;
						case "4":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("phone_ext"), "Emp_no 4 not csUpdate, received " + table[i].ChangeState("phone_ext").ToString());
							Assert.AreEqual("234", table[i]["phone_ext"].ToString(), "Emp_no 4 not '234', received " + table.Rows[i]["phone_ext"].ToString());
							break;
						case "65":
							Assert.AreEqual(IBChangeState.csDelete, table[i].ChangeState("emp_no"), "Emp_no 65 not csDelete, received " + table[i].ChangeState("phone_ext").ToString());
							break;
						case "145":
							Assert.AreEqual(IBChangeState.csInsert, table[i].ChangeState("emp_no"), "Emp_no 145 not csInsert, received " + table[i].ChangeState("emp_no").ToString());
							break;
					}
				}
				cmd.Transaction.Rollback();
			}
		}

		[Test]
		public void ChangeAllDataSetSrcName()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				DataSet ds = new DataSet();
				ds.Tables.Add(new IBDataTable { TableName = "employee" } );
				dataAdapter.FillWithChangeState(ds, "employee");
				IBDataTable table = (IBDataTable)ds.Tables[0];


				Assert.AreEqual(4, table.Rows.Count, "Wrong count, expected 4 records found " + table.Rows.Count.ToString());
				for (int i = 0; i < 4; i++)
				{
					switch (table.Rows[i]["emp_no"].ToString())
					{
						case "2":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("last_name"), "Emp_no 2 not csUpdate, received " + table[i].ChangeState("last_name").ToString());
							Assert.AreEqual("Nel", table[i]["last_name"].ToString(), "Emp_no 2 not 'Nel', received " + table.Rows[i]["last_name"].ToString());
							break;
						case "4":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("phone_ext"), "Emp_no 4 not csUpdate, received " + table[i].ChangeState("phone_ext").ToString());
							Assert.AreEqual("234", table[i]["phone_ext"].ToString(), "Emp_no 4 not '234', received " + table.Rows[i]["phone_ext"].ToString());
							break;
						case "65":
							Assert.AreEqual(IBChangeState.csDelete, table[i].ChangeState("emp_no"), "Emp_no 65 not csDelete, received " + table[i].ChangeState("phone_ext").ToString());
							break;
						case "145":
							Assert.AreEqual(IBChangeState.csInsert, table[i].ChangeState("emp_no"), "Emp_no 145 not csInsert, received " + table[i].ChangeState("emp_no").ToString());
							break;
					}
				}
				cmd.Transaction.Rollback();
			}
		}

		[Test]
		public void ChangeAllDataSetSrcNameStartEnd()
		{
			ActivateSubscription("EMPLOYEE_ALL", "TESTBED");
			using (var cmd = Connection.CreateCommand())
			{
				cmd.Transaction = _snaptransaction;
				cmd.CommandText = "select * from employee order by emp_no";

				var dataAdapter = new IBDataAdapter(cmd);
				DataSet ds = new DataSet();
				ds.Tables.Add(new IBDataTable { TableName = "employee" });
				dataAdapter.FillWithChangeState(ds, 1, 2, "employee");
				IBDataTable table = (IBDataTable)ds.Tables[0];


				Assert.AreEqual(2, table.Rows.Count, "Wrong count, expected 2 records found " + table.Rows.Count.ToString());
				for (int i = 0; i < table.Rows.Count; i++)
				{
					switch (table.Rows[i]["emp_no"].ToString())
					{
						case "2":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("last_name"), "Emp_no 2 not csUpdate, received " + table[i].ChangeState("last_name").ToString());
							Assert.AreEqual("Nel", table[i]["last_name"].ToString(), "Emp_no 2 not 'Nel', received " + table.Rows[i]["last_name"].ToString());
							break;
						case "4":
							Assert.AreEqual(IBChangeState.csUpdate, table[i].ChangeState("phone_ext"), "Emp_no 4 not csUpdate, received " + table[i].ChangeState("phone_ext").ToString());
							Assert.AreEqual("234", table[i]["phone_ext"].ToString(), "Emp_no 4 not '234', received " + table.Rows[i]["phone_ext"].ToString());
							break;
						case "65":
							Assert.AreEqual(IBChangeState.csDelete, table[i].ChangeState("emp_no"), "Emp_no 65 not csDelete, received " + table[i].ChangeState("phone_ext").ToString());
							break;
						case "145":
							Assert.AreEqual(IBChangeState.csInsert, table[i].ChangeState("emp_no"), "Emp_no 145 not csInsert, received " + table[i].ChangeState("emp_no").ToString());
							break;
					}
				}
				cmd.Transaction.Rollback();
			}
		}
		#endregion
	}

	public class ChangeViewTestsDialect1 : ChangeViewTests
	{
		public ChangeViewTestsDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1;
		}
	}

}
