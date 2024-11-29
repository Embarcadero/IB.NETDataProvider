# Changes for 10.0.1

## IBAnnotationNames.cs, IBValueGenerationStrategy.cs
** Added support for HiLo

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Metadata.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

*    FbAnnotationNames.cs renamed to IBAnnotationNames.cs
*    FbAnnotationNames now IBAnnotationNames
*		 Prefix changed from "Fb: " to "IB: "
