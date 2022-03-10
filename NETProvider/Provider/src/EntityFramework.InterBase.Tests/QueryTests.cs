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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using InterBaseSql.Data.InterBaseClient;
using NUnit.Framework;

namespace EntityFramework.InterBase.Tests
{
	public class QueryTests : EntityFrameworkTestsBase
	{
		class QueryTest1Context : IBTestDbContext
		{
			public QueryTest1Context(IBConnection conn)
				: base(conn)
			{ }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
				var queryTest1Entity = modelBuilder.Entity<QueryTest1Entity>();
				queryTest1Entity.Property(x => x.ID).HasColumnName("ID");
				queryTest1Entity.ToTable("TEST_QUERYTEST1ENTITY");
			}

			public IDbSet<QueryTest1Entity> QueryTest1Entity { get; set; }
		}
		[Test]
		public void QueryTest1()
		{
			using (var c = GetDbContext<QueryTest1Context>())
			{
				c.Database.ExecuteSqlCommand("create table test_querytest1entity (id int not null primary key)");
				Assert.DoesNotThrow(() => c.QueryTest1Entity.Max<QueryTest1Entity, int?>(x => x.ID));
			}
		}

		class QueryTest2Context : IBTestDbContext
		{
			public QueryTest2Context(IBConnection conn)
				: base(conn)
			{ }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
			}

			public IDbSet<Foo> Foos { get; set; }
		}
		[Test]
		public void QueryTest2()
		{
			using (var c = GetDbContext<QueryTest2Context>())
			{
				var q = c.Foos
					.OrderBy(x => x.ID)
					.Take(45).Skip(0)
					.Select(x => new
					{
						x.ID,
						x.BazID,
						BazID2 = x.Baz.ID,
						x.Baz.BazString,
					});
				Assert.DoesNotThrow(() =>
				{
					q.ToString();
				});
			}
		}

		class QueryTest3Context : IBTestDbContext
		{
			public QueryTest3Context(IBConnection conn)
				: base(conn)
			{ }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
			}

			public IDbSet<Foo> Foos { get; set; }
		}
		[Test]
		public void QueryTest3()
		{
			using (var c = GetDbContext<QueryTest3Context>())
			{
				var q = c.Foos
					 .OrderByDescending(m => m.Bars.Count())
					 .Skip(3)
					 .SelectMany(m => m.Bars);
				Assert.DoesNotThrow(() =>
				{
					q.ToString();
				});
			}
		}

		class SkipTakeContext : IBTestDbContext
		{
			public SkipTakeContext(IBConnection conn)
				: base(conn)
			{ }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
			}

			public IDbSet<Foo> Foos { get; set; }
		}
		[Test]
		public void SkipTake()
		{
			using (var c = GetDbContext<SkipTakeContext>())
			{
				var q = c.Foos
					.OrderBy(x => x.ID)
					.Take(45).Skip(5);
				StringAssert.Contains("ROWS 45", q.ToString());
				StringAssert.Contains("ROWS 5 + 1", q.ToString());
				Assert.DoesNotThrow(() =>
				{
					q.ToString();
				});
			}
		}

		[Test]
		public void Takeskip()
		{
			using (var c = GetDbContext<SkipTakeContext>())
			{
				var q = c.Foos
					.OrderBy(x => x.ID)
					.Skip(5).Take(45);
				Assert.DoesNotThrow(() =>
				{
					q.ToString();
				});
				StringAssert.Contains("ROWS 6 TO 51", q.ToString());
			}
		}

		class ProperVarcharLengthForConstantContext : IBTestDbContext
		{
			public ProperVarcharLengthForConstantContext(IBConnection conn)
				: base(conn)
			{ }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
			}

			public IDbSet<Bar> Bars { get; set; }
		}
		[Test]
		public void ProperVarcharLengthForConstantTest()
		{
			using (var c = GetDbContext<ProperVarcharLengthForConstantContext>())
			{
				var q = c.Bars.Where(x => x.BarString == "TEST");
				StringAssert.Contains("CAST(_UTF8'TEST' AS VARCHAR(8191))", q.ToString());

			}
		}

		class DbFunctionsContext : IBTestDbContext
		{
			public DbFunctionsContext(IBConnection conn)
				: base(conn)
			{ }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
			}

			public IDbSet<Qux> Quxs { get; set; }
		}
		[Test]
		public void QueryTestDbFunctionsCreateDateTime1()
		{
			using (var c = GetDbContext<DbFunctionsContext>())
			{
				var q = c.Quxs
					.Where(x => x.QuxDateTime == DbFunctions.CreateDateTime(2020, 3, 19, 14, 12, 0))
					.ToString();
				StringAssert.Contains("CAST('2020-3-19 14:12:00' AS TIMESTAMP)", q.ToString());
			}
		}
		[Test]
		public void QueryTestDbFunctionsCreateDateTime2()
		{
			using (var c = GetDbContext<DbFunctionsContext>())
			{
				var q = c.Quxs
					.Where(x => x.QuxDateTime == DbFunctions.CreateDateTime(2020, 3, 19, 14, 12, 36))
					.ToString();
				StringAssert.Contains("EF_DATEADD(SECOND, CAST(36 AS DOUBLE PRECISION), CAST('2020-3-19 14:12:00' AS TIMESTAMP))", q.ToString());
			}
		}
		[Test]
		public void QueryTestDbFunctionsCreateDateTime3()
		{
			using (var c = GetDbContext<DbFunctionsContext>())
			{
				var q = c.Quxs
					.Where(x => x.QuxDateTime == DbFunctions.CreateDateTime(null, null, null, null, null, null))
					.ToString();
				StringAssert.Contains("EF_DATEADD(DAY, -1, EF_DATEADD(MONTH, -1, EF_DATEADD(YEAR, -1, CAST('0001-01-01 00:00:00' AS TIMESTAMP))))", q.ToString());
			}
		}
		[Test]
		public void QueryTestDbFunctionsCreateDateTime4()
		{
			using (var c = GetDbContext<DbFunctionsContext>())
			{
				var q = c.Quxs
					.Where(x => x.QuxDateTime == DbFunctions.CreateDateTime(x.QuxYear, x.QuxMonth, x.QuxDay, null, null, null))
					.ToString();
				StringAssert.Contains("EF_DATEADD(DAY, -1, EF_DATEADD(MONTH, -1, EF_DATEADD(YEAR, -1, EF_DATEADD(DAY, \"B\".\"QuxDay\", EF_DATEADD(MONTH, \"B\".\"QuxMonth\", EF_DATEADD(YEAR, \"B\".\"QuxYear\", CAST('0001-01-01 00:00:00' AS TIMESTAMP)))))))", q.ToString());
			}
		}
	}


	class QueryTest1Entity
	{
		public int ID { get; set; }
	}

	class Foo
	{
		public int ID { get; set; }
		public int BazID { get; set; }
		public ICollection<Bar> Bars { get; set; }
		public Baz Baz { get; set; }
	}
	class Bar
	{
		public int ID { get; set; }
		public int FooID { get; set; }
		public string BarString { get; set; }
		public Foo Foo { get; set; }
	}
	class Baz
	{
		public int ID { get; set; }
		public string BazString { get; set; }
		public ICollection<Foo> Foos { get; set; }
	}
	class Qux
	{
		public int ID { get; set; }
		public DateTime QuxDateTime { get; set; }
		public int QuxYear { get; set; }
		public int QuxMonth { get; set; }
		public int QuxDay { get; set; }
	}
}
