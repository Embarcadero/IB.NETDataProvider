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
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Migrations.Sql;
using System.Diagnostics;
using System.Linq;
using EntityFramework.InterBase.SqlGen;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.Isql;
using InterBaseSql.Data.Services;

namespace EntityFramework.InterBase
{
	public class IBProviderServices : DbProviderServices
	{
		public const string ProviderInvariantName = "InterBaseSql.Data.InterBaseClient";
		public static readonly IBProviderServices Instance = new IBProviderServices();

		public IBProviderServices()
		{
			AddDependencyResolver(new SingletonDependencyResolver<IDbConnectionFactory>(new IBConnectionFactory()));
			AddDependencyResolver(new SingletonDependencyResolver<Func<MigrationSqlGenerator>>(() => new IBMigrationSqlGenerator(), ProviderInvariantName));
			DbInterception.Add(new IBMigrationsTransactionsInterceptor());
		}

		protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest manifest, DbCommandTree commandTree)
		{
			var prototype = CreateCommand(manifest, commandTree);
			var result = CreateCommandDefinition(prototype);
			return result;
		}

		private DbCommand CreateCommand(DbProviderManifest manifest, DbCommandTree commandTree)
		{
			if (manifest == null)
				throw new ArgumentNullException("manifest");

			if (commandTree == null)
				throw new ArgumentNullException("commandTree");

			var expectedTypes = PrepareTypeCoercions(commandTree);

			var command = IBCommand.CreateWithTypeCoercions(expectedTypes);

			command.CommandText = SqlGenerator.GenerateSql(commandTree, out var parameters, out var commandType);
			command.CommandType = commandType;

			// Get the function (if any) implemented by the command tree since this influences our interpretation of parameters
			EdmFunction function = null;
			if (commandTree is DbFunctionCommandTree)
			{
				function = ((DbFunctionCommandTree)commandTree).EdmFunction;
			}

			// Now make sure we populate the command's parameters from the CQT's parameters:
			foreach (var queryParameter in commandTree.Parameters)
			{
				IBParameter parameter;

				// Use the corresponding function parameter TypeUsage where available (currently, the SSDL facets and
				// type trump user-defined facets and type in the EntityCommand).
				if (null != function && function.Parameters.TryGetValue(queryParameter.Key, false, out var functionParameter))
				{
					parameter = CreateSqlParameter(functionParameter.Name, functionParameter.TypeUsage, functionParameter.Mode, DBNull.Value);
				}
				else
				{
					parameter = CreateSqlParameter(queryParameter.Key, queryParameter.Value, ParameterMode.In, DBNull.Value);
				}

				command.Parameters.Add(parameter);
			}

			// Now add parameters added as part of SQL gen (note: this feature is only safe for DML SQL gen which
			// does not support user parameters, where there is no risk of name collision)
			if (null != parameters && 0 < parameters.Count)
			{
				if (!(commandTree is DbInsertCommandTree) &&
					!(commandTree is DbUpdateCommandTree) &&
					!(commandTree is DbDeleteCommandTree))
				{
					throw new InvalidOperationException("SqlGenParametersNotPermitted");
				}

				foreach (var parameter in parameters)
				{
					command.Parameters.Add(parameter);
				}
			}

			return command;
		}

		protected override string GetDbProviderManifestToken(DbConnection connection)
		{
			try
			{
				var serverVersion = default(Version);
				if (connection.State == ConnectionState.Open)
				{
					serverVersion = IBServerProperties.ParseServerVersion(connection.ServerVersion);
				}
				else
				{
					var serverProperties = new IBServerProperties() { ConnectionString = connection.ConnectionString };
					serverVersion = IBServerProperties.ParseServerVersion(serverProperties.GetServerVersion());
				}
				return serverVersion.ToString(2);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Could not retrieve storage version.", ex);
			}
		}

		protected override DbProviderManifest GetDbProviderManifest(string versionHint)
		{
			if (string.IsNullOrEmpty(versionHint))
			{
				throw new ArgumentException("Could not determine store version; a valid store connection or a version hint is required.");
			}
			return new IBProviderManifest(versionHint);
		}

