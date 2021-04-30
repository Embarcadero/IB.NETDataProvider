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

using BenchmarkDotNet.Attributes;
using InterBaseSql.Data.InterBaseClient;

namespace Perf
{
	partial class CommandBenchmark
	{
		[GlobalSetup(Target = nameof(Fetch))]
		public void FetchGlobalSetup()
		{
			GlobalSetupBase();
			using (var conn = new IBConnection(ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = $"create table foobar (x {DataType})";
					cmd.ExecuteNonQuery();
				}
				using (var cmd = conn.CreateCommand())
				{
					for (var cnt = 200000; cnt > 0; cnt--)
					{
						cmd.CommandText = "insert into foobar values ({cnt});";
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		[Benchmark]
		public void Fetch()
		{
			using (var conn = new IBConnection(ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select x from foobar";
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var dummy = reader[0];
						}
					}
				}
			}
		}
	}
}
