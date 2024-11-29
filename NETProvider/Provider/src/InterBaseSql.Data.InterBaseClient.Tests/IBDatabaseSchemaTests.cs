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

using System.Data;
using System.Threading.Tasks;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class IBDatabaseSchemaTests : IBTestsBase
	{
		#region Constructors

		public IBDatabaseSchemaTests(IBServerType serverType)
			: base(serverType)
		{ }

		#endregion

		#region Non-Async Unit Tests

		[Test]
		public void CharacterSets()
		{
			Connection.GetSchema("CharacterSets");
		}

		[Test]
		public void CheckConstraints()
		{
			Connection.GetSchema("CheckConstraints");
		}

		[Test]
		public void CheckConstraintsByTable()
		{
			Connection.GetSchema("CheckConstraintsByTable");
		}

		[Test]
		public void Collations()
		{
			Connection.GetSchema("Collations");
		}

		[Test]
		public void Columns()
		{
			var columns = Connection.GetSchema("Columns");

			columns = Connection.GetSchema("Columns", new string[] { null, null, "TEST", "INT_FIELD" });

			Assert.AreEqual(1, columns.Rows.Count);
		}

		[Test]
		public void ColumnPrivileges()
		{
			Connection.GetSchema("ColumnPrivileges");
		}

		[Test]
		public void Domains()
		{
			Connection.GetSchema("Domains");
		}

		[Test]
		public void ForeignKeys()
		{
			Connection.GetSchema("ForeignKeys");
		}

		[Test]
		public void ForeignKeyColumns()
		{
			var foreignKeys = Connection.GetSchema("ForeignKeys");

			foreach (DataRow row in foreignKeys.Rows)
			{
				var foreignKeyColumns = Connection.GetSchema(
					"ForeignKeyColumns",
					new string[] { (string)row["TABLE_CATALOG"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"], (string)row["CONSTRAINT_NAME"] });
			}
		}

		[Test]
		public void Functions()
		{
			Connection.GetSchema("Functions");
		}

		[Test]
		public void Generators()
		{
			Connection.GetSchema("Generators");
		}

		[Test]
		public void Indexes()
		{
			Connection.GetSchema("Indexes");
		}

		[Test]
		public void IndexColumns()
		{
			Connection.GetSchema("IndexColumns");
		}

		[Test]
		public void PrimaryKeys()
		{
			var primaryKeys = Connection.GetSchema("PrimaryKeys");

			primaryKeys = Connection.GetSchema("PrimaryKeys", new string[] { null, null, "TEST" });

			Assert.AreEqual(1, primaryKeys.Rows.Count);
		}

		[Test]
		public void ProcedureParameters()
		{
			var procedureParameters = Connection.GetSchema("ProcedureParameters");

			procedureParameters = Connection.GetSchema("ProcedureParameters", new string[] { null, null, "SELECT_DATA" });

			Assert.AreEqual(3, procedureParameters.Rows.Count);
		}

		[Test]
		public void ProcedurePrivileges()
		{
			Connection.GetSchema("ProcedurePrivileges");
		}

		[Test]
		public void Procedures()
		{
			var procedures = Connection.GetSchema("Procedures");

			procedures = Connection.GetSchema("Procedures", new string[] { null, null, "SELECT_DATA" });

			Assert.AreEqual(1, procedures.Rows.Count);
		}

		[Test]
		public void Procedures_ShouldSkipSchemaAndProperlyUseParametersForProcedureName()
		{
			var procedures = Connection.GetSchema("Procedures");

			procedures = Connection.GetSchema("Procedures", new string[] { null, "DUMMY_SCHEMA", "SELECT_DATA" });

			Assert.AreEqual(1, procedures.Rows.Count);
		}

		[Test]
		public void DataTypes()
		{
			Connection.GetSchema("DataTypes");
		}

		[Test]
		public void Roles()
		{
			Connection.GetSchema("Roles");
		}

		[Test]
		public void Tables()
		{
			var tables = Connection.GetSchema("Tables");

			tables = Connection.GetSchema("Tables", new string[] { null, null, "TEST" });

			Assert.AreEqual(1, tables.Rows.Count);

			tables = Connection.GetSchema("Tables", new string[] { null, null, null, "TABLE" });

			Assert.AreEqual(5, tables.Rows.Count);
		}

		[Test]
		public void TableConstraints()
		{
			Connection.GetSchema("TableConstraints");
		}

		[Test]
		public void TablePrivileges()
		{
			Connection.GetSchema("TablePrivileges");
		}

		[Test]
		public void Triggers()
		{
			Connection.GetSchema("Triggers");
		}

		[Test]
		public void UniqueKeys()
		{
			Connection.GetSchema("UniqueKeys");
		}

		[Test]
		public void ViewColumns()
		{
			Connection.GetSchema("ViewColumns");
		}

		[Test]
		public void Views()
		{
			Connection.GetSchema("Views");
		}

		[Test]
		public void ViewPrivileges()
		{
			Connection.GetSchema("ViewPrivileges");
		}

		#endregion

		#region Non-Async Unit Tests

		[Test]
		public async Task CharacterSetsAsync()
		{
			await Connection.GetSchemaAsync("CharacterSets");
		}

		[Test]
		public async Task CheckConstraintsAsync()
		{
			await Connection.GetSchemaAsync("CheckConstraints");
		}

		[Test]
		public async Task CheckConstraintsByTableAsync()
		{
			await Connection.GetSchemaAsync("CheckConstraintsByTable");
		}

		[Test]
		public async Task CollationsAsync()
		{
			await Connection.GetSchemaAsync("Collations");
		}

		[Test]
		public async Task ColumnsAsync()
		{
			var columns = await Connection.GetSchemaAsync("Columns");

			columns = await Connection.GetSchemaAsync("Columns", new string[] { null, null, "TEST", "INT_FIELD" });

			Assert.AreEqual(1, columns.Rows.Count);
		}

		[Test]
		public async Task ColumnPrivilegesAsync()
		{
			await Connection.GetSchemaAsync("ColumnPrivileges");
		}

		[Test]
		public async Task DomainsAsync()
		{
			await Connection.GetSchemaAsync("Domains");
		}

		[Test]
		public async Task ForeignKeysAsync()
		{
			await Connection.GetSchemaAsync("ForeignKeys");
		}

		[Test]
		public async Task ForeignKeyColumnsAsync()
		{
			var foreignKeys = await Connection.GetSchemaAsync("ForeignKeys");

			foreach (DataRow row in foreignKeys.Rows)
			{
				var foreignKeyColumns = await Connection.GetSchemaAsync(
					"ForeignKeyColumns",
					new string[] { (string)row["TABLE_CATALOG"], (string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"], (string)row["CONSTRAINT_NAME"] });
			}
		}

		[Test]
		public async Task FunctionsAsync()
		{
			await Connection.GetSchemaAsync("Functions");
		}

		[Test]
		public async Task GeneratorsAsync()
		{
			await Connection.GetSchemaAsync("Generators");
		}

		[Test]
		public async Task IndexesAsync()
		{
			await Connection.GetSchemaAsync("Indexes");
		}

		[Test]
		public async Task IndexColumnsAsync()
		{
			await Connection.GetSchemaAsync("IndexColumns");
		}

		[Test]
		public async Task PrimaryKeysAsync()
		{
			var primaryKeys = await Connection.GetSchemaAsync("PrimaryKeys");

			primaryKeys = Connection.GetSchema("PrimaryKeys", new string[] { null, null, "TEST" });

			Assert.AreEqual(1, primaryKeys.Rows.Count);
		}

		[Test]
		public async Task ProcedureParametersAsync()
		{
			var procedureParameters = await Connection.GetSchemaAsync("ProcedureParameters");

			procedureParameters = Connection.GetSchema("ProcedureParameters", new string[] { null, null, "SELECT_DATA" });

			Assert.AreEqual(3, procedureParameters.Rows.Count);
		}

		[Test]
		public async Task ProcedurePrivilegesAsync()
		{
			await Connection.GetSchemaAsync("ProcedurePrivileges");
		}

		[Test]
		public async Task ProceduresAsync()
		{
			var procedures = await Connection.GetSchemaAsync("Procedures");

			procedures = Connection.GetSchema("Procedures", new string[] { null, null, "SELECT_DATA" });

			Assert.AreEqual(1, procedures.Rows.Count);
		}

		[Test]
		public async Task Procedures_ShouldSkipSchemaAndProperlyUseParametersForProcedureNameAsync()
		{
			var procedures = await Connection.GetSchemaAsync("Procedures");

			procedures = Connection.GetSchema("Procedures", new string[] { null, "DUMMY_SCHEMA", "SELECT_DATA" });

			Assert.AreEqual(1, procedures.Rows.Count);
		}

		[Test]
		public async Task DataTypesAsync()
		{
			await Connection.GetSchemaAsync("DataTypes");
		}

		[Test]
		public async Task RolesAsync()
		{
			await Connection.GetSchemaAsync("Roles");
		}

		[Test]
		public async Task TablesAsync()
		{
			var tables = await Connection.GetSchemaAsync("Tables");

			tables = await Connection.GetSchemaAsync("Tables", new string[] { null, null, "TEST" });

			Assert.AreEqual(1, tables.Rows.Count);

			tables = await Connection.GetSchemaAsync("Tables", new string[] { null, null, null, "TABLE" });

			Assert.AreEqual(5, tables.Rows.Count);
		}

		[Test]
		public async Task TableConstraintsAsync()
		{
			await Connection.GetSchemaAsync("TableConstraints");
		}

		[Test]
		public async Task TablePrivilegesAsync()
		{
			await Connection.GetSchemaAsync("TablePrivileges");
		}

		[Test]
		public async Task TriggersAsync()
		{
			await Connection.GetSchemaAsync("Triggers");
		}

		[Test]
		public async Task UniqueKeysAsync()
		{
			await Connection.GetSchemaAsync("UniqueKeys");
		}

		[Test]
		public async Task ViewColumnsAsync()
		{
			await Connection.GetSchemaAsync("ViewColumns");
		}

		[Test]
		public async Task ViewsAsync()
		{
			await Connection.GetSchemaAsync("Views");
		}

		[Test]
		public async Task ViewPrivilegesAsync()
		{
			await Connection.GetSchemaAsync("ViewPrivileges");
		}
		#endregion

	}
	public class IBDatabaseSchemaTestsDialect1 : IBDatabaseSchemaTests
	{
		public IBDatabaseSchemaTestsDialect1(IBServerType serverType)
			: base(serverType)
		{
			IBTestsSetup.Dialect = 1;
		}
	}
}
