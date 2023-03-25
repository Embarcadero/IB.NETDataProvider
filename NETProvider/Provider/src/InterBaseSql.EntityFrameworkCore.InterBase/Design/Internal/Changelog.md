# Changes for 7.13.6

## IBDesignTimeServices
* Updated for EFCore 6.0 service integration

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs IB or Fes.  This was done so that both the IB driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Design.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Design.Internal
*		Use the IB named versions of classes and variable names where appropriate moved from IB to ib prefixes.
		
*	  All files renamed from IBXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example IBBackup.cs becomes IBBackup.cs and the class IBBackup is now IBBackup

*    using FirebirdSql.EntityFrameworkCore.Firebird.Diagnostics.Internal, FirebirdSql.EntityFrameworkCore.Firebird.Scaffolding.Internal and FirebirdSql.EntityFrameworkCore.Firebird.Storage.Internal
* 		  now InterBaseSql.EntityFrameworkCore.InterBase.Diagnostics.Internal, InterBaseSql.EntityFrameworkCore.InterBase.Scaffolding.Internal and InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;

