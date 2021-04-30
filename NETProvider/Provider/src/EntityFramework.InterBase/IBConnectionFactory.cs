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
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;

using InterBaseSql.Data.InterBaseClient;

namespace EntityFramework.InterBase
{
	public class IBConnectionFactory : IDbConnectionFactory
	{
		public DbConnection CreateConnection(string nameOrConnectionString)
		{
			if (nameOrConnectionString == null)
				throw new ArgumentNullException(nameof(nameOrConnectionString));

			if (nameOrConnectionString.Contains('='))
			{
				return new IBConnection(nameOrConnectionString);
			}
			else
			{
				var configuration = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
				if (configuration == null)
					throw new ArgumentException("Specified connection string name cannot be found.");
				return new IBConnection(configuration.ConnectionString);
			}
		}
	}
}
