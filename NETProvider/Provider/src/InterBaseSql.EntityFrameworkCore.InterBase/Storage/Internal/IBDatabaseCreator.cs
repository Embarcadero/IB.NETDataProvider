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

//$Authors = Jiri Cincura (jiri@cincura.net), Jean Ressouche, Rafael Almeida (ralms@ralms.net)

using System;
using System.Threading;
using System.Threading.Tasks;
using InterBaseSql.Data.Common;
using InterBaseSql.Data.InterBaseClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;

public class IBDatabaseCreator : RelationalDatabaseCreator
{
	readonly IIBRelationalConnection _connection;
	readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

	public IBDatabaseCreator(RelationalDatabaseCreatorDependencies dependencies, IIBRelationalConnection connection, IRawSqlCommandBuilder rawSqlCommandBuilder)
		: base(dependencies)
	{
		_connection = connection;
		_rawSqlCommandBuilder = rawSqlCommandBuilder;
	}

	public override void Create()
	{
		IBConnection.CreateDatabase(_connection.ConnectionString, pageSize: 16384);

		var designTimeModel = Dependencies.CurrentContext.Context.GetService<IDesignTimeModel>().Model;

		// uses Execute Block - revisit later

		//var collation = designTimeModel.GetCollation();
		//if (collation != null)
		//{
		//	Dependencies.ExecutionStrategy.Execute(
		//		_connection,
		//		connection => CreateAlterCollationCommand(collation).ExecuteNonQuery(
		//			new RelationalCommandParameterObject(
		//				connection,
		//				null,
		//				null,
		//				Dependencies.CurrentContext.Context,
		//				Dependencies.CommandLogger)));
		//}
	}
	public override async Task CreateAsync(CancellationToken cancellationToken = default)
	{
		await IBConnection.CreateDatabaseAsync(_connection.ConnectionString, pageSize: 16384, cancellationToken: cancellationToken).ConfigureAwait(false);

		var designTimeModel = Dependencies.CurrentContext.Context.GetService<IDesignTimeModel>().Model;

		// uses Execute Block - revisit later

		//var collation = designTimeModel.GetCollation();
		//if (collation != null)
		//{
		//	await Dependencies.ExecutionStrategy.ExecuteAsync(
		//		_connection,
		//		connection => CreateAlterCollationCommand(collation).ExecuteNonQueryAsync(
		//			new RelationalCommandParameterObject(
		//				connection,
		//				null,
		//				null,
		//				Dependencies.CurrentContext.Context,
		//				Dependencies.CommandLogger))).ConfigureAwait(false);
		//}
	}

	public override void Delete()
	{
		IBConnection.ClearPool((IBConnection)_connection.DbConnection);
		IBConnection.DropDatabase(_connection.ConnectionString);
	}
	public override Task DeleteAsync(CancellationToken cancellationToken = default)
	{
		IBConnection.ClearPool((IBConnection)_connection.DbConnection);
		return IBConnection.DropDatabaseAsync(_connection.ConnectionString, cancellationToken);
	}

	public override bool Exists()
	{
		try
		{
			_connection.Open();
			return true;
		}
		catch (IBException)
		{
			return false;
		}
		finally
		{
			_connection.Close();
		}
	}
	public override async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
			return true;
		}
		catch (IBException)
		{
			return false;
		}
		finally
		{
			await _connection.CloseAsync().ConfigureAwait(false);
		}
	}

	public override bool HasTables()
	{
		return Dependencies.ExecutionStrategy.Execute(
		_connection,
		connection => Convert.ToInt64(CreateHasTablesCommand().ExecuteScalar(
			new RelationalCommandParameterObject(
				connection,
				null,
				null,
				Dependencies.CurrentContext.Context,
				Dependencies.CommandLogger)))
			!= 0);
	}
	public override Task<bool> HasTablesAsync(CancellationToken cancellationToken = default)
	{
		return Dependencies.ExecutionStrategy.ExecuteAsync(
			_connection,
			async (connection, ct) => Convert.ToInt64(await CreateHasTablesCommand().ExecuteScalarAsync(
				new RelationalCommandParameterObject(
					connection,
					null,
					null,
					Dependencies.CurrentContext.Context,
					Dependencies.CommandLogger),
				ct).ConfigureAwait(false))
				!= 0,
			cancellationToken);
	}

	IRelationalCommand CreateHasTablesCommand()
	   => _rawSqlCommandBuilder
		   .Build("SELECT COUNT(*) FROM rdb$relations WHERE COALESCE(rdb$system_flag, 0) = 0 AND rdb$view_blr IS NULL");

//	IRelationalCommand CreateAlterCollationCommand(string collation)
//		=> _rawSqlCommandBuilder
//			.Build($@"EXECUTE BLOCK
//AS
//BEGIN
//	execute statement 'alter character set ' || (select coalesce(trim(rdb$character_set_name), 'NONE') from rdb$database) || ' set default collation {collation}';
//END");
}