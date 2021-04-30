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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System.Data.Common;

namespace InterBaseSql.Data.InterBaseClient
{
	public class InterBaseClientFactory : DbProviderFactory
	{
		#region Static Properties

		public static readonly InterBaseClientFactory Instance = new InterBaseClientFactory();

		#endregion

		#region Properties

		public override bool CanCreateDataSourceEnumerator
		{
			get { return false; }
		}

		#endregion

		#region Constructors

		private InterBaseClientFactory()
			: base()
		{ }

		#endregion

		#region Methods

		public override DbCommand CreateCommand()
		{
			return new IBCommand();
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			return new IBCommandBuilder();
		}

		public override DbConnection CreateConnection()
		{
			return new IBConnection();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new IBConnectionStringBuilder();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return new IBDataAdapter();
		}

		public override DbParameter CreateParameter()
		{
			return new IBParameter();
		}

		#endregion
	}
}
