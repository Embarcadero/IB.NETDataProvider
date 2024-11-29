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

using System.Globalization;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities;

public class IBTestStoreFactory : RelationalTestStoreFactory
{
	public static IBTestStoreFactory Instance { get; } = new IBTestStoreFactory();

	static IBTestStoreFactory()
	{
		// See #14847 on EntityFrameworkCore.
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
	}

	public override TestStore Create(string storeName)
		=> IBTestStore.Create(storeName);

	public override TestStore GetOrCreate(string storeName)
		=> IBTestStore.GetOrCreate(storeName);

	public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
		=> serviceCollection.AddEntityFrameworkInterBase();
}