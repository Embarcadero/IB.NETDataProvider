# ADO.NET

### Steps

* Install `InterBaseSql.Data.InterBaseClient` from NuGet.
* Add `using InterBaseSql.Data.InterBaseClient;`.
* Basic classes are `IBConnection`, `IBTransaction`, `IBCommand` and `IBDataReader`.
* Connection string can be built using `IBConnectionStringBuilder`.

### Code

```csharp
using (var connection = new IBConnection("database=localhost:demo.ib;user=sysdba;password=masterkey"))
{
	connection.Open();
	using (var transaction = connection.BeginTransaction())
	{
		using (var command = new IBCommand("select * from demo", connection, transaction))
		{
			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					var values = new object[reader.FieldCount];
					reader.GetValues(values);
					Console.WriteLine(string.Join("|", values));
				}
			}
		}
	}
}
```

### Scripts

```sql
create table demo (id int primary key, foobar varchar(20) character set utf8);
```

```sql
insert into demo values (6, 'FooBar');
```
