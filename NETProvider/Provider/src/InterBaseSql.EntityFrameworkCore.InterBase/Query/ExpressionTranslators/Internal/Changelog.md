# Changes for 10.0.1

## IBDateOnlyMethodTranslator.cs (Added)

## IBByteArrayMethodTranslator.cs, IBMathTranslator.cs, IBNewGuidTranslator.cs, IBStringContainsTranslator.cs, IBStringEndsWithTranslator.cs, IBStringFirstOrDefaultTranslator.cs,
##     IBStringIndexOfTranslator.cs, IBStringIsNullOrWhiteSpaceTranslator.cs, IBStringLastOrDefaultTranslator.cs, IBStringStartsWithTranslator.cs,
##     IBStringSubstringTranslator.cs, IBStringTrimTranslator.cs, IBTimeSpanPartComponentTranslator.cs    
** Fixed function names to be right for the EFCore UDF and code to call it correctly (This comes in the ADO.NET Client msi or in GitHub)

## IBConvertTranslator.cs
** ToBoolean added

# Changes for 7.13.6 (mostly update to EFCore 6.0 and Fb 9.x driver)

## removed IBStartsWithOptimizedTranslator.cs, IBEndsWithOptimizedTranslator.cs, IBDateTimeDatePartComponentTranslator.cs and IBContainsOptimizedTranslator.cs

## Added IBByteArrayMethodTranslator.cs, IBDateOnlyPartComponentnTranslator.cs, IBDateTimePartComponentTranslator.cs, IBStringContainsTranslator.cs, 
##       IBStringEndsWithTranslator.cs, IBStringFirstOrDefaultTranslator.cs, IBStringLastOrDefaultTranslator.cs, IBStringStartsWithTranslator.cs,
##       IBTimeOnlyPartComponentTranslator.cs and IBTimeSpanPartComponentTranslator.cs


# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Query.ExpressionTranslators.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Query.ExpressionTranslators.Internal
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

*   using FirebirdSql.EntityFrameworkCore.Firebird.Query.Internal now InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal
		
##	FbConvertTranslator.cs renamed to IBConvertTranslator.cs
*	  FbConvertTranslator now IBConvertTranslator 
*		internal dictionary now maps int64 to Numeric(18, 0)  

##  FbDateAddTranslator.cs now IBDateAddTranslator.cs
*	  FbDateAddTranslator now IBDateAddTranslator
*	  Translate now uses the UDF EF_DATEADD
		
##	FbEndsWithOptimizedTranslator.cs renamed to IBEndsWithOptimizedTranslator.cs
*	  FbEndsWithOptimizedTranslator now IBEndsWithOptimizedTranslator
*		Translate now used the IDF EF_LENGTH 
		
##	FbMathTranslator.cs renamed to IBMathTranslator.cs
*	  FbMathTranslator now IBMathTranslator
*		The internal Fb function ABS replaced with the UDF EF_ABS
*		The internal Fb function CEILING replaced with EF_CEILING
*		The internal Fb function FLOOR replaced with the UDF EF_FLOOR
*		The internal Fb function POWER replaced with the UDF EF_POWER
*		The internal Fb function EXP replaced with the UDF EF_EXP
			
##	FbNewGuidTranslator.cs renamed to IBNewGuidTranslator.cs
*	  FbNewGuidTranslator now IBNewGuidTranslator
*		Translate now calls the UDF EF_NEWGUID
			
##  FbObjectToStringTranslator.cs renamed to IBObjectToStringTranslator.cs			
*	  FbObjectToStringTranslator now IBObjectToStringTranslator
*		Translate now calls the UDF EF_UUID_TO_CHAR
			
##  FbStartsWithOptimizedTranslator.cs renamed to IBStartsWithOptimizedTranslator.cs  
*	  FbStartsWithOptimizedTranslator now IBStartsWithOptimizedTranslator
*		Translate now calls the UDF EF_LENGTH
		  
##	FbStringIndexOfTranslator.cs renamed to IBStringIndexOfTranslator.cs
*	  FbStringIndexOfTranslator now IBStringIndexOfTranslator
*		Translate now calls the UDF EF_POSITION
			
##	FbStringIsNullOrWhiteSpaceTranslator.cs renamed to IBStringIsNullOrWhiteSpaceTranslator.cs
*	  FbStringIsNullOrWhiteSpaceTranslator now IBStringIsNullOrWhiteSpaceTranslator
*		Translate now calls the UDF EF_TRIM
			
##	FbStringLengthTranslator.cs renamed to IBStringLengthTranslator.cs
*	  FbStringLengthTranslator now IBStringLengthTranslator
*		Translate now calls the UDF EF_LENGTH
			
##	FbStringReplaceTranslator.cs renamed to IBStringReplaceTranslator.cs
*	  FbStringReplaceTranslator now IBStringReplaceTranslator
*		  Translate now calls the UDF EF_REPLACE
			
##	FbStringToLowerTranslator.cs renamed to IBStringToLowerTranslator.cs
*	  FbStringToLowerTranslator now IBStringToLowerTranslator
*		Translate now calls the UDF EF_LOWER
			
##	FbStringToLowerTranslator.cs renamed to IBStringToLowerTranslator.cs
*	  FbStringToLowerTranslator now IBStringToLowerTranslator
*		Translate now single quotes "'BOTH'", "'LEADING'" and "'TRAILING'" to be passed to EF_TRIM UDF
			
			
		  
	
