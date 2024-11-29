# Changes for 10.0.1
** Added ~30 new test units

## ComplexNavigationsQueryIBFixture.cs
** Added Seed override to cleanup items that stopped seeding

## ComplexNavigationsQueryIBTest.cs
** Removed all GeneratedNameTooLongTheory overrides

## GearsOfWarQueryIBFixture.cs
** Added Seed override

## InheritanceRelationshipsQueryIBFixture.cs (removed)

## QueryNoClientEvalIBFixture.cs (removed)

## NullKeysIBTest.cs
** Added OnModelCreating
** Override Seed to build data in a correct order

## NullSemanticsQueryIBFixture.cs
** Added OnModelCreating

## NullSemanticsQueryIBTest.cs
** Override Bool_equal_nullable_bool_compared_to_null, Bool_equal_nullable_bool_HasValue, Bool_logical_operation_with_nullable_bool_HasValue, Bool_not_equal_nullable_bool_HasValue,
**          Bool_not_equal_nullable_bool_compared_to_null, Like_with_escape_char and Bool_not_equal_nullable_int_HasValue to not call checks not valid for IB
** Set multiple functions as not supported by IB

## QueryNoClientEvalIBTest.cs, WarningsIBTest.cs
** Switched Fixture from QueryNoClientEvalIBFixture to NorthwindQueryIBFixture<NoopModelCustomizer>

## UdfDbFunctionIBTests.cs
** OnModelCreating fixed up EFUDF function names
** Seed removed the CREATE FUNCTION sql as IB does not support this.

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
		
 
		
	
		
