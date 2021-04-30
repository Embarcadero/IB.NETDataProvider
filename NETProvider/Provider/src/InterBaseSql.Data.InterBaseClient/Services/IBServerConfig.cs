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

namespace InterBaseSql.Data.Services
{
	public class IBServerConfig
	{
		public int LockMemSize { get; internal set; }
		public int LockSemCount { get; internal set; }
		public int LockSignal { get; internal set; }
		public int EventMemorySize { get; internal set; }
		public int PrioritySwitchDelay { get; internal set; }
		public int MinMemory { get; internal set; }
		public int MaxMemory { get; internal set; }
		public int LockGrantOrder { get; internal set; }
		public int AnyLockMemory { get; internal set; }
		public int AnyLockSemaphore { get; internal set; }
		public int AnyLockSignal { get; internal set; }
		public int AnyEventMemory { get; internal set; }
		public int LockHashSlots { get; internal set; }
		public int DeadlockTimeout { get; internal set; }
		public int LockRequireSpins { get; internal set; }
		public int ConnectionTimeout { get; internal set; }
		public int DummyPacketInterval { get; internal set; }
		public int IpcMapSize { get; internal set; }
		public int DefaultDbCachePages { get; internal set; }
		public int TracePools { get; internal set; }
		public int RemoteBuffer { get; internal set; }
		public int CPUAffinity { get; internal set; }
		public int SweepQuantum { get; internal set; }
		public int UserQuantum { get; internal set; }
		public int SleepTime { get; internal set; }
		public int MaxThreads { get; internal set; }
		public int AdminDB { get; internal set; }
		public int UseSanctuary { get; internal set; }
		public int EnableHT { get; internal set; }
		public int UseRouter { get; internal set; }
		public int SortMemBufferSize { get; internal set; }
		public int SQLCmpRecursion { get; internal set; }
		public int SQLBoundThreads { get; internal set; }
		public int SQLSyncScope { get; internal set; }
		public int IdxRecnumMarker { get; internal set; }
		public int IdxGarbageCollection { get; internal set; }
		public int WinLocalConnectRetries { get; internal set; }
		public int ExpandMountpoint { get; internal set; }
		public int LoopbackConnection { get; internal set; }
		public int ThreadStackSize { get; internal set; }
		public int MaxDBVirmemUse { get; internal set; }
		public int MaxAssistants { get; internal set; }
		public int AppdataDir { get; internal set; }
		public int MemoryReclamation { get; internal set; }
		public int PageCacheExpansion { get; internal set; }
		public int StartingTransactionID { get; internal set; }
		public int DatabaseODSVersion { get; internal set; }
		public int HostlicImportDir { get; internal set; }
		public int HostlicInfoDir { get; internal set; }
		public int EnablePartialIndexSelectivity { get; internal set; }
		public int PredictiveIOPages { get; internal set; }

		internal IBServerConfig()
		{ }
	}
}
