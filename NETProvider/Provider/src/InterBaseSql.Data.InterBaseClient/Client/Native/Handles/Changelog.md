# Changes for 10.0.0
* Handle folder renamed to Handles
* Namespace changed from InterBaseSql.Data.Client.Native.Handle to InterBaseSql.Data.Client.Native.Handles

## BlobHandle.cs, DatabaseHandle.cs, ServiceHandle.cs, StatementHandle.cs, TransactionHandle.cs
* Switched IBConnection.ParseStatusVector to StatusVectorHelper.ParseStatusVector

# Changes for 7.10.2

##  General - 	
*	   All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.
		 
##  Common 
*    Namespaces changed from FirebirdSql.Data.Client.Native.Handle to InterBaseSql.Data.Client.Native.Handle
*	  Uses InterBaseSql.Data.Common instead of FirebirdSql.Data.Common
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.

##  BlobHandle.cs
*	  ReleaseHandle uses the IB named classes
		
##	DatabaseHandle.cs	
*	  DatabaseHandle returns an instance of InterBaseHandle
		
##	IFirebirdHandle.cs renamed to IInterBaseHandle.cs
  
##  IInterBaseHandle.cs
*    SetClient now expects an IIBClient.	
	
##	FirebirdHandle.cs renamed to InterBaseHandle.cs

##  InterBaseHandle.cs
*    Fixed up the class names and variable names to represent the IB versions.	
		
##	StatementHandle.cs
*    StatementHandle returns an InterBaseHandle	
		
##	TransactionHandle.cs
*     TransactionHandle returns an InterBaseHandle	
