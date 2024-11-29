using InterBaseSql;
using InterBaseSql.Data.InterBaseClient;
using System.Data.Common;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
try
{// Create a new data adapter based on the specified query.
	var Connection = new IBConnection();
	Connection.ConnectionString = "data source=localhost;database=c:\\embarcadero\\InterBase64\\Examples\\Database\\employee.gdb;user=sysdba;password=masterkey";
	Connection.Open();

	var transaction = Connection.BeginTransaction();

	var command = new IBCommand("select * from employee", Connection, transaction);

	IDataReader reader = command.ExecuteReader();
	while (reader.Read())
	{
		var s = ""; ;
		for (var i = 0; i < reader.FieldCount; i++)
		{
			if (s == "")
			{
				s += reader[i].ToString();
			}
			else
			    s = s + " | " + reader.GetValue(i).ToString();
		}
		Console.WriteLine(s);
	}
	if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		Console.WriteLine("Platform is macOS");
	else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		Console.WriteLine("Platform is Linux");

#if NET5_0_OR_GREATER
	Console.WriteLine("NET5_0_OR_GREATER is defined");
#else
    Console.WriteLine("NET5_0_OR_GREATER is NOT defined");
#endif
#if NET8_0_OR_GREATER
    Console.WriteLine("NET8_0_OR_GREATER is defined");
#else
	Console.WriteLine("NET8_0_OR_GREATER is NOT defined");
#endif

	reader.Close();
	command.Dispose();
	transaction.Rollback();
}
catch (Exception e)
{
	Console.WriteLine(e.Message);
}

// See https://aka.ms/new-console-template for more information