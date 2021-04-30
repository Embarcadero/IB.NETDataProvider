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
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.TestsBase;

namespace EntityFramework.InterBase.Tests
{
	public abstract class EntityFrameworkTestsBase : IBTestsBase
	{
		static EntityFrameworkTestsBase()
		{
#if NET5_0
			System.Data.Common.DbProviderFactories.RegisterFactory(IBProviderServices.ProviderInvariantName, InterBaseClientFactory.Instance);
#endif
			DbConfiguration.SetConfiguration(new IBTestDbContext.Conf());
		}

		public EntityFrameworkTestsBase()
			: base(IBServerType.Default, false)
		{ }

		public DbProviderServices GetProviderServices()
		{
			return IBProviderServices.Instance;
		}

		public TContext GetDbContext<TContext>() where TContext : IBTestDbContext
		{
			Database.SetInitializer<TContext>(null);
			Connection.Close();
			return (TContext)Activator.CreateInstance(typeof(TContext), Connection);
		}
	}
}
