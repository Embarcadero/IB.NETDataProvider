/*
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
using Microsoft.EntityFrameworkCore;
using InterBaseSql.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Tests.EndToEnd;

public class InsertTests : EntityFrameworkCoreTestsBase
{
	class InsertContext : IBTestDbContext
	{
		public InsertContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<InsertEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID");
			insertEntityConf.Property(x => x.Name).HasColumnName("NAME");
			insertEntityConf.ToTable("TEST_INSERT");
		}
	}
	class InsertEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	[Test]
	public void Insert()
	{
		using (var db = GetDbContext<InsertContext>())
		{
			db.Database.ExecuteSqlRaw("create table test_insert (id int not null primary key, name varchar(20))");
			var entity = new InsertEntity() { Id = -6, Name = "foobar" };
			db.Add(entity);
			db.SaveChanges();
			Assert.AreEqual(-6, entity.Id);
		}
	}

	class IdentityInsertContext : IBTestDbContext
	{
		public IdentityInsertContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<IdentityInsertEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID")
				.UseSequenceTrigger();
			insertEntityConf.Property(x => x.Name).HasColumnName("NAME");
			insertEntityConf.ToTable("TEST_INSERT_IDENTITY");
		}
	}
	class IdentityInsertEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	class SequenceInsertContext : IBTestDbContext
	{
		public SequenceInsertContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<SequenceInsertEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID")
				.UseSequenceTrigger();
			insertEntityConf.Property(x => x.Name).HasColumnName("NAME");
			insertEntityConf.ToTable("TEST_INSERT_SEQUENCE");
		}
	}
	class SequenceInsertEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	[Test]
	public void SequenceInsertNoID()
	{
		using (var db = GetDbContext<SequenceInsertContext>())
		{
			try
			{
				db.Database.ExecuteSqlRaw("drop table test_insert_sequence");
				db.Database.ExecuteSqlRaw("drop generator seq_test_insert_sequence");
			}
			catch { }
			db.Database.ExecuteSqlRaw("create table test_insert_sequence (id int not null primary key, name varchar(20))");
			db.Database.ExecuteSqlRaw("create generator seq_test_insert_sequence");
			db.Database.ExecuteSqlRaw("set generator seq_test_insert_sequence to 30");
			db.Database.ExecuteSqlRaw("create trigger test_insert_sequence_id for test_insert_sequence before insert as begin if (new.id is null) then begin new.id = gen_id(seq_test_insert_sequence, 1); end end");
			var entity = new SequenceInsertEntity() { Name = "foobar" };
			db.Add(entity);
			db.SaveChanges();
			var value = db.Set<SequenceInsertEntity>()
							.FromSqlRaw("select * from test_insert_sequence where name = 'foobar'")
							.AsNoTracking()
							.FirstAsync();
			Assert.AreEqual(31, value.Result.Id);
			Assert.AreEqual("foobar", value.Result.Name);

		}
	}

	[Test]
	public void SequenceInsert()
	{
		using (var db = GetDbContext<SequenceInsertContext>())
		{
			db.Database.ExecuteSqlRaw("create table test_insert_sequence (id int not null primary key, name varchar(20))");
			db.Database.ExecuteSqlRaw("create generator seq_test_insert_sequence");
			db.Database.ExecuteSqlRaw("set generator seq_test_insert_sequence to 30");
			db.Database.ExecuteSqlRaw("create trigger test_insert_sequence_id for test_insert_sequence before insert as begin if (new.id is null) then begin new.id = gen_id(seq_test_insert_sequence, 1); end end");
			var entity = new SequenceInsertEntity() { Name = "foobar" };
			db.Add(entity);
			db.SaveChanges();
			var value = db.Set<SequenceInsertEntity>()
							.FromSqlRaw("select * from test_insert_sequence where name = 'foobar'")
							.AsNoTracking()
							.FirstAsync();
			Assert.AreEqual(31, value.Result.Id);
		}
	}

	class DefaultValuesInsertContext : IBTestDbContext
	{
		public DefaultValuesInsertContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<DefaultValuesInsertEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID")
				.ValueGeneratedOnAdd();
			insertEntityConf.Property(x => x.Name).HasColumnName("NAME")
				.ValueGeneratedOnAdd();
			insertEntityConf.ToTable("TEST_INSERT_DEVAULTVALUES");
		}
	}
	class DefaultValuesInsertEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	class TwoComputedInsertContext : IBTestDbContext
	{
		public TwoComputedInsertContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<TwoComputedInsertEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID")
				.UseSequenceTrigger();
			insertEntityConf.Property(x => x.Name).HasColumnName("NAME");
			insertEntityConf.Property(x => x.Computed1).HasColumnName("COMPUTED1")
				.ValueGeneratedOnAddOrUpdate();
			insertEntityConf.Property(x => x.Computed2).HasColumnName("COMPUTED2")
				.ValueGeneratedOnAddOrUpdate();
			insertEntityConf.ToTable("TEST_INSERT_2COMPUTED");
		}
	}
	class TwoComputedInsertEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Computed1 { get; set; }
		public string Computed2 { get; set; }
	}
	[Test]
	public async Task TwoComputedInsert()
	{
		await using (var db = await GetDbContextAsync<TwoComputedInsertContext>())
		{
			await db.Database.ExecuteSqlRawAsync("create table test_insert_2computed (id int not null primary key, name varchar(20), computed1 computed by ('1' || name), computed2 computed by ('2' || name))");
			var entity = new TwoComputedInsertEntity() { Id = 1, Name = "foobar" };
			await db.AddAsync(entity);
			await db.SaveChangesAsync();
			var value = db.Set<TwoComputedInsertEntity>()
							.FromSqlRaw("select * from test_insert_2computed where name = 'foobar'")
							.AsNoTracking()
							.FirstAsync();
			Assert.AreEqual("1foobar", value.Result.Computed1);
			Assert.AreEqual("2foobar", value.Result.Computed2);
		}
	}
}