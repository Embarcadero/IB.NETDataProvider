# Changes for 7.10.2 

##  General - 
**	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common 
**    Namespaces changed from FirebirdSql.EntityFrameworkCore.Firebird.Tests.Migrations to InterBaseSql.EntityFrameworkCore.InterBase.Tests.Migrations
**		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
**	  All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
**	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup
			
**		using FirebirdSql.Data.FirebirdClient now using InterBaseSql.Data.InterBaseClient
		
##	MigrationsTests.cs
**	  CreateTable
***      Removed the _UTF8 qualifier		
***      Fixed the check to count \r\n equal to \n in extraction
***			Checked for Create GENERATOR
			
**	  AlterColumnAddSequenceTrigger
***			Checked for Create GENERATOR
	
**	  CreateSequence, AlterSequence and RestartSequence
***		  Switched to IB Syntax
			
		
	
	
