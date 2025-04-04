#Changes for 10.0.3

# New unit - RemoteConnections.cs
** This unit does true remote connections to another machine instead of TCP/IP loopback (which could fail when the host is stripped from the connection since the DB is in the same location for local connections)
** These tests will fail on other peoples systems if run.  Important that you update the constants at the top of the class to appropriate for your system.  There are a couple hard-coded string which will be cleaned up in the next release at this time.  They too need updating.

# ConnectionStringTests.cs
** Added 2 tests around parsing secure connection strings.

# Changes for 10.0.1
** Almost all tests now have an Async version in addition to their sync version.  This differs from Fb code where all tests were converted to Async.
** All tests run in both normal and embedded modes

## IBExceptionTests.cs (Added)
** Tests the new exception class

## GdsConnectionTests.cs (removed)

## IBArrayTests.cs
** TimeArrayTest, TimeArrayPartialUpdateTest just returns in Dialect 1 (no time type)

## IBCommandTests.cs
** InsertTimeTest now virtual
** DisposeTest removed
** CommandPlanTest now GetCommandPlanTest
** NoCommandPlanTest now GetCommandPlanNoPlanTest
** Added PassesDateTimeWithProperPrecision, PassesTimeSpanWithProperPrecision
** NET60+ added PassDateOnly, PassTimeOnly
** Dialect 1 tests ignore InsertTimeTest, PassesTimeSpanWithProperPrecision

## IBConnectionStringBuilderTests.cs
** Dialect test also now checks the IBConnectionString Dialect property.

## IBConnectionTests.cs
** Added ConnectionPoolingFailedNewConnectionIsNotBlockingPool

## IBDataAdapterTests.cs
** InsertTest excludes time field data for dialect 1
** UpdateBigIntTest, UpdateNumericTest, UpdateDecimalTest adds check for D1 result to be a double, D3 result to be an Int64
** UpdateTimeTest now virtual, Dialect 1 test ignores
** IBDataAdapterTestsDialect1 descended from the wrong class now descends from IBDataAdapterTests (copy n paste error)

## IBDatabaseInfoTests.cs
** DatabaseInfoTest now CompleteDatabaseInfoTest 

## IBParameterTests.cs
** FbDbTypeFromEnumAsValueTest now IBDbTypeFromEnumAsValueTest
** FbDbTypeFromDBNullAsValueTest now IBDbTypeFromDBNullAsValueTest

## IBServicesTests.cs
** Added running in embedded mode

## SrpClientTests.cs (removed)

# Changes for 7.13.6

## All Files
^ Added support to run the tests also in dialect 1.  Old tests only tested against dialect 3.
  This includes changes for type > numeric(9,0) where in D3 it is a scaled int64 and in D1 double precision
	Also include changes around the DATE data tyoe which is a timestamp in D1 and TIMe not being there in D1.

## IBArrayTests.cs
^ BigIntArrayTest made virutal so the dialect 1 version of teh test could be overriden
^ TimeArrayTest made virtual because Dialect 1 does not have a time datatype and can be a test basically not run
^ TimeArrayPartialUpdateTest  made virtual because Dialect 1 does not have a time datatype and can be a test basically not run

## IBConnectionStringBuilderTests.cs
^ Added a dialect test to test the connection properly downgrades from 3 to 1.

## IBConnectionTests.cs
^ added a test around casting to the DATE type and dialect 1
^ added a test DialectScaleback to test the new dialect downgrade event

## IBDatabaseInfoTests.cs
^ added test to retrieve the DB dialect

## IBDataReaderTests.cs
^ added a dialect 1 test that decimals are the right type.


# Changes for 7.12.1

## ChangeViewTests.cs
* Cleaned up a few unused variable warnings

## ConnectionStringTests.cs
* Added ParsingDatabaseHostnamesWithPort to test parsing port out of the Database property
* Added ParsingServerHostnamesWithPort to test the new support for passing the server/port/<dbpath> through the server instead of the Database property

## IBArrayTests.cs and IBBlobTests.cs
* IFDEF for NET60 and greater to switch to a different way to generate random numbers

## IBConnectionStringBuilderTests.cs
* Added CharacterSetDefault to test that no passed CharSet is None, not UTF8

## IBEventTests.cs (new)
* Added test for registering events
* In general it is easier to test events with demos than a unit test unfortunately.

## InterbaseSQL.Data.InterBaseClient.Tests.csproj
* Updated to include NET60 as a target platform

# Changes for 7.11.0

## ChangeViewTests.cs (new)
* ChangeAll test
* ChangeAllStringAccessor test accessing fields change state by name
* ChangeDelete tests finding deletes
* ChangeInsert tests newly inserted records
* ChangeUpdate tests changes to records
* ChangeUpdateCol tests changes to subscriptions on certain columns
* ChangeInsertUpdate tests finding newly inserts or updated rows
* ChangeToNull tests the Null code fix
* NewRowDefautls tests that new rows' change state are all csUnknown
* DataTableAddCustomCol tests that adding a column to a IBDataTable sets it's change state to csUnknown
* DataTableChangeColumns tests the creation of the psuedo columns showing the change states
* ChangeAllDataTable tests seeing all insert, update and deletes in the underlying table
* ChangeAllDataTableStartEnd tests filling with a limited number of rows starting at row 2
* ChangeAllDataSet tests the FillWithChangeSet(dataSet) method
* ChangeAllDataSetSrcName tests the FillWithChangeSet(dataSet, string) method
* ChangeAllDataSetSrcNameStartEnd tests the FillWithChangeSet(dataSet, startRecord, maxRecords, string) method

