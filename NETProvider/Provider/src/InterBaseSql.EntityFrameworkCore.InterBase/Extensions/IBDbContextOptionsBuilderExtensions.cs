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
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure;
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class IBDbContextOptionsBuilderExtensions
{
	public static DbContextOptionsBuilder UseInterBase(this DbContextOptionsBuilder optionsBuilder, string connectionString, Action<IBDbContextOptionsBuilder> ibOptionsAction = null)
	{
		var extension = (IBOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
		((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
		ibOptionsAction?.Invoke(new IBDbContextOptionsBuilder(optionsBuilder));
		return optionsBuilder;
	}

	public static DbContextOptionsBuilder UseInterBase(this DbContextOptionsBuilder optionsBuilder, DbConnection connection, Action<IBDbContextOptionsBuilder> ibOptionsAction = null)
	{
		var extension = (IBOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection);
		((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
		ibOptionsAction?.Invoke(new IBDbContextOptionsBuilder(optionsBuilder));
		return optionsBuilder;
	}

	public static DbContextOptionsBuilder<TContext> UseInterBase<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, string connectionString, Action<IBDbContextOptionsBuilder> ibOptionsAction = null)
		where TContext : DbContext
	{
		return (DbContextOptionsBuilder<TContext>)UseInterBase((DbContextOptionsBuilder)optionsBuilder, connectionString, ibOptionsAction);
	}

	public static DbContextOptionsBuilder<TContext> UseInterBase<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, DbConnection connection, Action<IBDbContextOptionsBuilder> ibOptionsAction = null)
		where TContext : DbContext
	{
		return (DbContextOptionsBuilder<TContext>)UseInterBase((DbContextOptionsBuilder)optionsBuilder, connection, ibOptionsAction);
	}

	static IBOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.Options.FindExtension<IBOptionsExtension>()
			?? new IBOptionsExtension();
}
