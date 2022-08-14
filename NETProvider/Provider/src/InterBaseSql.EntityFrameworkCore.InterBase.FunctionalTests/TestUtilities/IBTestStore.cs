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

using System;
using InterBaseSql.Data.InterBaseClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities
{
	public class IBTestStore : RelationalTestStore
	{
		public static IBTestStore Create(string name)
			=> new IBTestStore(name, shared: false);

		public static IBTestStore GetOrCreate(string name)
			=> new IBTestStore(name, shared: true);

		public IBTestStore(string name, bool shared)
			: base(name, shared)
		{
     		        var path = AppDomain.CurrentDomain.BaseDirectory;
			var csb = new IBConnectionStringBuilder
			{
				Database = $"{path}EFCore_{name}.ib",
				DataSource = "localhost",
				UserID = "sysdba",
				Password = "masterkey",
				Pooling = false,
				Charset = "utf8"
			};
			ConnectionString = csb.ToString();
			Connection = new IBConnection(ConnectionString);
		}

		protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean)
		{
			using (var context = createContext())
			{
				// create database explicitly to specify Page Size and Forced Writes
				IBConnection.CreateDatabase(ConnectionString, pageSize: 16384, forcedWrites: false, overwrite: true);
				context.Database.EnsureCreated();
				clean?.Invoke(context);
				Clean(context);
				seed?.Invoke(context);
			}
		}

		public override void Dispose()
		{
			Connection.Dispose();
			base.Dispose();
		}

		public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
			=> builder.UseInterBase(Connection);

		public override void Clean(DbContext context)
		{ }
	}
}
