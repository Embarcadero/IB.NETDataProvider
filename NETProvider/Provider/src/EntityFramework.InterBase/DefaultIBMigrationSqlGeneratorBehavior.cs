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

//$Authors = Jiri Cincura (jiri@cincura.net)

using System.Collections.Generic;

namespace EntityFramework.InterBase
{
	public class DefaultIBMigrationSqlGeneratorBehavior : IIBMigrationSqlGeneratorBehavior
	{
		public virtual IEnumerable<string> CreateIdentityForColumn(string columnName, string tableName)
		{
			var identitySequenceName = CreateIdentitySequenceName(columnName, tableName);
			using (var writer = IBMigrationSqlGenerator.SqlWriter())
			{
				writer.Write("create generator ");
				writer.Write(identitySequenceName);
				writer.Write("';");
				writer.WriteLine();
				yield return writer.ToString();
			}

			using (var writer = IBMigrationSqlGenerator.SqlWriter())
			{
				writer.Write("CREATE TRIGGER ");
				writer.Write(IBMigrationSqlGenerator.Quote(CreateTriggerName(columnName, tableName)));
				writer.Write(" BEFORE INSERT ON ");
				writer.Write(IBMigrationSqlGenerator.Quote(tableName));
				writer.WriteLine();
				writer.WriteLine("AS");
				writer.WriteLine("BEGIN");
				writer.Indent++;
				writer.Write("if (new.");
				writer.Write(IBMigrationSqlGenerator.Quote(columnName));
				writer.Write(" is null) then");
				writer.WriteLine();
				writer.WriteLine("begin");
				writer.Indent++;
				writer.Write("new.");
				writer.Write(IBMigrationSqlGenerator.Quote(columnName));
				writer.Write(" = next value for ");
				writer.Write(identitySequenceName);
				writer.Write(";");
				writer.WriteLine();
				writer.Indent--;
				writer.WriteLine("end");
				writer.Indent--;
				writer.Write("END");
				yield return writer.ToString();
			}
		}

		public virtual IEnumerable<string> DropIdentityForColumn(string columnName, string tableName)
		{
			var triggerName = CreateTriggerName(columnName, tableName);
			using (var writer = IBMigrationSqlGenerator.SqlWriter())
			{
				writer.Write("drop trigger ");
				writer.Write(IBMigrationSqlGenerator.Quote(triggerName));
				writer.Write("';");
				writer.WriteLine();
				yield return writer.ToString();
			}
		}

		protected virtual string CreateTriggerName(string columnName, string tableName)
		{
			return string.Format("ID_{0}_{1}", tableName, columnName);
		}

		protected virtual string CreateIdentitySequenceName(string columnName, string tableName)
		{
			return "GEN_IDENTITY";
		}
	}
}
