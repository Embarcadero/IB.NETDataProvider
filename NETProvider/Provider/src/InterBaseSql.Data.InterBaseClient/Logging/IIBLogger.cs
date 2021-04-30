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

namespace InterBaseSql.Data.Logging
{
	public interface IIBLogger
	{
		bool IsEnabled(IBLogLevel level);
		void Log(IBLogLevel level, string msg, Exception exception = null);
	}

	public static class IIBLoggerExtensions
	{ 
		public static void Trace(this IIBLogger logger, string msg) => logger.Log(IBLogLevel.Trace, msg);
		public static void Debug(this IIBLogger logger, string msg) => logger.Log(IBLogLevel.Debug, msg);
		public static void Info(this IIBLogger logger, string msg) => logger.Log(IBLogLevel.Info, msg);
		public static void Warn(this IIBLogger logger, string msg) => logger.Log(IBLogLevel.Warn, msg);
		public static void Error(this IIBLogger logger, string msg) => logger.Log(IBLogLevel.Error, msg);
		public static void Fatal(this IIBLogger logger, string msg) => logger.Log(IBLogLevel.Fatal, msg);

		public static void Trace(this IIBLogger logger, string msg, Exception ex) => logger.Log(IBLogLevel.Trace, msg, ex);
		public static void Debug(this IIBLogger logger, string msg, Exception ex) => logger.Log(IBLogLevel.Debug, msg, ex);
		public static void Info(this IIBLogger logger, string msg, Exception ex) => logger.Log(IBLogLevel.Info, msg, ex);
		public static void Warn(this IIBLogger logger, string msg, Exception ex) => logger.Log(IBLogLevel.Warn, msg, ex);
		public static void Error(this IIBLogger logger, string msg, Exception ex) => logger.Log(IBLogLevel.Error, msg, ex);
		public static void Fatal(this IIBLogger logger, string msg, Exception ex) => logger.Log(IBLogLevel.Fatal, msg, ex);
	}
}
