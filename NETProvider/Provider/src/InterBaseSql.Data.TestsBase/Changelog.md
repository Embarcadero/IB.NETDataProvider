#Changes for 10.0.1

## NoServerCategoryAttribute.cs (Added)

## IBEmbeddedServerTypeTestFixtureSource.cs
** now returns IBServerType.Embedded so tests run in embedded mode

# Changes for 7.13.6
^ added setup support to setup a Dialect 1 DB to test against.

# Changes for 7.12.1

## IBTestsBase.cs
* Updated the way random numbers generated in NET60 and up

## IBTestsSetup
* Added a SP for events in the DB install setup

# Changes for 7.11.0

## IBTestsBase.cs
* InsertTestData updates to fill out the table used for change view tests

## IBTestsSetup.cs
* Added for ChangeView test support
    * CreateDomains 
    * CreateSubscriptions 
    * DoGrant
* Database method changes Db location to use AppDomain.CurrentDomain.BaseDirectory
* CreateTables creates an EMPLOYEE table for ChangeVaie tests

# Changes for 7.10.2 

## General - 
* All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

# Common 
* Namespaces changed from FirebirdSql.Data.TestsBase to InterBaseSql.Data.TestsBase
*	Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
* All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
	  *  For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup
*	using FirebirdSql.Data.FirebirdClient now using InterBaseSql.Data.InterBaseClient
		
# FbDefaultServerTypeTestFixtureSource.cs now IBDefaultServerTypeTestFixtureSource.cs
* GetEnumerator only does Default and Embedded
		
#	FbEmbeddedServerTypeTestFixtureSource.cs now IBEmbeddedServerTypeTestFixtureSource.cs	
* GetEnumerator yeilds only IBServerType.Default (this is mainly cause you'd have to setup ToGo in the debug directory to run the tests)
		
#	FbTestsBase.cs now IBTestsBase.cs
* Removed properties Compression and WireCrypt 
* Removed parameters for Compression and WireCrypt from the constructor
* DeleteAllData now uses a SP created when the DB is created instead of execute block
*	BuildConnectionStringBuilder removed parameters for Compression and WireCrypt
		
#	FbTestsSetup.cs now IBTestsSetup.cs	
* Pagesize the default 4096 
* All Recreates just create (Fb feature)
* Added CreateUDF which is a script that inserts the UDF functions.  Called from the Setup routine
*	Database method now places the DB in the assemblies directory
*	CreateTables 
    * Changed BigInt to Numeric(18, 0)
    * Array fields all 1 based
*	CreateProcedures
    * Added an Octets procedure due to issue in IB with parameters in the select portion
    * Added DELETEALLDATA
    * Added TEST_SP for large return column tests		
* CreateTriggers
    *  Removed the connect trigger (fb feature)		
			
			
	
	
	
		
