#Changes for 7.13.6 (updated for EFCore 6.0 and to Fb 9.x)

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common 
*    Namespaces changed from EntityFramework.Firebird.FunctionalTests.Helpers to EntityFramework.InterBase.FunctionalTests.Helpers
*		 Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup
			
*		using FirebirdSql.EntityFrameworkCore.Firebird.Metadata now using InterBaseSql.EntityFrameworkCore.InterBase.Metadata
		
##		SkippingAttributes.cs
*		  "Firebird" string replaced with "InterBase"
*		  NotSupportedOnFirebirdFactAttribute now NotSupportedOnInterBaseFactAttribute
*			NotSupportedOnFirebirdTheoryAttribute now NotSupportedOnInterBaseTheoryAttribute
			
