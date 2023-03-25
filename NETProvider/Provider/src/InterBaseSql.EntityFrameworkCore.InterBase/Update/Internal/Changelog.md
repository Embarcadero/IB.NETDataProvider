#Changes for 7.13.6 (updated for EFCore 6.0 and to Fb 9.x)

## IBUpdateSqlGenerator.cs
* removed returning values from insert and updates since IB does not support this.

# Changes for 7.10.2 

##  General - 
*	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

##  Common -
*    Namespace changed from FirebirdSql.EntityFrameworkCore.Firebird.Storage.Internal to InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal
*    Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.
		
*	  All files renamed from FbXxxxx.cs to IBXxxxx.cs with the internal class matching that same change.  
*	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup

##  FbUpdateSqlGenerator.cs renamed to IBUpdateSqlGenerator.cs
*	  FbUpdateSqlGenerator now IBUpdateSqlGenerator
*	  AppendInsertOperation removed the anyread section as IB does not support RETURNING VALUES in insert statements
*   AppendUpdateOperation removed the execute block that was used to get updated values.  IB version does not update server side changes, user needs ot refresh te object in that situation
*		AppendDeleteOperation removed the execute block that was used to determine rows affected.
			