		internal static IBParameter CreateSqlParameter(string name, TypeUsage type, ParameterMode mode, object value)
		{
			var result = new IBParameter(name, value);

			var direction = MetadataHelpers.ParameterModeToParameterDirection(mode);
			if (result.Direction != direction)
			{
				result.Direction = direction;
			}

			// output parameters are handled differently (we need to ensure there is space for return
			// values where the user has not given a specific Size/MaxLength)
			var isOutParam = mode != ParameterMode.In;
			var sqlDbType = GetSqlDbType(type, isOutParam, out var size);

			if (result.IBDbType != sqlDbType)
			{
				result.IBDbType = sqlDbType;
			}

			// Note that we overwrite 'facet' parameters where either the value is different or
			// there is an output parameter.
			if (size.HasValue && (isOutParam || result.Size != size.Value))
			{
				result.Size = size.Value;
			}

			var isNullable = MetadataHelpers.IsNullable(type);
			if (isOutParam || isNullable != result.IsNullable)
			{
				result.IsNullable = isNullable;
			}

			return result;
		}

		private static IBDbType GetSqlDbType(TypeUsage type, bool isOutParam, out int? size)
		{
			// only supported for primitive type
			var primitiveTypeKind = MetadataHelpers.GetPrimitiveTypeKind(type);

			size = default;

			switch (primitiveTypeKind)
			{
				case PrimitiveTypeKind.Boolean:
					return IBDbType.SmallInt;

				case PrimitiveTypeKind.Int16:
					return IBDbType.SmallInt;

				case PrimitiveTypeKind.Int32:
					return IBDbType.Integer;

				case PrimitiveTypeKind.Int64:
					return IBDbType.BigInt;

				case PrimitiveTypeKind.Double:
					return IBDbType.Double;

				case PrimitiveTypeKind.Single:
					return IBDbType.Float;

				case PrimitiveTypeKind.Decimal:
					return IBDbType.Decimal;

				case PrimitiveTypeKind.Binary:
					// for output parameters, ensure there is space...
					size = GetParameterSize(type, isOutParam);
					return GetBinaryDbType(type);

				case PrimitiveTypeKind.String:
					size = GetParameterSize(type, isOutParam);
					return GetStringDbType(type);

				case PrimitiveTypeKind.DateTime:
					return IBDbType.TimeStamp;

				case PrimitiveTypeKind.Time:
					return IBDbType.Time;

				case PrimitiveTypeKind.Guid:
					return IBDbType.Guid;

				default:
					Debug.Fail("unknown PrimitiveTypeKind " + primitiveTypeKind);
					throw new InvalidOperationException("unknown PrimitiveTypeKind " + primitiveTypeKind);
			}
		}

		private static int? GetParameterSize(TypeUsage type, bool isOutParam)
		{
			if (MetadataHelpers.TryGetMaxLength(type, out var maxLength))
			{
				// if the MaxLength facet has a specific value use it
				return maxLength;
			}
			else if (isOutParam)
			{
				// if the parameter is a return/out/inout parameter, ensure there
				// is space for any value
				return int.MaxValue;
			}
			else
			{
				// no value
				return default;
			}
		}

		private static IBDbType GetStringDbType(TypeUsage type)
		{
			Debug.Assert(type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType && PrimitiveTypeKind.String == ((PrimitiveType)type.EdmType).PrimitiveTypeKind, "only valid for string type");

			IBDbType dbType;
			// Specific type depends on whether the string is a unicode string and whether it is a fixed length string.
			// By default, assume widest type (unicode) and most common type (variable length)
			if (!MetadataHelpers.TryGetIsFixedLength(type, out var fixedLength))
			{
				fixedLength = false;
			}

			if (!MetadataHelpers.TryGetIsUnicode(type, out var unicode))
			{
				unicode = true;
			}

			if (fixedLength)
			{
				dbType = (unicode ? IBDbType.Char : IBDbType.Char);
			}
			else
			{
				if (!MetadataHelpers.TryGetMaxLength(type, out var maxLength))
				{
					maxLength = (unicode ? IBProviderManifest.UnicodeVarcharMaxSize : IBProviderManifest.AsciiVarcharMaxSize);
				}
				if (maxLength == default || maxLength > (unicode ? IBProviderManifest.UnicodeVarcharMaxSize : IBProviderManifest.AsciiVarcharMaxSize))
				{
					dbType = IBDbType.Text;
				}
				else
				{
					dbType = (unicode ? IBDbType.VarChar : IBDbType.VarChar);
				}
			}

			return dbType;
		}

