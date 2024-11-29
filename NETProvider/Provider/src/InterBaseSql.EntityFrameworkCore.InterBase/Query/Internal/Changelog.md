# Changes for 10.0.1

## IBQueryTranslationPreprocessorFactory.cs, IBQueryTranslationPreprocessor.cs, IBQueryRootProcessor.cs (Added)

## IBQuerySqlGenerator.cs
** Added VisitCase override to handle booleans (append an "= TRUE" so compatible with IB)
** Added VisitCrossJoin override to use IB's "," syntax
** Added VisitSqlUnary override to hand BITNOT comparisons
** All the EF_BINXxx corrected to EF_BITXxx

# Changes for 7.13.6 (Primarily matching Fb 9.x driver changes for EFCore 6.0)

## Added IBSQLTranslatingExpressionVisitor.cs and IBSQLTranslatingExpressionVisitorFactory.cs



# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Query.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Query.Internal
*		Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

*    using FirebirdSql.EntityFrameworkCore.Firebird.Infrastructure.Internal, FirebirdSql.EntityFrameworkCore.Firebird.Query.Expressions.Internal and FirebirdSql.EntityFrameworkCore.Firebird.Storage.Internal
*		  now InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal, InterBaseSql.EntityFrameworkCore.InterBase.Query.Expressions.Internal and InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal
			
##	FbQuerySqlGenerator.cs now IBQuerySqlGenerator.cs
*	  FbQuerySqlGenerator now IBQuerySqlGenerator
*		  Added private GeneratingLimits boolean used to impliment Rows without a cast on the parameters at the end vs Top in the SELECT section.
*			VisitSqlBinary "MOD" now "EF_MOD"
*			VisitSqlBinary "BIN_AND" now "EF_BINAND"
*			VisitSqlBinary "BIN_OR" now "EF_BINOR"
*			VisitSqlBinary "BINXOR" now "EF_BIN_XOR"
*			VisitSqlBinary "BIN_SHL" now "EF_BINSHL"
*			VisitSqlBinary "BIN_SHR" now "EF_BINSHR"
*			VisitSqlParameter no longer casts the parameter for rows ever when GeneratingLimits is true 
*			VisitSqlConstant no longer casts the parameter when GeneratingLimits is true 
*			GenerateLimitOffset 
*			  sets the GeneratingLimits to true to stop casts beign generated regardless of the sqlConstantExpression value
*        Uses the integer max as a end limit vs long max value (IB only supports rows to maxint)
*				GeneratingLimits set to false before exiting
*			VisitSubstring now uses EF_SUBSTR and no longer uses the FB specific FROM..FOR form
*			VisitTrim calls EF_TRIM instead of the internal function for Fb TRIM
			
			
