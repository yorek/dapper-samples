# Dapper .NET Samples
## Samples that shows how to use Dapper .NET.

These are the samples mentioned in the [Dapper .Net](https://github.com/StackExchange/Dapper) publication on Medium:

[https://medium.com/dapper-net](https://medium.com/dapper-net)

Work is still in progress, so come back often to check out new articles and samples. My aim to to publish one article per week, until all feature of Dapper are covered.

Here's the list of articles published so far:

 1. [Getting Started with Dapper .NET](https://medium.com/dapper-net/get-started-with-dapper-net-591592c335aa)
 2. [Multiple Executions](https://medium.com/dapper-net/multiple-executions-56c410e9f8dd)
 3. [Multiple Resultsets](https://medium.com/dapper-net/handling-multiple-resultsets-4b108a8c5172)
 4. [Multiple Mapping](https://medium.com/dapper-net/multiple-mapping-d36c637d14fa)
 5. [SQL Server Features](https://medium.com/dapper-net/sql-server-specific-features-2773d894a6ae)
 6. [Custom Mapping](https://medium.com/dapper-net/custom-columns-mapping-1cd45dfd51d6)
 7. [Custom Handling](https://medium.com/dapper-net/custom-type-handling-4b447b97c620)
 8. [One-To-Many Relationships]()

## Running The Samples

To run the "Basic Samples", related to the first article "Getting Started with Dapper .NET", just run

```dotnet run -f net452```

from

```Dapper.Samples.Basics```

folder. To run advanced samples you have to move into 

```Dapper.Samples.Advanced```

and then from here you can just run

```dotnet run -f net452```

to run ALL samples or 

```dotnet run -f net452 "Sample Name"```

to run that specific sample. Eg:

```dotnet run -f net452 "Multiple Executions"```

to run only the "Multiple Execution" sample.

To have a list of all advanced samples available run:

```dotnet run -f net452 -help```

## Notes

### .NET Version

Samples are done using [.NET Core 2.0](https://www.microsoft.com/net/download/windows) and [.NET Framework 4.5.2](https://github.com/Microsoft/dotnet/blob/master/releases/README.md): make sure you have them installed it on your machine.

In order to use features not yet supported by .NET Core 2.0, like SQL Server Spatial Data Types, the project supports multiple targets:

* net452
* netcoreapp2.0

To execute the application targeting one specifc framework, just use the `-f` option when running the console app:

```dotnet run -f net452```

more info on the `-f` option here:

[dotnet run](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore2x)

### SQL Server

Samples also use SQL Server as database server. If you don't have a Windows machine, you can use the Docker version: [SQL Server 2017](https://www.microsoft.com/en-us/sql-server/sql-server-2017). 
SQL Server database file is attached automatically using the `LocalDB/MSSQLServer` instance. If you prefer to use a non-local instance, make sure you change the connection string accordingly, and attach the database file to your instance.