## ConnectionStringTests.cs
* Added tests for the new truncate_char option

## IBCommandTests
* ExecuteNonQueryOnFetch tests the new FetchEvent

## IBConnectionTests.cs
* ConnectionWithTruncateChar tests the new TruncateChar option

## IBDatabaseSchemaTests.cs
* Fixed count error assert in the Tables test (extra table created for the change view tests)

## IBDataReaderTests.cs
* Added a TruncateCharTest testing hte auto right trim for Char fields

## IBSchemaTests.cs
* Fixed the Tables test to reflect the new table added for ChangeView tests

## IBServicesTests.cs
* Change the code for the location to create the DB based on the AppDomain.CurrentDomain.BaseDirectory value

# Changes for 7.10.2 

## General 
* All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.FirebirdClient.Tests to EntityFramework.InterBase.Tests
* Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.	
* All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup
*	using FirebirdSql.Data.FirebirdClient, FirebirdSql.Data.Common now  InterBaseSql.Data.InterBaseClient, InterBaseSql.Data.Common
*	All test constructors removed compression and WireCrypt from passed params.

		
## ConnectionStringTests.cs
* Added test ParsingEUAEnabledandInstanceNameConnectionStringTest
*	Removed tests CryptKeyWithBase64FullPadding, CryptKeyWithBase64SinglePadding, CryptKeyWithBase64NoPadding and WireCryptMixedCase
*	All temp database extensions changed form .fbd to .ib
	
## FbBooleanSupportTests.cs renamed to IBBooleanSupportTests.cs
* SimpleSelectWithBoolConditionTest removed NULL test and rewrote to not use NOT DISTINCT FROM to still do boolean param testing
		
## FbCommandTests.cs renamed to IBCommandTests.cs	
* Added PrepareTest
* Removed ReturningClauseParameterTest, ReturningClauseScalarTest and ReturningClauseReaderTest 
*	Removed CommandCancellationTest
*	Removed CommandExplainedPlanTest and NoCommandExplainedPlanTest
*	Removed PassesTimeSpanWithProperPrecision and PassesDateTimeWithProperPrecision (IB Propblem with params in the select portion)
		
## FbConnectionStringBuilderTests.cs renamed to IBConnectionStringBuilderTests..cs	
* Removed CryptKeyValueSetter, CryptKeyValueGetter, WireCryptSetter and WireCryptGetter

## FbConnectionTests.cs renamed to IBConnectionTests.cs		
* Removed NoDatabaseTriggersWrongConnectionStringTest, DatabaseTriggersTest, UserIDCorrectlyPassedToServer, UseTrustedAuth, CreateDropDatabaseUsingTrustedAuth, UseCompression, UseWireCrypt, PassCryptKey and CaseSensitiveLogin
	
## FbDataReaderTests.cs renamed to IBDataReaderTests.cs
* Added an extension to flipping the Endian of a Guid (potential bug in 
* GetOrdinalTest tests against rdb$database
*	ReadBinaryTest tests against rdb$database and the x prefix to the string removed (Fb specific syntax)
*	ReadGuidRoundTripTest Added a cast to octets to get aroudn an IB bug of incorrect charset.  Also had to Flip the endian due to the driver hard-coding to little, but native GUID being big (or vice versa)
*	ReadGuidRoundTrip2Test 
		 1. turned the execute block into a create SP then calls the SP and finally drops the SP at the end.
		 2.	comparison done to the ASCII string representation and once again needed to flip the endian of the driver generated Guid (problem in DecodeGuid(byte[]) ?)
* DNET183_VarcharSpacesShouldNotBeTrimmed hardcoded the 'foo ' instead of param to workaround IB limit
		
## FbDecFloat16SupportTests.cs, FbDecFloat32SupportTests.cs, FbDecFloatTypeTests.cs, FbExceptionTests.cs, FbLongNumericsSupportTests.cs and FbDecInt128SupportTests.cs removed as they are Fb specific data types
	
## FbRemoteEventTests.cs renamed to IBRemoteEventTests.cs
* All tests currently commented out due to high use of Fb features in them.  Needs rewriting.
		
## FbSchemaTests.cs renamed to IBSchemaTests.cs
* Tables test increased number expected due to a table being added to the DB creation script.
		
## FbServicesTests.cs renamed to IBServicesTests.cs
* BackupRestoreTest now uses the current assemblies directoryas the location of hte B/R
*	Removed Restore statistics (Fb feature)
*	Removed StreamingBackupRestoreTest, StreamingBackupRestoreTest_BackupPart and StreamingBackupRestoreTest_RestorePart
*	Removed ShutdownOnline2Test
*	AddDeleteUserTest Added some Try blocks to make sure the added user is always deleted
*	Removed NBackupBackupRestoreTest, TraceTest, NoLingerTest, StatisticsWithEncryptedTest, StatisticsWithEncryptedTest
			
## Removed files FbTimeZonesSupportTests.cs, FbZonedDateTimeTypeTests.cs and FbZonedTimeTypeTests.cs	
	
## FbTransactionTests.cs renamed to IBTransactionTests.cs
* ReadCommittedReadConsistency removed
	
## TrackerIssuesTests.cs
* DNET217_ReadingALotOfFields
		 * SP mpoved to the DB creation script to get around not using recreate.
* DNET304_VarcharOctetsParameterRoundtrip
     * Created a SP called Octets to get aroudn IB limitation of parameters in the select portion
* GetSomething
     * Switched monitoring table name to start with tmp$
