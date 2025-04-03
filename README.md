# InterBase .NET Data Provider

# 10.0.2 Summary

## EFCore and primary driver brought up to Fb 10.x equivalent

* Better EFCore 6.0 support
* EFCore is now .NET 8.0 only, if you need Net 6.0 support use 7.14.6 (which supports EFCore 6.0 also)
* Primary driver now supports Async calls.
* Primary driver now always treats ISC_DOUBLE as type double in .NET.  Before it could be Decimal even though the .Value would always be a double.  This can have backwards compatibility issues.  It should only affect columns that started life in Dialect 1 and were Numeric(10+, 1+) and is either still D1 or the DB is now D3 but the column was never converted to a scaled integer.

# 7.14.6 Summary

* Fixed MacOS determining and loading code, so should work on MAC both Default and Embedded

# 7.14.0 Summary (released as part of 7.14.6)

## Better Code page support.
* if System.Text.Encoding.CodePages can be loaded the ANSI code pages are available when the data's code page is null.  Previously high byte characters would fail assuming UTF8 could handle it.
* This might require adding System.Text.Encoding.CodePages.dll to your project on existing projects.

## Better Dialect 1 support - Returns Double as the data type for SQL_D_DOUBLE types instead of scaled int64

## Support for Generating the Schema data that mirrors the old Dbx based ADO.NET driver
* Note that the current ADO.NET driver is not meant to be backwards compatible, but in this instance it was decided to make this portion backwards.
* IBDBXLegacyTypes.IncludeLegacySchemaType added.  Default is false, when true outputs schema data like the old DBX based driver did (types, names etc)

More information at the following links:

* [Providers](NETProvider/Provider/README.md)
	* [ADO.NET provider](NETProvider/Provider/docs/ado-net.md)
	* [Entity Framework 6 provider](NETProvider/Provider/docs/entity-framework-6.md)
	* [Entity Framework Core provider](NETProvider/Provider/docs/entity-framework-core.md)
	* [Services - Backup](NETProvider/Provider/docs/services-backup.md)
	* [Events](NETProvider/Provider/docs/events.md)
	* [ADO.NET - Schema](NETProvider/Provider/docs/ado-net-schema.md)
* [DDEX provider](NETProvider/DDEX/readme.md)

| NuGets | Version | Downloads |
|--------|---------|-----------|
| [InterBaseSql.Data.InterBaseClient](https://www.nuget.org/packages/InterBaseSql.Data.InterBaseClient) | ![InterBaseSql.Data.InterBaseClient](https://img.shields.io/nuget/v/InterBaseSql.Data.InterBaseClient.svg) | ![InterBaseSql.Data.InterBaseClient](https://img.shields.io/nuget/dt/InterBaseSql.Data.InterBaseClient.svg) |
| [InterBaseSql.EntityFrameworkCore.InterBase](https://www.nuget.org/packages/InterBaseSql.EntityFrameworkCore.InterBase) | ![InterBaseSql.EntityFrameworkCore.InterBase](https://img.shields.io/nuget/v/InterBaseSql.EntityFrameworkCore.InterBase.svg) | ![InterBaseSql.EntityFrameworkCore.InterBase](https://img.shields.io/nuget/dt/InterBaseSql.EntityFrameworkCore.InterBase.svg) |

| GitHub |  |
|--------|--|
| Downloads | ![Downloads](https://img.shields.io/github/downloads/Embarcadero/IB.NETDataProvider/total.svg) |
| Code size | ![Code size](https://img.shields.io/github/languages/code-size/Embarcadero/IB.NETDataProvider) |
