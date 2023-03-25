﻿/*
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
using System.Data.Common;
using System.Threading.Tasks;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.Query;

public class SqlExecutorIBTest : SqlExecutorTestBase<NorthwindQueryIBFixture<NoopModelCustomizer>>
{
	public SqlExecutorIBTest(NorthwindQueryIBFixture<NoopModelCustomizer> fixture)
		: base(fixture)
	{ }

	protected override DbParameter CreateDbParameter(string name, object value)
		=> new IBParameter { ParameterName = name, Value = value };

	protected override string TenMostExpensiveProductsSproc => throw new NotSupportedException();
	protected override string CustomerOrderHistorySproc => throw new NotSupportedException();
	protected override string CustomerOrderHistoryWithGeneratedParameterSproc => throw new NotSupportedException();

	[DoesNotHaveTheDataFact]
	public override void Executes_stored_procedure()
	{
		base.Executes_stored_procedure();
	}

	[DoesNotHaveTheDataFact]
	public override Task Executes_stored_procedure_async()
	{
		return base.Executes_stored_procedure_async();
	}

	[DoesNotHaveTheDataFact]
	public override void Executes_stored_procedure_with_generated_parameter()
	{
		base.Executes_stored_procedure_with_generated_parameter();
	}

	[DoesNotHaveTheDataFact]
	public override Task Executes_stored_procedure_with_generated_parameter_async()
	{
		return base.Executes_stored_procedure_with_generated_parameter_async();
	}

	[DoesNotHaveTheDataFact]
	public override void Executes_stored_procedure_with_parameter()
	{
		base.Executes_stored_procedure_with_parameter();
	}

	[DoesNotHaveTheDataFact]
	public override Task Executes_stored_procedure_with_parameter_async()
	{
		return base.Executes_stored_procedure_with_parameter_async();
	}
}
