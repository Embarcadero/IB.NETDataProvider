﻿/*
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;

public class IBSqlGenerationHelper : RelationalSqlGenerationHelper, IIBSqlGenerationHelper
{
	public static int Dialect = 3;

	public IBSqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies)
		: base(dependencies)
	{ }

	public virtual string StringLiteralQueryType(string s)
	{
		var length = MinimumStringQueryTypeLength(s);
		EnsureStringLiteralQueryTypeLength(length);
		return $"VARCHAR({length}) CHARACTER SET UTF8";
	}

	public virtual string StringParameterQueryType(bool isUnicode)
	{
		var size = isUnicode ? IBTypeMappingSource.UnicodeVarcharMaxSize : IBTypeMappingSource.VarcharMaxSize;
		return $"VARCHAR({size})";
	}

	public virtual void GenerateBlockParameterName(StringBuilder builder, string name)
	{
		builder.Append("@").Append(name);
	}

	public virtual string AlternativeStatementTerminator => "~";

	static int MinimumStringQueryTypeLength(string s)
	{
		var length = s?.Length ?? 0;
		if (length == 0)
			length = 1;
		return length;
	}

	static void EnsureStringLiteralQueryTypeLength(int length)
	{
		if (length > IBTypeMappingSource.UnicodeVarcharMaxSize)
			throw new ArgumentOutOfRangeException(nameof(length));
	}

	public static string NotEmpty([NotNull] string? value, string parameterName)
	{
		if (value is null)
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw new ArgumentNullException(parameterName);
		}

		if (value.Trim().Length == 0)
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw new ArgumentException(AbstractionsStrings.ArgumentIsEmpty(parameterName));
		}

		return value;
	}
	public override string DelimitIdentifier(string identifier)
	{
		if (Dialect == 3)
			return $"\"{EscapeIdentifier(NotEmpty(identifier, nameof(identifier)))}\"";
		else
			return $"{EscapeIdentifier(NotEmpty(identifier, nameof(identifier)))}";
	}

	public override void DelimitIdentifier(StringBuilder builder, string identifier)
	{
		NotEmpty(identifier, nameof(identifier));

		if (Dialect == 3)
		{
			builder.Append('"');
			EscapeIdentifier(builder, identifier);
			builder.Append('"');
		}
		else
			EscapeIdentifier(builder, identifier);
	}

	public override string DelimitIdentifier(string name, string? schema)
		=> DelimitIdentifier(NotEmpty(name, nameof(name)));

}