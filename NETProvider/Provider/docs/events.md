# Events

### Steps

* Install `InterBaseSql.Data.InterBaseClient` from NuGet.
* Add `using InterBaseSql.Data.InterBaseClient;`.

### Code

```csharp
using (var events = new IBRemoteEvent("database=localhost:demo.ib;user=sysdba;password=masterkey"))
{
	events.RemoteEventCounts += (sender, e) => Console.WriteLine($"Event: {e.Name} | Counts: {e.Counts}");
	events.RemoteEventError += (sender, e) => Console.WriteLine($"ERROR: {e.Error}");
	events.QueueEvents("EVENT1", "EVENT2", "EVENT3", "EVENT4");
	Console.WriteLine("Listening...");
	Console.ReadLine();
}
```
