#Changes for 7.13.7

## IBSqlGeneratorHelper
*  Added a static variable called Dialect.  It defaults to 3, but when you are working with a Dialcet 1 DB you need to change that static variable to 1.
*  Overrode DelimitIdentifier, DelimitIdentifier and DelimitIdentifier to now use the new Dialect variable to determine if the identifier should be double quoted or not.

#Changes for 7.13.6 (updated for EFCore 6.0 and to Fb 9.x) 

## added IBDateOnlyTypeMapping.cs, IBRelationalTransaction.cs, IBTimeOnlyTypeMapping.cs, IBTransactionFactory.cs, and IRelationalIBTransaction.cs

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Storage.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

##  FbGuidTypeMapping.cs renamed to IBGuidTypeMapping.cs
*	  FbGuidTypeMapping now IBGuidTypeMapping
*	  GenerateNonNullSqlLiteral now calls EF_CHAR_TO_UUID
			
##	FbTypeMappingSource.cs renamed to IBTypeMappingSource.cs
*	  FbTypeMappingSource now IBTypeMappingSource
*	  IBTypeMappingsSource LongTypeMapping now maps to NUMERIC(18, 0)
 
##  NotSupportedInInterBase.cs added
*	  Special exception for unsupported features
		
