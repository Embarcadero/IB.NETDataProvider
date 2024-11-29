# Changes for 10.0.1

## ConsoleLoggingProvider.cs
** ConsoleLoggerProvider now obsolete
** ConsoleLoggingProvider added, but basically hte same as before.

## LogMessages.cs (new)
** Static class for doing logging to a passed in IIBLogger

# Changes for 7.10.2 

## General 
* All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.Logging to InterBaseSql.Data.Logging
*	Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.

## FbLogLevel.cs renamed to IBLogLevel.cs
*	FbLogLevel now IBLogLevel
		
## FbLogManager.cs renamed to IBLogManager.cs	
*	FbLogManager now IBLogManager
*	FirebirdClient string changed to InterBaseClient
		
## IFbLogger.cs renamed to IIBLogger.cs
*	IFbLogger now IIBLogger
		
## IFbLoggingProvider.cs	renamed to IIBLoggingProvider.cs
*	IFbLoggingProvider now IIBLoggingProvider
		
	
		
	
