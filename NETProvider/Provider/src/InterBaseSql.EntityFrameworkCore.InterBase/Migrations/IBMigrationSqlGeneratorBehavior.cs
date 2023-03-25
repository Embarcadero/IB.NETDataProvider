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

using InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Migrations;

public class IBMigrationSqlGeneratorBehavior : IIBMigrationSqlGeneratorBehavior
{
	readonly ISqlGenerationHelper _sqlGenerationHelper;

	public IBMigrationSqlGeneratorBehavior(ISqlGenerationHelper sqlGenerationHelper)
	{
		_sqlGenerationHelper = sqlGenerationHelper;
	}

	public virtual void CreateSequenceTriggerForColumn(string columnName, string tableName, string schemaName, MigrationsSqlGenerationOptions options, MigrationCommandListBuilder builder)
	{
		var identitySequenceName = CreateSequenceTriggerSequenceName(columnName, tableName, schemaName);

		builder.Append("create generator ");
		builder.Append(identitySequenceName);
		builder.AppendLine(_sqlGenerationHelper.StatementTerminator);
		if (options.HasFlag(MigrationsSqlGenerationOptions.Script))
		{
			builder.AppendLine(((IIBSqlGenerationHelper)_sqlGenerationHelper).AlternativeStatementTerminator);
		}
		else
		{
			builder.AppendLine(_sqlGenerationHelper.StatementTerminator);
		}
		builder.EndCommand();

		builder.Append("CREATE TRIGGER ");
		builder.Append(_sqlGenerationHelper.DelimitIdentifier(CreateSequenceTriggerName(columnName, tableName, schemaName)));
		builder.Append(" ACTIVE BEFORE INSERT ON ");
		builder.Append(_sqlGenerationHelper.DelimitIdentifier(tableName, schemaName));
		builder.AppendLine();
		builder.AppendLine("AS");
		builder.AppendLine("BEGIN");
		builder.IncrementIndent();
		builder.Append("if (new.");
		builder.Append(_sqlGenerationHelper.DelimitIdentifier(columnName));
		builder.Append(" is null) then");
		builder.AppendLine();
		builder.AppendLine("begin");
		builder.IncrementIndent();
		builder.Append("new.");
		builder.Append(_sqlGenerationHelper.DelimitIdentifier(columnName));
		builder.Append(" = next value for ");
		builder.Append(identitySequenceName);
		builder.Append(_sqlGenerationHelper.StatementTerminator);
		builder.AppendLine();
		builder.DecrementIndent();
		builder.AppendLine("end");
		builder.DecrementIndent();
		builder.Append("END");
		builder.AppendLine();
		if (options.HasFlag(MigrationsSqlGenerationOptions.Script))
		{
			builder.AppendLine(((IIBSqlGenerationHelper)_sqlGenerationHelper).AlternativeStatementTerminator);
		}
		else
		{
			builder.AppendLine(_sqlGenerationHelper.StatementTerminator);
		}
		builder.EndCommand();

		if (options.HasFlag(MigrationsSqlGenerationOptions.Script))
		{
			builder.Append("SET TERM ");
			builder.Append(_sqlGenerationHelper.StatementTerminator);
			builder.AppendLine(((IIBSqlGenerationHelper)_sqlGenerationHelper).AlternativeStatementTerminator);
			builder.EndCommand();
		}
	}

	public virtual void DropSequenceTriggerForColumn(string columnName, string tableName, string schemaName, MigrationsSqlGenerationOptions options, MigrationCommandListBuilder builder)
	{
		var triggerName = CreateSequenceTriggerName(columnName, tableName, schemaName);

		if (options.HasFlag(MigrationsSqlGenerationOptions.Script))
		{
			builder.Append("SET TERM ");
			builder.Append(((IIBSqlGenerationHelper)_sqlGenerationHelper).AlternativeStatementTerminator);
			builder.AppendLine(_sqlGenerationHelper.StatementTerminator);
			builder.EndCommand();
		}

		builder.Append("execute statement 'drop trigger ");
		builder.Append(_sqlGenerationHelper.DelimitIdentifier(triggerName));
		builder.Append("'");
		builder.Append(_sqlGenerationHelper.StatementTerminator);
		builder.AppendLine();
		if (options.HasFlag(MigrationsSqlGenerationOptions.Script))
		{
			builder.AppendLine(((IIBSqlGenerationHelper)_sqlGenerationHelper).AlternativeStatementTerminator);
		}
		else
		{
			builder.AppendLine(_sqlGenerationHelper.StatementTerminator);
		}
		builder.EndCommand();

		if (options.HasFlag(MigrationsSqlGenerationOptions.Script))
		{
			builder.Append("SET TERM ");
			builder.Append(_sqlGenerationHelper.StatementTerminator);
			builder.AppendLine(((IIBSqlGenerationHelper)_sqlGenerationHelper).AlternativeStatementTerminator);
			builder.EndCommand();
		}
	}

	protected virtual string CreateSequenceTriggerName(string columnName, string tableName, string schemaName)
	{
		return !string.IsNullOrEmpty(schemaName)
			? $"ID_{schemaName}_{tableName}_{columnName}"
			: $"ID_{tableName}_{columnName}";
	}

	protected virtual string CreateSequenceTriggerSequenceName(string columnName, string tableName, string schemaName)
	{
		return "GEN_IDENTITY";
	}
}
