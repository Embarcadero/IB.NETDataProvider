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

namespace InterBaseSql.Data.Logging;

public static class IBLogManager
{
	public static IIBLoggingProvider Provider
	{
		get
		{
			_providerRetrieved = true;
			return _provider;
		}
		set
		{
			if (_providerRetrieved)
				throw new InvalidOperationException("The logging provider must be set before any action is taken");

			_provider = value ?? throw new ArgumentNullException(nameof(value));
		}
	}

	public static bool IsParameterLoggingEnabled { get; set; }

	static IIBLoggingProvider _provider;
	static bool _providerRetrieved;

	static IBLogManager()
	{
		_provider = new NullLoggingProvider();
	}

	internal static IIBLogger CreateLogger(string name) => Provider.CreateLogger("InterBaseClient." + name);
}
