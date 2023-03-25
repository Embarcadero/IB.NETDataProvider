# Changes for 7.13.6

## IBModelBuilderExtensions.cs
*  Removed IdentityColumn as IB does not support Identity columns

## IBModelExtensions.cs
* added 2 more GetValueGenerationStrategys

## IBPropertyExtensions.cs
* Fixed up procedure signature changes to EFCore 6.0
* additional IdentityColumn removal stuff

## IBServiceCollectionExtensions.cs
* added AddInterBase function
* UpdatedAddEntityFrameworkInterBase for EFCore 6.0

# Changes for 7.12.1
* For now IBValueGenerationStrategy.IdentityColumn remains, but is not a language feature supported by InterBase

## IBModelBuilderExtensions.cs, IBPropertyBuilderExtensions.cs, IBPropertyExtensions.cs
* Removed UseIdentityColumns as InterBase does not support this 

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Extensions to InterBaseSql.EntityFrameworkCore.InterBase.Extensions
*    Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

*    using FirebirdSql.EntityFrameworkCore.Firebird.Infrastructure.Internal, FirebirdSql.EntityFrameworkCore.Firebird.Metadata now InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal,
*          InterBaseSql.EntityFrameworkCore.InterBase.Metadata
		
##	FbDatabaseFacadeExtensions.cs	renamed to IBDatabaseFacadeExtensions.cs
*	  IsFirebird now IsInterBase

##  FbDbContextOptionsBuilderExtensions.cs renamed to IBDbContextOptionsBuilderExtensions.cs
*	  UseFirebird now UseInterBase
		
##  FbServiceCollectionExtensions.cs renamed to IBServiceCollectionExtensions.cs
*	  All using FirebirdSQLXxx renamed to using interBaseSQL.Xxxx
*		fbServiceCollectionExtensions now IBServiceCollectionExtensions
*		  AddEntityFrameworkFirebird now AddEntityFrameworkInterBase
			
	
