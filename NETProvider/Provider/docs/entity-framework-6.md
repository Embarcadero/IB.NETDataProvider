# Entity Framework 6

### Steps

* Install `EntityFramework.InterBase` from NuGet.
* Add `DbProviderFactories` record.
```xml
	<system.data>
		<DbProviderFactories>
			<remove invariant="InterBaseSql.Data.InterBaseClient" />
			<add name="InterBaseClient" description="InterBaseClient" invariant="InterBaseSql.Data.InterBaseClient" type="InterBaseSql.Data.InterBaseClient.InterBaseClientFactory, InterBaseSql.Data.InterBaseClient" />
		</DbProviderFactories>
	</system.data>
```
* Add/modify `entityFramework` configuration section.
```xml
	<configSections>
		<section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
	</configSections>
	<entityFramework>
		<defaultConnectionFactory type="EntityFramework.InterBase.IBConnectionFactory, EntityFramework.InterBase" />
		<providers>
			<provider invariantName="InterBaseSql.Data.InterBaseClient" type="EntityFramework.InterBase.IBProviderServices, EntityFramework.InterBase" />
			<provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
		</providers>
	</entityFramework>
```
* Create your `DbContext`.

### Code

```csharp
class Program
{
	static void Main(string[] args)
	{
		using (var db = new MyContext("database=localhost:demo.ib;user=sysdba;password=masterkey"))
		{
			db.Database.Log = Console.WriteLine;

			db.Demos.ToList();
		}
	}
}

class MyContext : DbContext
{
	public MyContext(string connectionString)
		: base(new IBConnection(connectionString), true)
	{ }

	public DbSet<Demo> Demos { get; set; }

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Properties()
			.Configure(x => x.HasColumnName(x.ClrPropertyInfo.Name.ToUpper()));

		var demoConf = modelBuilder.Entity<Demo>();
		demoConf.ToTable("DEMO");
	}
}

class Demo
{
	public int Id { get; set; }
	public string FooBar { get; set; }
}
``` 

### Scripts

```sql
create table demo (id int primary key, foobar varchar(20) character set utf8);
```

```sql
insert into demo values (6, 'FooBar');
```
