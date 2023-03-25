#Changes for 7.13.6 (updated for EFCore 6.0 and to Fb 9.x)

## removed FiltersIBTest.cs, DbFunctionsIBTest.cs, ComplexNavigationsWeakQueryIBTest.cs, InheritanceIBTest.cs, FiltersInheritanceIBTest.cs, FiltersInheritanceIBFixture.cs,
##         CompiledQueryIBTest.cs, ChangeTrackingIBTest.cs, AsNoTrackingIBTest.cs, AsyncSimpleQueryIBTest.cs, AsyncGearsOfWarQueryIBTest.cs, AsyncFromSqlQueryIBTest.cs,
##         AsTrackingIBTest.cs, ComplexNavigationsWeakQueryIBFixture.cs, GroupByQueryIBTest.cs, SimpleQueryIBTest.cs, QueryTaggingIBTest.cs, QueryNavigationsIBTest.cs,
##         InheritanceIBFixture.cs, IncludeIBTest.cs, IncludeAsyncIBTest.cs

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common 
*    Namespaces changed from FirebirdSql.EntityFrameworkCore.Firebird.FunctionalTests.Query to InterBaseSQL.EntityFrameworkCore.InterBase.FunctionalTests.Query
*		 Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  In some cases the Fb occurs inside the name and not as a prefix.
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup
			
*		using FirebirdSql.Data.FirebirdClient now using InterBaseSql.Data.InterBaseClient
		
*		"Firebird" in strings or test names replaced with "InterBase"
		
 
		
	
		
