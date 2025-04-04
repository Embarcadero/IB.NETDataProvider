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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Tests.EndToEnd;

public class UpdateTests : EntityFrameworkCoreTestsBase
{
	class UpdateContext : IBTestDbContext
	{
		public UpdateContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<UpdateEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID");
			insertEntityConf.Property(x => x.Foo).HasColumnName("FOO");
			insertEntityConf.Property(x => x.Bar).HasColumnName("BAR");
			insertEntityConf.ToTable("TEST_UPDATE");
		}
	}

	class UpdateEntity
	{
		public int Id { get; set; }
		public string Foo { get; set; }
		public string Bar { get; set; }
	}

	[Test]
	public void Update()
	{
		using (var db = GetDbContext<UpdateContext>())
		{
			db.Database.ExecuteSqlRaw("create table test_update (id int not null primary key, foo varchar(20), bar varchar(20))");
			db.Database.ExecuteSqlRaw("INSERT into test_update values (66, 'foo', 'bar')");
			var entity = new UpdateEntity() { Id = 66, Foo = "test", Bar = "test" };
			var entry = db.Attach(entity);
			entry.Property(x => x.Foo).IsModified = true;
			db.SaveChanges(true);
			var value = db.Set<UpdateEntity>()
			.FromSqlRaw("select * from test_update where id = 66")
			.AsNoTracking()
			.First();
			Assert.AreEqual("test", value.Foo);
			Assert.AreNotEqual("test", value.Bar);
		}
	}

	class ComputedUpdateContext : IBTestDbContext
	{
		public ComputedUpdateContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<ComputedUpdateEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID");
			insertEntityConf.Property(x => x.Foo).HasColumnName("FOO");
			insertEntityConf.Property(x => x.Bar).HasColumnName("BAR");
			insertEntityConf.Property(x => x.fullname).HasColumnName("FULLNAME")
				.ValueGeneratedOnAddOrUpdate();
			insertEntityConf.ToTable("TEST_UPDATE_COMPUTED");
		}
	}
	class ComputedUpdateEntity
	{
		public int Id { get; set; }
		public string Foo { get; set; }
		public string Bar { get; set; }
		public string fullname { get; set; }
	}
	[Test]
	public void ComputedUpdate()
	{
		using (var db = GetDbContext<ComputedUpdateContext>())
		{
			db.Database.ExecuteSqlRaw("create table test_update_computed (id int not null primary key, foo varchar(20), bar varchar(20), fullname computed by (foo || bar))");
			db.Database.ExecuteSqlRaw("insert into test_update_computed (id, foo, bar) values (66, 'foo', 'bar')");
			var entity = new ComputedUpdateEntity() { Id = 66, Foo = "test", Bar = "test" };
			var entry = db.Attach(entity);
			entry.Property(x => x.Foo).IsModified = true;
			db.SaveChanges();
			var value = db.Set<ComputedUpdateEntity>()
							.FromSqlRaw("select * from test_update_computed where id = 66")
							.AsNoTracking()
							.FirstAsync();
			Assert.AreEqual("testbar", value.Result.fullname);
		}
	}

	class ConcurrencyUpdateContext : IBTestDbContext
	{
		public ConcurrencyUpdateContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<ConcurrencyUpdateEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID");
			insertEntityConf.Property(x => x.Foo).HasColumnName("FOO");
			insertEntityConf.Property(x => x.Stamp).HasColumnName("STAMP")
				.ValueGeneratedOnAddOrUpdate()
				.IsConcurrencyToken();
			insertEntityConf.ToTable("TEST_UPDATE_CONCURRENCY");
		}
	}
	class ConcurrencyUpdateEntity
	{
		public int Id { get; set; }
		public string Foo { get; set; }
		public DateTime Stamp { get; set; }
	}
	[Test]
	public void ConcurrencyUpdate()
	{
		using (var db = GetDbContext<ConcurrencyUpdateContext>())
		{
			db.Database.ExecuteSqlRaw("create table test_update_concurrency (id int not null primary key, foo varchar(20), stamp timestamp)");
			db.Database.ExecuteSqlRaw("insert into test_update_concurrency values (66, 'foo', current_timestamp)");
			var entity = new ConcurrencyUpdateEntity() { Id = 66, Foo = "test", Stamp = new DateTime(1970, 1, 1) };
			var entry = db.Attach(entity);
			entry.Property(x => x.Foo).IsModified = true;
			Assert.Throws<DbUpdateConcurrencyException>(() => db.SaveChanges());
		}
	}

	class ConcurrencyUpdateNoGeneratedContext : IBTestDbContext
	{
		public DbSet<ConcurrencyUpdateNoGeneratedEntity> ConcurrencyUpdateNoGeneratedEntity { get; set; }

		public ConcurrencyUpdateNoGeneratedContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<ConcurrencyUpdateNoGeneratedEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID");
			insertEntityConf.Property(x => x.Foo).HasColumnName("FOO");
			insertEntityConf.Property(x => x.Stamp).HasColumnName("STAMP")
				.IsConcurrencyToken();
			insertEntityConf.ToTable("TEST_UPDATE_CONCURRENCY_NG");
		}
	}
	class ConcurrencyUpdateNoGeneratedEntity
	{
		public int Id { get; set; }
		public string Foo { get; set; }
		public DateTime Stamp { get; set; }
	}
	[Test]
	public void ConcurrencyUpdateNoGenerated()
	{
		using (var db = GetDbContext<ConcurrencyUpdateNoGeneratedContext>())
		{
			db.Database.ExecuteSqlRaw("create table test_update_concurrency_ng (id int not null primary key, foo varchar(20), stamp timestamp)");
			db.Database.ExecuteSqlRaw("INSERT into test_update_concurrency_ng values (66, 'foo', current_timestamp)");
			var entity = new ConcurrencyUpdateNoGeneratedEntity() { Id = 66, Foo = "test", Stamp = new DateTime(1970, 1, 1) };
			var entry = db.Attach(entity);
			entry.Property(x => x.Foo).IsModified = true;
			entry.Property(x => x.Stamp).IsModified = true;
			db.SaveChanges();
			entry.State = EntityState.Detached;
			entity = db.ConcurrencyUpdateNoGeneratedEntity
				       .Where(s => s.Id == 66)
					   .FirstOrDefault();
			var value = db.Set<ConcurrencyUpdateNoGeneratedEntity>()
							.FromSqlRaw("select * from test_update_concurrency_ng where id = 66")
							.AsNoTracking()
							.FirstAsync();
			Assert.AreEqual("test", value.Result.Foo);
			Assert.AreEqual(new DateTime(1970, 1, 1), value.Result.Stamp);
		}
	}

	class TwoComputedUpdateContext : IBTestDbContext
	{
		public DbSet<TwoComputedUpdateEntity> TwoComputedUpdateEntity { get; set; }
		public TwoComputedUpdateContext(string connectionString)
			: base(connectionString)
		{ }

		protected override void OnTestModelCreating(ModelBuilder modelBuilder)
		{
			base.OnTestModelCreating(modelBuilder);

			var insertEntityConf = modelBuilder.Entity<TwoComputedUpdateEntity>();
			insertEntityConf.Property(x => x.Id).HasColumnName("ID");
			insertEntityConf.Property(x => x.Foo).HasColumnName("FOO");
			insertEntityConf.Property(x => x.Bar).HasColumnName("BAR");
			insertEntityConf.Property(x => x.Computed1).HasColumnName("COMPUTED1")
				.ValueGeneratedOnAddOrUpdate();
			insertEntityConf.Property(x => x.Computed2).HasColumnName("COMPUTED2")
				.ValueGeneratedOnAddOrUpdate();
			insertEntityConf.ToTable("TEST_UPDATE_2COMPUTED");
		}
	}
	class TwoComputedUpdateEntity
	{
		public int Id { get; set; }
		public string Foo { get; set; }
		public string Bar { get; set; }
		public string Computed1 { get; set; }
		public string Computed2 { get; set; }
	}
	[Test]
	public void TwoComputedUpdate()
	{
		using (var db = GetDbContext<TwoComputedUpdateContext>())
		{
			db.Database.ExecuteSqlRaw("create table test_update_2computed (id int not null, foo varchar(20), bar varchar(20), computed1 computed by (foo || bar), computed2 computed by (bar || bar),  primary key(id))");
			db.Database.ExecuteSqlRaw("INSERT into test_update_2computed (id, foo, bar) values (66, 'foo', 'bar')");
			var entity = new TwoComputedUpdateEntity() { Id = 66, Foo = "test", Bar = "test" };
			var entry = db.Attach(entity);
			entry.Property(x => x.Foo).IsModified = true;
			db.SaveChanges();
			entry.State = EntityState.Detached;
			entity = db.TwoComputedUpdateEntity
				       .Where(s => s.Id == 66)
					   .FirstOrDefault();
			Assert.AreEqual("testbar", entity.Computed1);
			Assert.AreEqual("barbar", entity.Computed2);
		}
	}
}
