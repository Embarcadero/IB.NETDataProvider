# Changes for 7.10.2 

## General 
* All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
* Namespaces changed from FirebirdSql.Data.Schema to InterBaseSql.Data.Schema
*	Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.	
*	All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
	* For example FbIndexes.cs becomes IBIndexes.cs and the class FbIndexes is now IBIndexes
* All "ORDER BY " sections switched to the underlying column instead of the Alias (not needed actually)	
	
## FbColumns.cs renamed to IBColumns.cs
*	GetCommandText removed IDENTITY_TYPE column in the select (Fb specific)
*	ProcessResult 
		* removed IS_IDENTITY column
		*	removed setting the IS_IDENTITY column
		
## FbFunctions.cs renamed to IBFunctions.cs
*	removed PACKAGE_NAME from the Select SQL
*	removed PACKAGE_NAME from the order by
	
## FbMetaData.xml renamed to IBMetaData.xml	
* Firebird string replaced with InterBaseSql
*	Removed entry fo bigint
		
## FbProcedureParameters.cs renamed to IBProcedureParameters.cs
*	Removed PACKAGE support from the SQL
*	Removed PACKAGE_NAME from the ORDER BY
		
## FbProcedures.cs renamed to IBProcedures.cs
*	Removed PACKAGE support from the SQL
*	Removed PACKAGE_NAME from the ORDER BY

## FbSchemaFactory.cs renamed to IBSchemaFactory.cs
* Replaced the Firebird xml with InterBaseSql.Data.Schema.IBMetaData.xml
	
