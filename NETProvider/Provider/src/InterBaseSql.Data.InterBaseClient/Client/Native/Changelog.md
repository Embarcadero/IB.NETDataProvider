# Changes for 7.13.6

## IBArray.cs
^ Added support for sql_double

# Changes for 7.12.1

## IBServiceManager.cs
* Added support to connect to remote servers

## IIBClient.cs
* Added delegate for the cdecl callback for events
* Fixed the signatures around events 

## LinuxClient.cs and WindowsClient.cs
* Fixed the Event signatures to match IIBClient.cs

# Changes for 7.11.0

***Change View support added***

## XsqldaMarshaler.cs
* Fixed null flag check to check >= 0 instead of != -1

## IBDatabase.cs
* Added support for the new truncate char feature.  When true Char fields are right truncated

# Changes for 7.10.2 

## General - 
	
* To support the posix versions of the native IB client DLLs the whole injection scheme was removed due to not being able to support cdecl calling convention (and there are 3 or 4 windows dll's that use cdecl around events that their native driver suppport did not support).  The current code always injected stdcall as the calling convention and .NET 4.52 does not support the cdecl calling convention.  At this time it was decided to not support a managed interface to IB and only go through the supplied native clients.
* All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.Client.Native to InterBaseSql.Data.Client.Native
* Uses InterBaseSql.Data.Common instead of FirebirdSql.Data.Common
* Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
## FbClientFactory.cs renamed to IBClientFactory.cs 
- IBClientFactory total rewrite to support cdecl calling convention on POSIX platforms.
- Added Register<T>(string id) to register each of the IIBClient class implementors
- GetGDSLibrary returns a shared instance to the explicit IIClient for the running platform with the passed Server type.  Naming convention is expected to be <plaform><servertype>  So windows default class name would be WindowsClient.  Linux Embedded would be LinuxEmbedded.  In each case you would pass Client or Embedded as ServerType and the routine figures out the platform. 
		
## FesArray.cs renamed to IBArray.cs	
* FesArray class renamed to IBArray
* General change of Firebird class and variable names to the IB Equivalent		
* GetSlice - Supports Array descriptor V2 when connected to a client version >= 7.0 and calls isc_array_get_slice2 in that case
* PutSlice - Supports Array descriptor V2 when connected to a client version >= 7.0 and calls isc_array_put_slice2 in that case
	
## FesBlob.cs renamed to IBBlob.cs	
* FesBlob class renamed to IBBlob
* General fixup of Firebird class and variable names to the IB Equivalent		
		  
## FesConnection.cs renamed to IBConnection.cs
* FesConnection class renamed to IBConnection
* General fixup of Firebird class and variable names to the IB Equivalent
* ParseStatusVector removed IscCodes.isc_arg_sql_state section	

## FesDatabase.cs renamed to IBDatabase.cs		
* FesDatabase class renamed to IBDatabase (general note - Over the Wire encryption is done differently between Fb and IB.  With IB it is part of the connection string)
* CreateDatabaseWithTrustedAuth removed
* CreateDatabase removed crypto key support
* Attach removed crypto key suppport
* AttachWithTrustedAuth removed
* CancelOperation basically is no op'ed until the Async code is updated. (fb version worked with their managed code)
* DatabaseParameterBufferBase basically removed and just DatabaseParameterBuffer used where approperiate
* GetServerVersion uses isc_info_version in the call to GetDatabaseinfo
* CheckCryptKeyForSupport removed
			
## FesServiceManager.cs renamed to IBServiceManager.cs (general note - Over the Wire encryption is done differently between Fb and IB.  With IB it is part of the connection string)
* FesServiceManager class renamed to IBServiceManager
* IBServiceManager constructor expects a IBServerType as the first parameter to determine default or embedded client
* Attach removed crypto support	

## FesStatement.cs renamed to IBStatement.cs	
* FesStatement class renamed to IBStatement	
* Execute - Call to isc_dsql_execute2 now passes the current SQLDAVersion
* Fetch - Call to isc_dsql_fetch now passes the current SQLDAVersion
* DescribeParameters - passes IscCodes.SQLDA_CURRENT_VERSION to isc_dsql_describe_bind
* Allocate now calls isc_dsql_alloc_statement2 instead of isc_dsql_alloc_statement
			
## FesTransaction.cs renamed to IBTransaction.cs
* FesTransaction class renamed to IBTransaction
* Fixed up some constant strings to reference InterBase instead of Firebird.
* BeginTransaction - fixed a pointer size bug in the Fb code.  Original code assumes pointers are always 4 bytes.
* Added StartSavepoint
* Added RollbackSavepoint
* Added ReleaseSavepoint
		
## IfbClient.cs renamed to IIBClient.cs
* IFbClient interface renamed to IIBClient
* added string LibraryName();
* added void LoadIBLibrary();
* added void FreeIBLibrary();
* added bool TryIBLoad();
* added void CheckIBLoaded();
* added string Platform { get; }
* added string ServerType();
* added decimal IBClientVersion { get; }
* added short SQLDAVersion { get; }
* removed fb_shutdown
* removed fb_cancel_operation
*	removed isc_dsql_allocate_statement
*	added isc_array_gen_sdl
*	added isc_array_gen_sdl2
*	added isc_array_get_slice2       
*	added isc_array_lookup_bounds
*	added isc_array_lookup_bounds2
*	added isc_array_lookup_desc
*	added isc_array_lookup_desc2
*	added isc_array_set_desc
*	added isc_array_set_desc2
*	added isc_array_put_slice2
*	added isc_blob_default_desc
*	added isc_blob_default_desc2
*	added isc_blob_gen_bpb
*	added isc_blob_gen_bpb2
*	added isc_blob_info
*	added isc_blob_lookup_desc
*	added isc_blob_lookup_desc2
*	added isc_blob_set_desc
*	added isc_blob_set_desc2
*	added isc_cancel_events
*	added isc_decode_date
*	added isc_decode_sql_date
*	added isc_decode_sql_time
*	added isc_decode_timestamp
*	added isc_dsql_alloc_statement2			
*	added isc_dsql_exec_immed2
*	added isc_dsql_batch_execute_immed
*	added isc_dsql_batch_execute
*	added isc_dsql_execute_immediate
*	added isc_dsql_set_cursor_name
*	added isc_encode_date
*	added isc_encode_sql_date
*	added isc_encode_sql_time
*	added isc_encode_timestamp
*	added isc_event_block
*	added isc_event_counts
*	added isc_free
*	added isc_get_segment
*	added isc_interprete
*	added isc_prepare_transaction
*	added isc_prepare_transaction2
*	added isc_que_events
*	added isc_release_savepoint
*	added isc_rollback_savepoint
*	added isc_start_savepoint
*	added isc_start_transaction
*	added isc_sqlcode
*	added isc_sql_interprete
*	added isc_transaction_info
*	added isc_vax_integer
*	added isc_portable_integer
*	added isc_get_client_version
*	added isc_get_client_major_version
*	added isc_get_client_minor_version
			
## LinuxClient.cs - new
* class LinuxClient implements IIBClient for Linux.  Since this is a POSIX system all calling conventions in the DLL are cdecl.
		
## LinuxEmbedded.cs - new
* Inherits from LinuxClient
* Overrides the LibraryName method to return the embedded client .so name
*	Overrides the ServerType method to return Embedded.
		
## MacOSClient.cs - new
* Inherits from LinuxClient
* Overrides the LibraryName method to return the embedded client .dynlib name
*	Overrides Platform to return MacOS
*	Overrides GetProcAddress and LoadLibrary to use the MacOS libdl.dynlib versions
		
## MacOSEmbedded.cs - new
* Inherits from MacOSClient
* Overrides the LibraryName method to return the embedded client .dynlib name
* Overrides the ServerType method to return Embedded.
	
## WindowsClient.cs - new
* class WindowsClient implements IIBClient for Windows.  Almost all calls are stdcall callign conventions, but a few are cdecl.
		
## WindowsEmbedded.cs - new
* Inherits from WindowsClient
* Overrides the LibraryName method to return the embedded client .dll name based on 32/64 bit environment
*	Overrides the ServerType method to return Embedded.
	
	
