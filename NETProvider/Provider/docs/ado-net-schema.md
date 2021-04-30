# ADO.NET - Schema

### Steps

* Install `InterBaseSql.Data.InterBaseClient` from NuGet.
* Add `using InterBaseSql.Data.InterBaseClient;`.

### Code

```csharp
using (var connection = new IBConnection("database=localhost:demo.ib;user=sysdba;password=masterkey"))
{
	connection.Open();

	var metadataCollections = connection.GetSchema();
	var dataTypes = connection.GetSchema(DbMetaDataCollectionNames.DataTypes);
	var dataSourceInformation = connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation);
	var reservedWords = connection.GetSchema(DbMetaDataCollectionNames.ReservedWords);
	var userTables = connection.GetSchema("Tables", new string[] { null, null, null, "TABLE" });
	var systemTables = connection.GetSchema("Tables", new string[] { null, null, null, "SYSTEM TABLE" });
	var tableColumns = connection.GetSchema("Columns", new string[] { null, null, "TableName" });
}
```
