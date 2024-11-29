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
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Tests.Query;

public class ElementaryTests : EntityFrameworkCoreTestsBase
{
	[Test]
	public void SimpleSelect()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var data = db.Set<TMPAttachment>().ToList();
			Assert.IsNotEmpty(data);
		}
	}

	[Test]
	public void SelectWithWhere()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Where(x => x.UserName.Trim() != string.Empty);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
			StringAssert.Contains("EF_TRIM(", sql);
		}
	}

	[Test]
	public void SelectWithWhereExtract()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Where(x => x.Timestamp.Second > -1 && x.Timestamp.DayOfYear == 1);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
		}
	}

	[Test]
	public void SelectWithWhereSubstring()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Where(x => x.UserName.Substring(0, 1) == string.Empty && x.UserName.Substring(0, 1) == string.Empty || x.UserName.Substring(x.AttachmentId, x.AttachmentId) != string.Empty || x.UserName.Substring(x.AttachmentId, x.AttachmentId) != string.Empty);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
		}
	}

	[Test]
	public void SelectWithWhereDateMember()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Where(x => x.Timestamp.Date == DateTime.Now.Date);
			query.Load();
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
		}
	}

	[Test]
	public void SelectTake()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Take(3);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
			StringAssert.IsMatch(@"ROWS \(.+\)", sql);
			StringAssert.DoesNotMatch(@" TO \(", sql);
		}
	}

	[Test]
	public void SelectSkipTake()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Skip(1)
				.Take(3);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
			StringAssert.IsMatch(@"ROWS \((.+) \+ 1\) TO \(\1 \+ .+\)", sql);
		}
	}

	[Test]
	public void SelectSkip()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Skip(1);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
			StringAssert.IsMatch(@"ROWS \(.+ \+ 1\) TO \(9223372036854775807\)", sql);
		}
	}

	[Ignore("InterBase does not support Exists in the select section")]
	[Test]
	public void SelectTopLevelAny()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			Assert.DoesNotThrow(() => db.Set<TMPAttachment>().Any(x => x.AttachmentId != 0));
		}
	}

	[Test]
	public void SelectableProcedureWithTable()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			db.CreateProcedures();
			var query = db.Set<TMPAttachment>()
				.Where(x => db.SelectableProcedureWithParam(10).Select(y => y.Value).Contains(x.AttachmentId));
			Assert.DoesNotThrow(() => query.Load());
		}
	}

	[Test]
	public void SelectableProcedureWithParam()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			db.CreateProcedures();
			var query = db.SelectableProcedureWithParam(10).Where(x => x.Value > 10);
			Assert.DoesNotThrow(() => query.Load());
		}
	}

	[Test]
	public void SelectWithCollate()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Where(x => x.UserName == EF.Functions.Collate("test", "UTF8"));
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
			StringAssert.Contains(@"CAST('test' AS VARCHAR(4) CHARACTER SET UTF8) COLLATE UTF8", sql);
		}
	}
}

class SelectContext : IBTestDbContext
{
	class LastCommandTextCommandInterceptor : DbCommandInterceptor
	{
		public string LastCommandText { get; private set; }

		public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
		{
			LastCommandText = command.CommandText;
			return base.NonQueryExecuted(command, eventData, result);
		}

		public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
		{
			LastCommandText = command.CommandText;
			return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
		}

		public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
		{
			LastCommandText = command.CommandText;
			return base.ReaderExecuted(command, eventData, result);
		}

		public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
		{
			LastCommandText = command.CommandText;
			return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
		}

		public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
		{
			LastCommandText = command.CommandText;
			return base.ScalarExecuted(command, eventData, result);
		}

		public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
		{
			LastCommandText = command.CommandText;
			return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
		}
	}

	LastCommandTextCommandInterceptor _lastCommandTextInterceptor;

	public SelectContext(string connectionString)
		: base(connectionString)
	{
		_lastCommandTextInterceptor = new LastCommandTextCommandInterceptor();
	}

	protected override void OnTestModelCreating(ModelBuilder modelBuilder)
	{
		base.OnTestModelCreating(modelBuilder);

		var TMPAttachmentConf = modelBuilder.Entity<TMPAttachment>();
		TMPAttachmentConf.HasKey(x => x.AttachmentId);
		TMPAttachmentConf.Property(x => x.AttachmentId).HasColumnName("TMP$ATTACHMENT_ID");
		TMPAttachmentConf.Property(x => x.UserName).HasColumnName("TMP$USER");
		TMPAttachmentConf.Property(x => x.Timestamp).HasColumnName("TMP$TIMESTAMP");
		TMPAttachmentConf.ToTable("TMP$ATTACHMENTS");

		// InterBase does not support default parameters so this can't be used
		//var selectableProcedureConf = modelBuilder.Entity<SelectableProcedure>();
		//selectableProcedureConf.HasNoKey();
		//selectableProcedureConf.Property(x => x.Value).HasColumnName("VAL");
		//selectableProcedureConf.ToFunction("SELECTABLE_PROCEDURE");

		var selectableProcedureWithParamConf = modelBuilder.Entity<SelectableProcedureWithParam>();
		selectableProcedureWithParamConf.HasNoKey();
		selectableProcedureWithParamConf.Property(x => x.Value).HasColumnName("VAL");
		modelBuilder.HasDbFunction(typeof(SelectContext).GetMethod(nameof(SelectableProcedureWithParam)),
			c => c.HasName("SELECTABLE_PROCEDURE"));

	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);


		optionsBuilder.AddInterceptors(_lastCommandTextInterceptor);
	}

	public string LastCommandText => _lastCommandTextInterceptor.LastCommandText;

	public IQueryable<SelectableProcedureWithParam> SelectableProcedureWithParam(int i) => FromExpression(() => SelectableProcedureWithParam(i));

	public void CreateProcedures()
	{
		Database.ExecuteSqlRaw(
@"create procedure selectable_procedure (i integer)
returns (val integer)
as
begin
	val = 6;
	suspend;
	val = i + 1;
	suspend;
end");
	}
}

class TMPAttachment
{
	public int AttachmentId { get; set; }
	public string UserName { get; set; }
	public DateTime Timestamp { get; set; }
}
class SelectableProcedure
{
	public int Value { get; set; }
}
class SelectableProcedureWithParam
{
	public int Value { get; set; }
}