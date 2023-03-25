# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs IB or Fes.  This was done so that both the IB driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Diagnostics.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Diagnostics.Internal
*		 Use the IB named versions of classes and variable names where appropriate moved from IB to ib prefixes.
		
*	  All files renamed from IBXxxxx to IBXxxxx with the internal class matching that same change.  
*	    For example IBBackup.cs becomes IBBackup.cs and the class IBBackup is now IBBackup
