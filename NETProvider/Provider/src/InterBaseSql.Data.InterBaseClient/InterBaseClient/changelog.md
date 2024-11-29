# Changes for 10.0.1

## ClientFactory.cs
** CreateIDatbaase now CreateDatabase returning DatabaseBase
** CreateIServiceManager now CreateServiceManage returning ServiceManagerBase
** UnsupportedProtocolException removed and replaced with more accurate IncorrectServerTypeException

## IBCommand
** Now descends/implements DbCommand, IIBPreparedCommand, IDescriptorFiller, ICloneable
** CommandTimeout now shows in the behavior category and defaults to 30
** DisposeAsync, PrepareAsync added for > NET48
** Added ExecuteNonQueryAsync, ExecuteReaderAsync, ExecuteScalarAsync, GetCommandPlanAsync, ExecuteDbDataReaderAsync, DisposeReaderAsync, FetchAsync, SetOutputParametersAsync (2),
**       CommitImplicitTransactionAsync, RollbackImplicitTransactionAsync, CloseAsync, ReleaseAsync, TransactionCompletedAsync
** Added UpdateParameterValuesAsync
** BuildNamedParametersDescriptor, BuildPlaceHoldersDescriptor, BuildParameterDescriptor, ValidateInputParameters
** Removed ParameterCastFixup
** Added PrepareAsync, ExecuteCommandAsync
** ParseNamedParameters, LogCommandExecutionIfEnabled removed

## IBCommandBuilder.cs
** Set the QuotePrefix correctly for Dialect 1 in constructor

## IBConnection.cs
** Added Logging support
** CreateDatabaseImpl removed
** Added CreateDatabaseAsync, DropDatabaseAsync
** TruncateChar now published to property editor
** >NET48 Added DisposeAsync
** Added BeginTransactionAsync, GetSchemaAsync, ChangeDatabaseAsync, OpenAsync, CloseAsync
** Added EnlistTransaction
** Added CreateBatch for NET60+

## IBConnectionInternal.cs
** Added CreateDatabaseAsync, DropDatabaseAsync, ConnectAsync, DisconnectAsync, BeginTransactionAsync, DisposeTransactionAsync, TransactionCompletedAsync, 
**       GetSchemaAsync, ReleasePreparedCommandsAsync, CloseEventManagerAsync
** AddPreparedCommand, RemovePreparedCommand signature now needs a IIBPreparedCommand instead of an IBCommand
** EnsureActiveTransaction now EnsureNoActiveTransaction
** Removed GetProcessName, GetSystemWebHostingPath, GetRealProcessName, GetClientVersion

## IBConnectionPoolManager.cs
** CleanupPool now called PrunePool

## IBDatabaseInfo.cs
** All methods got an Async version ie IscVersion is paired with IscVersionAsync

## IBDataReader.cs
** The following now hae an ASybc version - Close, Dispose, Read, GetSchemaTable, GetFieldValue, NextResult
** GetFieldValue added support for Date and Time only fields
** All the Get<type> (ie GetDouble) now call the appropriate GetFieldValue<type>

## IBParameter.cs
** SetIBDbType now calls TypeHelper.GetIBDataTypeFromType get retrieve the type

## IBRemoteEvent.cs
** The following got Async versions added - Open, Dispose, QueueEvents, CancelEvents

## IIBPreparedCommand.cs (new)
** Interface for prepared IBCommands

## IBTransaction.cs
** ReadOnly removed
** SupportsSavepoints added for NET50+
** DisposeAsync added for NET50+
** Logging added
** The following got Async versions added - Commit, Rollback (2), Save, Release, CommitRetaining, RollbackRetaining, BeginTransaction, CompleteTransaction

# Changes for 7.14.6

## IBCharset.cs
* Updated the enum to reflect the correct values for IB.

## IBCommand.cs
* Updated passing the Database to the Descriptor

## IBConnection
* Corrected the exception message when trying to change TruncateChar with an active connection (old message was reverse of what it should be)
* GetSchema now supports the new IBDBXLegacyTypes.IncludeLegacySchemaType switch

## IBDataReader.cs
* GetSchemaTable now correctly sets the table and column name parameters to 68 not 31
* GetSchemaTable Fixed when in dialect 1 the datatype is a Double.
* GetSchemaTable added support for IBDBXLegacyTypes.IncludeLegacySchemaType
* GetLegacyProviderType added for legacy support
* GetSchemaTableStructure added legacy support

## IBDBXLegacyTypes.cs (new)
* Support routines when in schema legacy mode.

## IBConnectionStringbuilder
* Added WriteToXML and ReadFromXML to support stremaing a connection string in from an XML file.

