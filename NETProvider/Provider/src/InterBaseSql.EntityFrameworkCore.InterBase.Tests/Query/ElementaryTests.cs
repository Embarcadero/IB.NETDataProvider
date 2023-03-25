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
			StringAssert.Contains("TRIM(", sql);
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
				.Skip(2)
				.Take(4);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
			StringAssert.IsMatch(@"ROWS \((.+)\) TO \(.+\)", sql);
		}
	}

	[Test]
	public void SelectSkip()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			var query = db.Set<TMPAttachment>()
				.Skip(2);
			Assert.DoesNotThrow(() => query.Load());
			var sql = db.LastCommandText;
			StringAssert.IsMatch(@"ROWS \(.+\) TO \(2147483647\)", sql);
		}
	}

	[Test]
	public void SelectTopLevelAny()
	{
		using (var db = GetDbContext<SelectContext>())
		{
			Assert.DoesNotThrow(() => db.Set<TMPAttachment>().Any(x => x.AttachmentId != 0));
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

		var monAttachmentConf = modelBuilder.Entity<TMPAttachment>();
		monAttachmentConf.HasKey(x => x.AttachmentId);
		monAttachmentConf.Property(x => x.AttachmentId).HasColumnName("TMP$ATTACHMENT_ID");
		monAttachmentConf.Property(x => x.UserName).HasColumnName("TMP$USER");
		monAttachmentConf.Property(x => x.Timestamp).HasColumnName("TMP$TIMESTAMP");
		monAttachmentConf.ToTable("TMP$ATTACHMENTS");
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);


		optionsBuilder.AddInterceptors(_lastCommandTextInterceptor);
	}

	public new string LastCommandText => _lastCommandTextInterceptor.LastCommandText;
}

class TMPAttachment
{
	public int AttachmentId { get; set; }
	public string UserName { get; set; }
	public DateTime Timestamp { get; set; }
}
