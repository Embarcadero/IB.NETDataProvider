# Changes for 10.0.1

## IBMigrationSqlGeneratorBehavior.cs
** CreateSequenceTriggerForColumn
***    Now creates a SP to build the trigger named IBEFC$GEN.  This checks for existence before creating, IBEFC$GEN is dropped after executed
***    Fixed a Syntax error in the Trigger creation
** DropSequenceTriggerForColumn
***    Now creates an SP named IBEFC$GENDROP that checks for the existence of the trigger before dropping.  IBEFC$GENDROP is dropped when done.

# IBMigrationsSqlGenerator.cs
** Generate (alter op) now directly changes the NULL flag when switching from NULL to NOT NULL or vice versa
** Generate (alter op) can now add a default.  It does this by directly manipulating the system tables.  Creates a temp column with same type and the new default, then copies the default information to the column to be changed and drops the new column.
** Generate (create Index) now supports desc indexes
** Generate (Restart Sequence Op) issues a Set Generator <> to <> command
** ColumnDefinition throws an exception when trying to create an Identity column
** Generate - new overrids for Insert, Update and delete Ops


# Changes to 7.13.6

## Removed Operations folder

## IBMigrationSqlGeneratorBehavior.cs
* Fixed code around IB not supporting returning values on inserts and deletes

## IBMigrationsSqlGenerator.cs
* Fixed bug around changing an column's nullability
* added COLLATE meta generation when a collate is indicated

# Changes 7.12.1

## IBMigrationsSqlGenerator.cs
* Removed/Commented all code around IBValueGenerationStrategy.IdentityColumn as InterBase does not support

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*   Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Migrations to InterBaseSql.EntityFrameworkCore.InterBase.Migrations
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

##    FbMigrationSqlGeneratorBehavior.cs renamed to IBMigrationSqlGeneratorBehavior.cs
*		  FbMigrationSqlGeneratorBehavior now IBMigrationSqlGeneratorBehavior
*			  CreateSequenceTriggerForColumn no longer uses EXECUTE BLOCK which means lost the ability to check if the generator already exists
*				CreateSequenceTriggerForColumn changes CREATE SEQUENCE to CREATE GENERATOR 
*				DropSequenceTriggerForColumn removed EXECUTE BLOCK which means loss of checking if trigger exists before dropping
				
##		FbMigrationsSqlGenerator.cs renamed to IBMigrationsSqlGenerator.cs
*		  using switched from the Firebird.Xxx to InterBase.Xxx
*			FbMigrationsSqlGenerator now IBMigrationsSqlGenerator
*			  "Firebird" strings replaced with "InterBase"
				
				

