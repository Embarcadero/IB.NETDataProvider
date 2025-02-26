#Changes for 10.0.1

## IBColumns.cs, IBDomains.cs, IBFunctions.cs, IBGenerators.cs, IBIndexes.cs, IBProcedureParameters.cs, IBProcedures.cs, IBTables.cs,
## IBTriggers.cs, IBViewColumns.cs, IBViews.cs 
** ProcessResult now returns a DataTable

## IBForeignKeyColumns.cs, IBForeignKeys.cs, IBIndexColumns.cs, IBTableConstraints.cs
** Deleted ProcessResults

## IBMetaData.xml
** Added DbxDataType information (makes it basically the same as IBMetaData_legacy.xml version which should be removed in next release)

## IBProcedurePrivileges.cs
** Class name changed to IBProcedurePrivileges from IBProcedurePrivilegesSchema

## IBSchema.cs
** Added GetSchemaAsync
** ProcessResult return type now DataTable

## IBSchemaFactory.cs
** Added GetSchemaAsync

# Changes for 7.14.6

# IBChecksByTable.cs
* GetCommandText - fixed pulling the wrong column for the constraint name

# IBColumns.cs, IBForeignKeyColumns.cs, IBForeignKeys.cs, IBIndexColumns.cs, IBIndexes.cs, IBProcedureParameters.cs, IBProcedures.cs, IBTableConstraints.cs
* Support added for IBDBXLegacyTypes.IncludeLegacySchemaType being true

# IBSchema.cs
* Added a Dialect parameter for support of dialect 1

# IBMetaData.xml, IBLegacryData.XML

* Updated the reserved words to IB only words.  So added some and removed Fb specific ones.  This is used in schema catalogs

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
	