#Changes for 7.13.6

## IBCommand.cs
^ added code for EFCore when it puts a parameter in the where clause ir (cast ? as VarChar(20))  iB cannot handle this.  
  When that happens and an excetion 335544569 is raised parse te]he SQL and try to replace the ? with the actual string.

## IBConnection.cs
^ added support for downgrading the dialect when attempting 3 but the DB is actually 1.  
^ Added DialectDowngradeWarning event that fires when the Dialect is downgraded.	
^ Added new read only property DBSQLDialect to get teh actual Dailect of the DB
^ Added private short GetDBSQLDialect for getting the DB Dialect
^ added void ValidateClientSQLDialect() that checks the requested dialect vs the dialect of the DB.  
  If the DB dialect is less set the dialect of hte connectino to it and fire the downgrade event.	

## IBConnectionInternal.cs
^ Only add the character set if not 'none'.

## IBDatabaseInfo.cs
^ Added support for SQLDialect property

# Changes for 7.12.1

## IBConnection.cs
* Support to get at the IDatabase to the inner connection

## IBConnectionInternal.cs
* Fixed a comment still referring to Firebird to InterBase

## IBEvents.cs
* New public class for handling InterBase Events

## IBEventThread.cs
* New internal class for background threading of InterBase Events

## IBTransaction.cs
* IFDEF to override Save method for save points when compiled for NET50 or higher
* IFDEF to override Rollback method for save points when compiled for NET50 or higher

# Changes for 7.11.0
***Change View support added***

## IBCommand.cs
* Added OnFetch internal event handler to copy the ChangeState to the new IBRow class

## IBConnection.cs
* New public function TruncateChar.  Set or returns the connection's TruncateChar option.  Can only be set with an open connection.

## IBConnectionInternal.cs
* Connect now transfers the TruncateChar option to the underlying DB.

## IBConnectionStringBuilder.cs
* TruncateChar property added

## IBDataAdapter
* General internal support for change views
	* CreateStatusTable (new) creates the table used to store the change status for each row/field
	* FillchangeStates (new) Copies the change state for the newly read row into the internal change status table
	* FetchRow (new) event callback when the IBCommand fetches a new row to get the change status values.
	### Fill commands that include change states.  They mirror the normal fills	but also stores change states
* FillWithChangeState(dataSet) (new) Creates a new IBDataTable and fills it
* FillWithChangeState(dataSet, srcTable) (new) fills the named table in the dataSet with values and change states.  Named table must be of type IBDataTable.
* FillWithChangeState(dataTable) (new) fills with the passed dataTable (must be IBDataTable type).
* FillWithChangeState(dataSet, startRecord, maxRecords, srcTable) (new) srcTable must be a IBDataTable in the dataSet
* FillwithChangeState(dataTable, startRecord, maxRecords) (new) Fills with a starting point and max records.  

## IBDataReader.cs
* GetChangeState(int) (new) returns the change state for the field by field number
* GetChangeState(string) (new) returns the change state for hte field by field name.

## IBDataRow.cs (new)
* new row class that has additional storage for ChangeState values.
* Newly created IBDataRows will have all field's IBChangeState set to csUnknown
* ChangeState(int) returns the IBChangeState for the field by ordinal position
* ChangeState(string) returns the IBChangeState for the field by field name
* ChangeState(DataColumn)returns the IBChangeState for the passed DataColumn

## IBDataTable.cs (new)
* New DataTable class that holds IBDataRows to support the extra change view information
* this[idx] returns the indexed IBDataRow
* Add(IBDataRow) adds a new row to the dataSet.
* Remove(row) removes the passed in row
* GetNewRow Creates a new IBDataRow and sets the change state for each field to csUnknown
* ChangeStateColumns property.  When set true a psuedo column is created for each column named "<field name> ChangeState" and fills it with the change state for that column.  When set false it removes these columns

## IBTransaction.cs
* property ReadOnly (new) when set true the transaction will be a read only transaction when started.  Default is false.

# Changes for 7.10.2 

## General - 
* All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.FirebirdClient to InterBaseSql.Data.InterBaseClient
* Uses InterBaseSql.Data.Common instead of FirebirdSql.Data.Common
* Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.

## FbWireCrypt.cs removed
	
## FbCharset.cs renamed IBCharset.cs
* Utf8 constant switched to 59
	 	
## FbCommand.cs renamed to IBCommand.cs
*	CommandExplainedPlan removed
	
## FbCommandBuilder.cs renamed to IBCommandBuilder.cs	
	
