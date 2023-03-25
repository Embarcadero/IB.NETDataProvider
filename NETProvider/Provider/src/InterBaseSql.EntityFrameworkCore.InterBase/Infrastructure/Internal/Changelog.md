# Changes for 7.13.6

## IBOptionsExtension.cs
* GetServiceProviderHashCode return type now int

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Design.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Design.Internal
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

*  FbOptionsExtension.cs renamed to IBOptionsExtension.cs
*	 FbOptionsExtension now IBOptionsExtension
*  ApplyServices now calls AddEntityFrameworkInterBase 
