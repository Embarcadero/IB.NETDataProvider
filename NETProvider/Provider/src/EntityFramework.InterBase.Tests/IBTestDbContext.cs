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

using System.Data.Entity;
using InterBaseSql.Data.InterBaseClient;

namespace EntityFramework.InterBase.Tests
{
	public class IBTestDbContext : DbContext
	{
		public class Conf : DbConfiguration
		{
			public Conf()
			{
				SetProviderServices(IBProviderServices.ProviderInvariantName, IBProviderServices.Instance);
			}
		}

		public IBTestDbContext(IBConnection conn)
			: base(conn, false)
		{ }
	}
}