		private static IBDbType GetBinaryDbType(TypeUsage type)
		{
			Debug.Assert(type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType &&
				PrimitiveTypeKind.Binary == ((PrimitiveType)type.EdmType).PrimitiveTypeKind, "only valid for binary type");

			// Specific type depends on whether the binary value is fixed length. By default, assume variable length.
			//bool fixedLength;
			//if (!MetadataHelpers.TryGetIsFixedLength(type, out fixedLength))
			//{
			//    fixedLength = false;
			//}

			return IBDbType.Binary;
		}

		private static Type[] PrepareTypeCoercions(DbCommandTree commandTree)
		{
			if (commandTree is DbQueryCommandTree queryTree)
			{
				if (queryTree.Query is DbProjectExpression projectExpression)
				{
					var resultsType = projectExpression.Projection.ResultType.EdmType;
					if (resultsType is StructuralType resultsAsStructuralType)
					{
						var members = resultsAsStructuralType.Members;
						return members.Select(ExtractExpectedTypeForCoercion).ToArray();
					}
				}
			}

			if (commandTree is DbFunctionCommandTree functionTree)
			{
				if (functionTree.ResultType != null)
				{
					Debug.Assert(MetadataHelpers.IsCollectionType(functionTree.ResultType.EdmType), "Result type of a function is expected to be a collection of RowType or PrimitiveType");

					var typeUsage = MetadataHelpers.GetElementTypeUsage(functionTree.ResultType);
					var elementType = typeUsage.EdmType;
					if (MetadataHelpers.IsRowType(elementType))
					{
						var members = ((RowType)elementType).Members;
						return members.Select(ExtractExpectedTypeForCoercion).ToArray();
					}
					else if (MetadataHelpers.IsPrimitiveType(elementType))
					{
						return new[] { MakeTypeCoercion(((PrimitiveType)elementType).ClrEquivalentType, typeUsage) };
					}
					else
					{
						Debug.Fail("Result type of a function is expected to be a collection of RowType or PrimitiveType");
					}
				}
			}

			return null;
		}

		private static Type ExtractExpectedTypeForCoercion(EdmMember member)
		{
			var type = ((PrimitiveType)member.TypeUsage.EdmType).ClrEquivalentType;
			return MakeTypeCoercion(type, member.TypeUsage);
		}

		private static Type MakeTypeCoercion(Type type, TypeUsage typeUsage)
		{
			if (type.IsValueType && MetadataHelpers.IsNullable(typeUsage))
				return typeof(Nullable<>).MakeGenericType(type);
			return type;
		}

		protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout,
#pragma warning disable 3001
			StoreItemCollection storeItemCollection)
#pragma warning restore 3001
		{
			IBConnection.CreateDatabase(connection.ConnectionString, pageSize: 16384);
			var script = DbCreateDatabaseScript(GetDbProviderManifestToken(connection), storeItemCollection);
			var fbScript = new IBScript(script);
			fbScript.Parse();
			if (fbScript.Results.Any())
			{
				using (var IBConnection = new IBConnection(connection.ConnectionString))
				{
					var execution = new IBBatchExecution(IBConnection);
					execution.AppendSqlStatements(fbScript);
					execution.Execute();
				}
			}
		}

		protected override string DbCreateDatabaseScript(string providerManifestToken,
#pragma warning disable 3001
			StoreItemCollection storeItemCollection)
#pragma warning restore 3001
		{
			return SsdlToIB.Transform(storeItemCollection, providerManifestToken);
		}

		protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout,
#pragma warning disable 3001
			StoreItemCollection storeItemCollection)
#pragma warning restore 3001
		{
			if (connection.State == ConnectionState.Open
				   || connection.State == ConnectionState.Executing
				   || connection.State == ConnectionState.Fetching)
			{
				return true;
			}
			else
			{
				try
				{
					connection.Open();
					return true;
				}
				catch
				{
					return false;
				}
				finally
				{
					try
					{
						connection.Close();
					}
					catch { }
				}
			}
		}

		protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout,
#pragma warning disable 3001
			StoreItemCollection storeItemCollection)
#pragma warning restore 3001
		{
			IBConnection.DropDatabase(connection.ConnectionString);
		}
	}
}
