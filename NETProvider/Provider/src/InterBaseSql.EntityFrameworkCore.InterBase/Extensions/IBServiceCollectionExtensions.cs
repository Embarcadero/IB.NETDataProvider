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
using InterBaseSql.EntityFrameworkCore.InterBase;
using InterBaseSql.EntityFrameworkCore.InterBase.Diagnostics.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure;
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Conventions;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Migrations;
using InterBaseSql.EntityFrameworkCore.InterBase.Migrations.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Update.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore;

public static class IBServiceCollectionExtensions
{
	public static IServiceCollection AddInterBase<TContext>(this IServiceCollection serviceCollection, string connectionString, Action<IBDbContextOptionsBuilder> ibOptionsAction = null, Action<DbContextOptionsBuilder> optionsAction = null)
		where TContext : DbContext
	{
		return serviceCollection.AddDbContext<TContext>(
			(serviceProvider, options) =>
			{
				optionsAction?.Invoke(options);
				options.UseInterBase(connectionString, ibOptionsAction);
			});
	}

	public static IServiceCollection AddEntityFrameworkInterBase(this IServiceCollection serviceCollection)
	{
		var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
			.TryAdd<LoggingDefinitions, IBLoggingDefinitions>()
			.TryAdd<IDatabaseProvider, DatabaseProvider<IBOptionsExtension>>()
			.TryAdd<IValueGeneratorCache>(p => p.GetRequiredService<IIBValueGeneratorCache>())
			.TryAdd<IRelationalDatabaseCreator, IBDatabaseCreator>()
			.TryAdd<IRelationalTypeMappingSource, IBTypeMappingSource>()
			.TryAdd<ISqlGenerationHelper, IBSqlGenerationHelper>()
			.TryAdd<IRelationalAnnotationProvider, IBRelationalAnnotationProvider>()
			.TryAdd<IModelValidator, IBModelValidator>()
			.TryAdd<IProviderConventionSetBuilder, IBConventionSetBuilder>()
			.TryAdd<IUpdateSqlGenerator>(p => p.GetService<IIBUpdateSqlGenerator>())
			.TryAdd<IModificationCommandBatchFactory, IBModificationCommandBatchFactory>()
			.TryAdd<IValueGeneratorSelector, IBValueGeneratorSelector>()
			.TryAdd<IRelationalConnection>(p => p.GetService<IIBRelationalConnection>())
			.TryAdd<IRelationalTransactionFactory, IBTransactionFactory>()
			.TryAdd<IMigrationsSqlGenerator, IBMigrationsSqlGenerator>()
			.TryAdd<IHistoryRepository, IBHistoryRepository>()
			.TryAdd<IMemberTranslatorProvider, IBMemberTranslatorProvider>()
			.TryAdd<IMethodCallTranslatorProvider, IBMethodCallTranslatorProvider>()
			.TryAdd<IQuerySqlGeneratorFactory, IBQuerySqlGeneratorFactory>()
			.TryAdd<IQueryTranslationPreprocessorFactory, IBQueryTranslationPreprocessorFactory>()
			.TryAdd<ISqlExpressionFactory, IBSqlExpressionFactory>()
			.TryAdd<ISingletonOptions, IIBOptions>(p => p.GetService<IIBOptions>())
			.TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, IBSqlTranslatingExpressionVisitorFactory>()
			.TryAddProviderSpecificServices(b => b
				.TryAddSingleton<IIBOptions, IBOptions>()
				.TryAddSingleton<IIBMigrationSqlGeneratorBehavior, IBMigrationSqlGeneratorBehavior>()
				.TryAddSingleton<IIBUpdateSqlGenerator, IBUpdateSqlGenerator>()
				.TryAddSingleton<IIBValueGeneratorCache, IBValueGeneratorCache>()
				.TryAddSingleton<IIBSequenceValueGeneratorFactory, IBSequenceValueGeneratorFactory>()
				.TryAddScoped<IIBRelationalConnection, IBRelationalConnection>()
				.TryAddScoped<IIBRelationalTransaction, IBRelationalTransaction>());

		builder.TryAddCoreServices();

		return serviceCollection;
	}
}
