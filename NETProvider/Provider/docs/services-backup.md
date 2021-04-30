# Services - Backup

### Steps

* Install `InterBaseSql.Data.InterBaseClient` from NuGet.
* Add `using InterBaseSql.Data.Services;`.

### Code

```csharp
var backup = new IBBackup("database=localhost:demo.ib;user=sysdba;password=masterkey");
backup.BackupFiles.Add(new IBBackupFile(@"C:\backup.ibk"));
//backup.Options = ...
backup.Verbose = true;
backup.ServiceOutput += (sender, e) => Console.WriteLine(e.Message);
backup.Execute();
```

### More
* `IBRestore`