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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using InterBaseSql.Data.Client.Native;

namespace InterBaseSql.Data.Common
{
	internal interface IDatabase
	{
		Action<IscException> WarningMessage { get; set; }

		int Handle { get; }
		int TransactionCount { get; set; }
		string ServerVersion { get; }
		Charset Charset { get; set; }
		short PacketSize { get; set; }
		short Dialect { get; set; }
		bool HasRemoteEventSupport { get; }
		bool ConnectionBroken { get; }
		bool TruncateChar { get; set; }
		public IIBClient IBClient { get; }

		void Attach(DatabaseParameterBuffer dpb, string database);
		void Detach();

		void CreateDatabase(DatabaseParameterBuffer dpb, string database);
		void DropDatabase();

		TransactionBase BeginTransaction(TransactionParameterBuffer tpb);

		StatementBase CreateStatement();
		StatementBase CreateStatement(TransactionBase transaction);

		List<object> GetDatabaseInfo(byte[] items);
		List<object> GetDatabaseInfo(byte[] items, int bufferLength);

		void CloseEventManager();
		void QueueEvents(RemoteEvent events);
		void CancelEvents(RemoteEvent events);

		void CancelOperation(int kind);
	}
}
