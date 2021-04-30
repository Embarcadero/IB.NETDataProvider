﻿/*
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
using System.Text;

namespace InterBaseSql.Data.Logging
{
	public class ConsoleLoggerProvider : IIBLoggingProvider
	{
		readonly IBLogLevel _minimumLevel;

		public ConsoleLoggerProvider(IBLogLevel minimumLevel = IBLogLevel.Info)
		{
			_minimumLevel = minimumLevel;
		}

		public IIBLogger CreateLogger(string name) => new ConsoleLogger(_minimumLevel);

		sealed class ConsoleLogger : IIBLogger
		{
			readonly IBLogLevel _minimumLevel;

			public ConsoleLogger(IBLogLevel minimumLevel)
			{
				_minimumLevel = minimumLevel;
			}

			public bool IsEnabled(IBLogLevel level)
			{
				return level >= _minimumLevel;
			}

			public void Log(IBLogLevel level, string msg, Exception exception = null)
			{
				if (!IsEnabled(level))
					return;

				var sb = new StringBuilder();
				sb.Append("[");
				sb.Append(level.ToString().ToUpper());
				sb.Append("]");

				sb.AppendLine(msg);

				if (exception != null)
					sb.AppendLine(exception.ToString());

				Console.Error.Write(sb.ToString());
			}
		}
	}
}
