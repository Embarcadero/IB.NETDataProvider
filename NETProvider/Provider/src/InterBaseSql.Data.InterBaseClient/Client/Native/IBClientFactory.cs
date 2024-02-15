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

//$Authors = Dean Harding, Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InterBaseSql.Data.InterBaseClient;
using InterBaseSql.Data.Client.Native.Handle;
using InterBaseSql.Data.Common;
using System.Runtime.InteropServices;

namespace InterBaseSql.Data.Client.Native
{
	public class IBClientFactory
	{
		static Dictionary<string, Type> _registeredType = new Dictionary<string, Type>();
		static Dictionary<string, IIBClient> _instances = new Dictionary<string, IIBClient>();

		public static IBClientFactory Instance { get; } = new IBClientFactory();

		private IBClientFactory() { }
		private static Assembly currAssembly = Assembly.GetExecutingAssembly();

		public void Register<T>(string id)
		{
			var type = typeof(T);
			if (type.IsAbstract || type.IsInterface)
				throw new ArgumentException("Cannot create instance of interface or abstract class");

			_registeredType.Add(id, type);
		}

		public static IIBClient GetGDSLibrary(IBServerType id)
		{
			Type type;
			string Platform = "Windows";

#if NET5_0_OR_GREATER
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				Platform = "Linux";
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				Platform = "MacOS";
#else
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
					Platform = "Windows";
					break;
				case PlatformID.Unix:
					Platform = "Linux";
					break;
				case PlatformID.MacOSX:
					Platform = "MacOS";
					break;
				default:
					throw new Exception("Unsupported OS");
			}
#endif


			Platform += id.ToString();

			if (_registeredType.Count == 0)
			{
				foreach (Type aType in currAssembly.GetTypes())
				{
					if (!aType.IsClass || aType.IsAbstract ||
						!aType.GetInterfaces().Contains(typeof(IIBClient)))
					{
						continue;
					}

					if (!_registeredType.TryGetValue(aType.Name, out type))
					{
						var aPlatform = (IIBClient)Activator.CreateInstance(aType);
						_registeredType.Add(aPlatform.Platform + aPlatform.ServerType(), aType);
					}
				}
			}
			if (!_instances.TryGetValue(Platform, out IIBClient cl))
			{
				if (!_registeredType.TryGetValue(Platform, out type))
					throw new Exception(id + " not registered");

				cl = (IIBClient)Activator.CreateInstance(type);
			}

			cl.LoadIBLibrary();
			return cl;
		}

	}
}
