using System.Reflection;
using InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Extensions;

using System;

/// <summary>
///		InterBaseSQL specific extension methods for <see cref="DatabaseFacade"/>.
/// </summary>
public static class IBDatabaseFacadeExtensions
{
	/// <summary>
	///		<para>
	///			Returns true if the database provider currently in use is the InterBaseSQL provider.
	///		</para>
	///		<para>
	///			This method can only be used after the <see cref="DbContext" /> has been configured because
	///			it is only then that the provider is known. This means that this method cannot be used
	///			in <see cref="DbContext.OnConfiguring" /> because this is where application code sets the
	///			provider to use as part of configuring the context.
	///		</para>
	/// </summary>
	/// <param name="database">
	///		The facade from <see cref="DbContext.Database" />.
	/// </param>
	/// <returns>
	///		True if InterBasedSQL is being used; false otherwise.
	/// </returns>
	public static bool IsInterBase(this DatabaseFacade database)
		=> database.ProviderName.Equals(typeof(IBOptionsExtension).GetTypeInfo().Assembly.GetName().Name, StringComparison.Ordinal);
}
