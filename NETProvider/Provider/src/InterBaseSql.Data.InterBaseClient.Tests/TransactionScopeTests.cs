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

#if NETCOREAPP3_1 || NET5_0
using System;
using System.Transactions;
using InterBaseSql.Data.TestsBase;
using NUnit.Framework;

namespace InterBaseSql.Data.InterBaseClient.Tests
{
	[TestFixtureSource(typeof(IBDefaultServerTypeTestFixtureSource))]
	[TestFixtureSource(typeof(IBEmbeddedServerTypeTestFixtureSource))]
	public class TransactionScopeTests : IBTestsBase
	{
#region Constructors

		public TransactionScopeTests(IBServerType serverType)
			: base(serverType)
		{ }

#endregion

#region Unit Tests

		[Test]
		public void SimpleSelectTest()
		{
			var csb = BuildConnectionStringBuilder(IBServerType);

			csb.Enlist = true;

			using (var scope = new TransactionScope())
			{
				using (var c = new IBConnection(csb.ToString()))
				{
					c.Open();

					using (var command = new IBCommand("select * from TEST where (0=1)", c))
					{
						using (var r = command.ExecuteReader())
						{
							while (r.Read())
							{
							}
						}
					}
				}

				scope.Complete();
			}
		}

		[Test]
		public void InsertTest()
		{
			var csb = BuildConnectionStringBuilder(IBServerType);

			csb.Enlist = true;

			using (var scope = new TransactionScope())
			{
				using (var c = new IBConnection(csb.ToString()))
				{
					c.Open();

					var sql = "insert into TEST (int_field, date_field) values (1002, @date)";

					using (var command = new IBCommand(sql, c))
					{
						command.Parameters.Add("@date", IBDbType.Date).Value = DateTime.Now.ToString();

						var ra = command.ExecuteNonQuery();

						Assert.AreEqual(ra, 1);
					}
				}

				scope.Complete();
			}
		}

#endregion
	}
}
#endif
