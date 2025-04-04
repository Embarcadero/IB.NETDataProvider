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
using System.Threading.Tasks;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.TestsBase;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Tests;

public abstract class EntityFrameworkCoreTestsBase : IBTestsBase
{
	public EntityFrameworkCoreTestsBase()
		: base(IBServerType.Default, false)
	{ }

	public TContext GetDbContext<TContext>() where TContext : IBTestDbContext
	{
		Connection.Close();
		return (TContext)Activator.CreateInstance(typeof(TContext), Connection.ConnectionString);
	}
	public async Task<TContext> GetDbContextAsync<TContext>() where TContext : IBTestDbContext
	{
		await Connection.CloseAsync();
		return (TContext)Activator.CreateInstance(typeof(TContext), Connection.ConnectionString);
	}
}
