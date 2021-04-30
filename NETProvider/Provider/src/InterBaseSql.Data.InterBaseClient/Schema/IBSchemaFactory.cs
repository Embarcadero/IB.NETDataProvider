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
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

using InterBaseSql.Data.InterBaseClient;

namespace InterBaseSql.Data.Schema
{
	internal sealed class IBSchemaFactory
	{
		#region Static Members

		private static readonly string ResourceName = "InterBaseSql.Data.Schema.IBMetaData.xml";

		#endregion

		#region Constructors

		private IBSchemaFactory()
		{
		}

		#endregion

		#region Methods

		public static DataTable GetSchema(IBConnection connection, string collectionName, string[] restrictions)
		{
			var filter = string.Format("CollectionName = '{0}'", collectionName);
			var ds = new DataSet();
			using (var xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
			{
				var oldCulture = Thread.CurrentThread.CurrentCulture;
				try
				{
					Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
					// ReadXml contains error: http://connect.microsoft.com/VisualStudio/feedback/Validation.aspx?FeedbackID=95116
					// that's the reason for temporarily changing culture
					ds.ReadXml(xmlStream);
				}
				finally
				{
					Thread.CurrentThread.CurrentCulture = oldCulture;
				}
			}

			var collection = ds.Tables[DbMetaDataCollectionNames.MetaDataCollections].Select(filter);

			if (collection.Length != 1)
			{
				throw new NotSupportedException("Unsupported collection name.");
			}

			if (restrictions != null && restrictions.Length > (int)collection[0]["NumberOfRestrictions"])
			{
				throw new InvalidOperationException("The number of specified restrictions is not valid.");
			}

			if (ds.Tables[DbMetaDataCollectionNames.Restrictions].Select(filter).Length != (int)collection[0]["NumberOfRestrictions"])
			{
				throw new InvalidOperationException("Incorrect restriction definition.");
			}

			switch (collection[0]["PopulationMechanism"].ToString())
			{
				case "PrepareCollection":
					return PrepareCollection(connection, collectionName, restrictions);

				case "DataTable":
					return ds.Tables[collection[0]["PopulationString"].ToString()].Copy();

				case "SQLCommand":
					return SqlCommandSchema(connection, collectionName, restrictions);

				default:
					throw new NotSupportedException("Unsupported population mechanism");
			}
		}

		#endregion

		#region Private Methods

		private static DataTable PrepareCollection(IBConnection connection, string collectionName, string[] restrictions)
		{
			IBSchema returnSchema = collectionName.ToUpperInvariant() switch
			{
				"CHARACTERSETS" => new IBCharacterSets(),
				"CHECKCONSTRAINTS" => new IBCheckConstraints(),
				"CHECKCONSTRAINTSBYTABLE" => new IBChecksByTable(),
				"COLLATIONS" => new IBCollations(),
				"COLUMNS" => new IBColumns(),
				"COLUMNPRIVILEGES" => new IBColumnPrivileges(),
				"DOMAINS" => new IBDomains(),
				"FOREIGNKEYCOLUMNS" => new IBForeignKeyColumns(),
				"FOREIGNKEYS" => new IBForeignKeys(),
				"FUNCTIONS" => new IBFunctions(),
				"GENERATORS" => new IBGenerators(),
				"INDEXCOLUMNS" => new IBIndexColumns(),
				"INDEXES" => new IBIndexes(),
				"PRIMARYKEYS" => new IBPrimaryKeys(),
				"PROCEDURES" => new IBProcedures(),
				"PROCEDUREPARAMETERS" => new IBProcedureParameters(),
				"PROCEDUREPRIVILEGES" => new IBProcedurePrivilegesSchema(),
				"ROLES" => new IBRoles(),
				"TABLES" => new IBTables(),
				"TABLECONSTRAINTS" => new IBTableConstraints(),
				"TABLEPRIVILEGES" => new IBTablePrivileges(),
				"TRIGGERS" => new IBTriggers(),
				"UNIQUEKEYS" => new IBUniqueKeys(),
				"VIEWCOLUMNS" => new IBViewColumns(),
				"VIEWS" => new IBViews(),
				"VIEWPRIVILEGES" => new IBViewPrivileges(),
				_ => throw new NotSupportedException("The specified metadata collection is not supported."),
			};
			return returnSchema.GetSchema(connection, collectionName, restrictions);
		}

		private static DataTable SqlCommandSchema(IBConnection connection, string collectionName, string[] restrictions)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
