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
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal;

public class IBTimeSpanPartComponentTranslator : IMemberTranslator
{
	const string SecondPart = "SECOND";
	const string MillisecondPart = "MILLISECOND";
	private static readonly Dictionary<MemberInfo, string> MemberMapping = new Dictionary<MemberInfo, string>
		{
			{  typeof(TimeSpan).GetProperty(nameof(TimeSpan.Hours)), "HOUR" },
			{  typeof(TimeSpan).GetProperty(nameof(TimeSpan.Minutes)), "MINUTE" },
			{  typeof(TimeSpan).GetProperty(nameof(TimeSpan.Seconds)), SecondPart },
			{  typeof(TimeSpan).GetProperty(nameof(TimeSpan.Milliseconds)), MillisecondPart },
		};

	readonly IBSqlExpressionFactory _ibSqlExpressionFactory;

	public IBTimeSpanPartComponentTranslator(IBSqlExpressionFactory ibSqlExpressionFactory)
	{
		_ibSqlExpressionFactory = ibSqlExpressionFactory;
	}

	public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
	{
		if (!MemberMapping.TryGetValue(member, out var part))
			return null;

		var result = (SqlExpression)_ibSqlExpressionFactory.SpacedFunction(
			"EXTRACT",
			new[] { _ibSqlExpressionFactory.Fragment(part), _ibSqlExpressionFactory.Fragment("FROM"), instance },
			true,
			new[] { false, false, true },
			typeof(int));
		if (part == SecondPart || part == MillisecondPart)
		{
			result = _ibSqlExpressionFactory.Function("EF_TRUNC", new[] { result }, true, new[] { true }, typeof(int));
		}
		return result;
	}
}