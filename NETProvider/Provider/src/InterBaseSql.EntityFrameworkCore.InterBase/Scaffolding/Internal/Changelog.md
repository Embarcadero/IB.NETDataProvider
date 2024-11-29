# Changes for 10.0.1

## IBDatabaseModelFactory.cs
** GetStoreType signature change added precision and scale
** GetIndexes handles desc indexes

#Changes for 7.13.6 (updated for EFCore 6.0 and to Fb 9.x)

## removed IBProviderConfigurationCodeGenerator.cs

## added IBProviderCodeGenerator.cs

## IBDatabaseModleFactory
* fixed Fb specific SQL to be IB compliant

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Scaffolding.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Scaffolding.Internal
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

*    using FirebirdSql.Data.FirebirdClient now InterBaseSql.Data.InterBaseClient
		
##	FbProviderConfigurationCodeGenerator.cs now IBProviderConfigurationCodeGenerator.cs
*    FbProviderConfigurationCodeGenerator now IBProviderConfigurationCodeGenerator
*	    MethodCallCodeFragment now uses IBDbContextOptionsBuilderExtensions.UseInterBase in the MethodCallCodeFragment call

