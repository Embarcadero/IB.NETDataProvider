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
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.EntityFrameworkCore.InterBase.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;

public class IBTestHelpers : TestHelpers
{
	protected IBTestHelpers()
	{ }

	public static IBTestHelpers Instance { get; } = new();

	public override IServiceCollection AddProviderServices(IServiceCollection services)
		=> services.AddEntityFrameworkInterBase();

	public override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseInterBase(new IBConnection("database=localhost:_.fdb;user=sysdba;password=masterkey;charset=utf8"));

#pragma warning disable EF1001
	public override LoggingDefinitions LoggingDefinitions { get; } = new IBLoggingDefinitions();
#pragma warning restore EF1001
}