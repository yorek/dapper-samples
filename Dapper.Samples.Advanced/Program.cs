using System;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Dapper.Samples.Advanced
{
    class Program
    {
        static void Main(string[] args)
        {
             var dataFolder = Directory.GetParent(Environment.CurrentDirectory).GetDirectories("Dapper.Samples.Data").Single();

            // Create connection string
            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = $@"{dataFolder.FullName
                }\DapperSample.mdf",
                IntegratedSecurity = true,
                ConnectTimeout = 30,
                ApplicationName = "Dapper.Samples.Advanced"

            };

            // Wrap common code in an Action shell
            Action<string, Action<SqlConnection>> ExecuteSample = (message, action) =>
            {
                Console.WriteLine(message);
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    action(conn);
                }
                Console.WriteLine();                    
            };

            ExecuteSample("Mulitple Execution", conn => MultipleExecution.ShowSample(conn));
        }
    }
}
