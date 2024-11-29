# Changes for 10.0.1

## IBBatchExecution.cs
** Added ExecuteAsync, ConnectToDatabaseAsync, CreateDatabaseAsync, ProvideCommandAsync, ProvideConnectionAsync, ExecuteCommandAsync,
**       CommitTransactionAsync, RollbackTransactionAsync, CloseConnectionAsync, DisposeCommandAsync


# Changes for 7.10.2 

##  General 
*	All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.Isql to InterBaseSql.Data.Isql
*	Uses InterBaseSql.Data.InterBaseClient instead of FirebirdSql.Data.FirebirdClient
*	Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.

## FbBatchExecution.cs renamed to IBBatchExecution.cs
*	FbBatchExecution now IBBatchExecution
		
## FbScript.cs	renamed to IBScript.cs
*	FbScript now IBScript
*	GetStatementType 
		* changed ALTER SEQUENCES to ALTER GENERATOR
		* changed CREATE SEQUENCE to CREATE GENERATOR
		* Removed EXECUTE BLOCK
			
## FbStatement.cs renamed to IBStatement.cs
*	FbStatement now IBStatement
		
## FbStatementCollection.cs renamed to IBStatementCollection.cs
*	FbStatementCollection now IBStatementCollection
		
		
		
