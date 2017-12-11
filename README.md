# Dapper .NET Samples
## Samples that shows how to use Dapper .NET.

These are the samples mentioned in the [Dapper .Net](https://github.com/StackExchange/Dapper) publication on Medium:

https://medium.com/dapper-net

Work is still in progress, so come back often to check out new articles and samples. My aim to to publish one article per week, until all feature of Dapper are covered.

Here's the list of articles published so far:

 1. [Getting Started with Dapper .NET](https://medium.com/dapper-net/get-started-with-dapper-net-591592c335aa)
 2. [Multiple Executions](https://medium.com/dapper-net/multiple-executions-56c410e9f8dd)
 3. Multiple Resultsets: Coming soon...

## Running The Samples

To run the "Basic Samples", related to the first article "Getting Started with Dapper .NET", just run 

```dotnet run```

from 

```Dapper.Samples.Basics```

folder.

To run advanced samples you have to move into 

```Dapper.Samples.Advanced```

and then from here you can just run

```dotnet run```

to run ALL samples or 

```dotnet run "Sample Name"```

to run that specific sample. Eg:

```dotnet run "Multiple Executions"```

to run only the "Multiple Execution" sample.

## Notes

Samples are done using [.NET Core 2.0](https://www.microsoft.com/net/download/windows), make sure you have installed it on your machine.
Samples also use SQL Server as database server. If you don't have a Windows machine, you can use the Docker version: [SQL Server 2017](https://www.microsoft.com/en-us/sql-server/sql-server-2017). 
SQL Server database file is attached automatically using the `LocalDB/MSSQLServer` instance. If you prefer to use a non-local instance, make sure you change the connection string accordingly, and attach the database file to your instance.
