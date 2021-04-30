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

using System;
//using InterBaseSql.Data.Client.Managed;
//using InterBaseSql.Data.Client.Managed.Version13;
using InterBaseSql.Data.Common;

namespace InterBaseSql.Data.InterBaseClient
{
	internal static class ClientFactory
	{
		public static IDatabase CreateIDatabase(ConnectionString options)
		{
			switch (options.ServerType)
			{
				case IBServerType.Default:
				case IBServerType.Embedded:
					return new Client.Native.IBDatabase(options.ServerType, Charset.GetCharset(options.Charset));
				default:
					throw IncorrectServerTypeException();
			}
		}

		public static IServiceManager CreateIServiceManager(ConnectionString options)
		{
			switch (options.ServerType)
			{
				case IBServerType.Default:
				case IBServerType.Embedded:
					return new Client.Native.IBServiceManager(options.ServerType, Charset.GetCharset(options.Charset));
				default:
					throw IncorrectServerTypeException();
			}
		}

		//private static NotSupportedException UnsupportedProtocolException()
		//{
		//	return new NotSupportedException("Protocol not supported.");
		//}

		private static Exception IncorrectServerTypeException()
		{
			return new NotSupportedException("Specified server type is not correct.");
		}

	}
}
