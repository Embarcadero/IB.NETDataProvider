# Changes for 7.13.6
* General updates from the Fb 9.x code for EFCore 6.0 support

# Changes for 7.12.1

## IBValueGenerationStrategyConvention.cs
* ProcessModelInitialized no longer initialized HasValueGenerationStrategy (this probably should be changed to the trigger value in next update)

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Metadata.Conventions to InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Conventions
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup
			
*		using FirebirdSql.EntityFrameworkCore.Firebird.Metadata.Internal now InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal where used

##  FbConventionSetBuilder.cs renamed to IBConventionSetBuilder.cs
*		  FbConventionSetBuilder now IBConventionSetBuilder
*			Build's creation of the new ServiceCollection uses AddEntityFrameworkInterBase and UseInterBase
			
##	FbStoreGenerationConvention.cs renamed to IBStoreGenerationConvention.cs
*		  FbStoreGenerationConvention now IBStoreGenerationConvention
			
			
			
