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
				
				