## FbConnection.cs renamed to IBConnection.cs
* CreateDatabaseImpl builds the DPB in code adding some newer isc_dpb codes but not all
*	Close - removed cancel logic (Fb specific item)
		
## FbConnectionInternal.cs renamed to IBConnectionInternal.cs
* CreateDatabase removed the Fb Encryption specific code
* DropDatabase removed the Fb Encryption specific code
*	Connect removed the Fb Encryption specific code
*	BuildDpb removed Fb specific items add isc_dpb_password support
*	Removed EnableCancel, DisableCancel and CancelCommand
*	Added EnsureActiveTransaction
		
## FbConnectionPoolManager.cs renamed to IBConnectionPoolManager.cs	
		
## FbConnectionStringBuilder.cs renamed to IBConnectionStringBuilder.cs	
*	"Firebird" string replaced by "InterBase"
*	Removed ClientLibrary property
*	Added EUAEnabled property
*	Added InstanceName property
*	Added SEPPassword property
*	Added SSL property
*	Added ServerPublicFile property
*	Added ServerPublicPath property
*	Added ClientCertFile property
*	Added ClientPassPhraseFile property
*	Added ClientPassPhrase property
*	Removed NoDatabaseTriggers, NoGarbageCollect, Compression, CryptKey and WireCrypt properties
*	Removed GetWireCrypt
		
## FbDataAdapter.cs renamed to IBDataAdapter.cs
*	FbDataAdapter now IBDataAdapter
	
## FbDatabaseInfo.cs renamed to IBDatabaseInfo.cs
*	Removed ServerVersion, ServerClass, OldestTransaction, OldestActiveTransaction, OldestActiveSnapshot, NextTransaction and ActiveTransactions
*	ForcedWrites now returns the string representation for the IBWriteMode
		
## FbDataReader.cs renamed to IBDataReader.cs
*	FbDataReader now IBDataReader
	
## FbDbType.cs renamed to IBDbType.cs
*	FbDbType renamed to IBDbType
*	TimeStampTZ, TimeStampTZEx,	TimeTZ,	TimeTZEx,	Dec16, Dec34, Int128 removed
			
## FbEnlistmentNotification.cs renamed to IBEnlistmentNotification.cs	
* FbEnlistmentNotification now IBEnlistmentNotification	
	
## FbError.cs renamed to IBError.cs
*	FbError now IBError
	
## FbErrorCollection.cs renamed to IBErrorCollection.cs
*	FbErrorCollection now IBErrorCollection
		
## FbException.cs renamed to IBException.cs	
*	FbException now IBException
		
## FbInfoMessageEventArgs.cs renamed to IBInfoMessageEventArgs.cs
*	FbInfoMessageEventArgs now IBInfoMessageEventArgs	
		
## FbParameter.cs renamed to IBParameter.cs	
*	FbParameter now IBParameter
		
## FbParameterCollection.cs renamed to IBParameterCollection.cs
*	FbParameterCollection now IBParameterCollection
		
## FbRemoteEvent.cs	renamed to IBRemoteEvent.cs
*	FbRemoteEvent now IBRemoteEvent
		
## FbRemoteEventCountsEventArgs.cs	renamed to IBRemoteEventCountsEventArgs.cs
*	FbRemoteEventCountsEventArgs now IBRemoteEventCountsEventArgs
	
## FbRemoteEventErrorEventArgs.cs renamed to IBRemoteEventErrorEventArgs.cs
*	FbRemoteEventErrorEventArgs now IBRemoteEventErrorEventArgs
		
## FbRowUpdatedEventArgs.cs renamed to IBRowUpdatedEventArgs.cs
*	FbRowUpdatedEventArgs now IBRowUpdatedEventArgs
		
## FbRowUpdatingEventArgs.cs	renamed to IBRowUpdatingEventArgs.cs
*	FbRowUpdatingEventArgs now IBRowUpdatingEventArgs
		
## FbServerType.cs renamed to IBServerType.cs
*	FbServerType now IBServerType
		
## FbTransaction.cs renamed to IBTransaction.cs
*	FbTransaction now IBTransaction
*	BuildTpb
*	Added support for isc_tpb_wait
*	Removed support for isc_tpb_read_consistency
			
## FbTransactionBehavior.cs renamed to IBTransactionBehavior.cs			
*	FbTransactionBehavior now IBTransactionBehavior	
*	Removed ReadConsistency (Fb specific)
		
## FbTransactionOptions.cs renamed to IBTransactionOptions.cs	
*	FbTransactionOptions now IBTransactionOptions
		
## FirebirdClientFactory.cs now InterBaseClientFactory.cs
* FirebirdClientFactory now InterBaseClientFactory
