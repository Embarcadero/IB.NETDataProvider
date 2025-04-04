# Changes for 10.0.3

## IBService.cs
** Added support for Instance names instead of Port

# Changes for 10.0.1

## IBBackup.cs
** Added ExecuteAsync

## IBConfiguration.cs
** Added Async versions for SetSqlDialect, SetSweepInterval, SetPageBuffers, DatabaseShutdown, DatabaseOnline, ActivateShadows, SetForcedWrites, SetReserveSpace,
**                          SetAccessMode, SetWriteMode 

## IBLog.cs
** Added ExecuteAsync

## IBRestore.cs
** Added ExecuteAsync

## IBSecurity.cs
** Added Async versions of AddUser, DeleteUser, ModifyUser, DisplayUser, DisplayUsers, GetUsersDbPath

## IBServerProperties.cs
** Added Async versions of GetVersion, GetServerVersion, GetImplementation, GetImplementation, GetRootDirectory, GetLockManager, GetMessageFile, GetDatabasesInfo,
**                         GetServerConfig, GetString, GetInfo 
** GetInfo now returns a List<object> instead of an IList

## IBService.cs
** Added Async versions of Open, Close, StartTask, Query (2)

## IBStatistical.cs
** Added ExecuteAsync

## IBStatisticalFlags.cs
** RecordVersions now RecordVersionsStatistics in the enum

## IBValidation.cs
** Added ExecuteAsync

## IBValidationFlags.cs
** Enum values now pull from their constants in IscCodes instead of hard-coded same constants

# Changes for 7.10.2 

## General 
*	All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.Services to InterBaseSql.Data.Services
*	Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.	
*	All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
	 * For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

## FbBackup.cs renamed to IBBackup.cs
*	Added Tablespace support
*	Added Encryption support
		
## FbBackupFlags.cs renamed to IBBackupFlags.cs
*	Removed isc_spb_bkp_no_triggers from the enum
		
## Added IBBackupTablespaceCollection.cs	
	
## Removed FbBackupRestoreStatistics.cs
	
## FbConfiguration.cs renamed to IBConfiguration.cs
*	removed DatabaseShutdown2
*	removed DatabaseOnline2
*	removed NoLinger
*	removed FbShutdownOnlineModeToIscCode
*	Added SetWriteMode to support the 3 IB write modes
		
## Removed FbNBackup.cs, FbNBackupFlags.cs	and FbNRestore.cs	- All Fb specific items
	
## FbRestore.cs renamed to FbRestore.cs
*	Added Encryption support
*	Added Tablespace support
*	Added support to restore to an ODS versions
*	Add Writemode support
*	Added Preallocate support
*	Added Starting transaction Id support
		
## Added IBRestoreExcludeTablespaceInfoCollection.cs, IBRestoreIncludeTablespaceInfo.cs, IBRestoreType.cs and IBRestoreIncludeTablespaceInfoCollection.cs for Tablespace support in restores
	
##	FbRestoreFlags.cs renamed to IBRestoreFlags.cs
* Added ValidationCheck, ReplaceTablespace and CreateTablespace to the enum
		
## FbServerConfig.cs	renamed to IBServerConfig.cs
* Added properties for
	 * TracePools 
	 * RemoteBuffer 
	 * CPUAffinity 
	 * SweepQuantum 
	 * UserQuantum 
	 * SleepTime 
	 * MaxThreads 
	 * AdminDB 
	 * UseSanctuary 
	 * EnableHT 
	 * UseRouter 
	 * SortMemBufferSize 
	 * SQLCmpRecursion 
	 * SQLBoundThreads 
	 * SQLSyncScope 
	 * IdxRecnumMarker 
	 * IdxGarbageCollection 
	 * WinLocalConnectRetries 
	 * ExpandMountpoint 
	 * LoopbackConnection 
	 * ThreadStackSize 
	 * MaxDBVirmemUse 
	 * MaxAssistants 
	 * AppdataDir 
	 * MemoryReclamation 
	 * PageCacheExpansion 
	 * StartingTransactionID 
	 * DatabaseODSVersion 
	 * HostlicImportDir 
	 * HostlicInfoDir 
	 * EnablePartialIndexSelectivity 
	 * PredictiveIOPages 
			
##	FbService.cs renamed to IBService.cs
*	 BuildSpb removed managed code references
*	Removed Crypto references for service attachments
*	ProcessQuery removed isc_info_svc_stdin section
*	ParseServerConfig added suuport for 
	* ISCCFG_TRACE_POOLS_KEY
	*	ISCCFG_REMOTE_BUFFER_KEY
	*	ISCCFG_CPU_AFFINITY_KEY
	*	ISCCFG_SWEEP_QUANTUM_KEY
	*	ISCCFG_USER_QUANTUM_KEY
	*	ISCCFG_SLEEP_TIME_KEY
	*	ISCCFG_MAX_THREADS_KEY
	* ISCCFG_ADMIN_DB_KEY
	*	ISCCFG_USE_SANCTUARY_KEY
	*	ISCCFG_ENABLE_HT_KEY
	*	ISCCFG_USE_ROUTER_KEY
	*	ISCCFG_SORTMEM_BUFFER_SIZE_KEY
	*	ISCCFG_SQL_CMP_RECURSION_KEY
	*	ISCCFG_SOL_BOUND_THREADS_KEY
	*	ISCCFG_SOL_SYNC_SCOPE_KEY
	*	ISCCFG_IDX_RECNUM_MARKER_KEY
	*	ISCCFG_IDX_GARBAGE_COLLECTION_KEY
	*	ISCCFG_WIN_LOCAL_CONNECT_RETRIES_KEY
	*	ISCCFG_EXPAND_MOUNTPOINT_KEY
	*	ISCCFG_LOOPBACK_CONNECTION_KEY
	*	ISCCFG_THREAD_STACK_SIZE_KEY
	*	ISCCFG_MAX_DB_VIRMEM_USE_KEY
	*	ISCCFG_MAX_ASSISTANTS_KEY
	*	ISCCFG_APPDATA_DIR_KEY
	*	ISCCFG_MEMORY_RECLAMATION_KEY
	*	ISCCFG_PAGE_CACHE_EXPANSION_KEY
	*	ISCCFG_STARTING_TRANSACTION_ID_KEY
	*	ISCCFG_DATABASE_ODS_VERSION_KEY
	*	ISCCFG_HOSTLIC_IMPORT_DIR_KEY
	*	ISCCFG_HOSTLIC_INFO_DIR_KEY
	*	ISCCFG_ENABLE_PARTIAL_INDEX_SELECTIVITY_KEY
	*	ISCCFG_PREDICTIVE_IO_PAGES_KEY
			
## Removed FbServiceTraceConfiguration.cs and FbServiceTraceEvents.cs	
	
## Removed FbShutdownOnlineMode.cs

## FbStatisticalFlags.cs renamed to IBStatisticalFlags.cs
* Added RecordVersions and StatTables to the flags enum
		
## Removed FbStreamingBackup.cs andFbStreamingRestore.cs

## Removed FbTrace.cs, fbTraceConfiguration.cs and FbTraceVersion.cs	
	
## Removed FbValidation2.cs
	
## FbValidationFlags.cs renamed to IBValidationFlags
* Added LimboTransactions to the enum
		
## Added IBWriteMode.cs for the 3 different write modes in InterBaseSql
	
	
	
	
		
		
	
	
