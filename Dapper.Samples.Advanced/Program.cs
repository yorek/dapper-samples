using System;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Reflection;

namespace Dapper.Samples.Advanced
{
    public interface ISample
    {
        int Order { get; }
        string Name { get; }
        void ShowSample(SqlConnection conn);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dapper .NET Usage Samples");
            Console.WriteLine("by Davide Mauri");
            Console.WriteLine("https://medium.com/dapper-net");
            Console.WriteLine();

            var dataSource = @"(LocalDB)\MSSQLLocalDB";
            //var dataSource = @".";

            // Create connection string
            var dataFolder = Directory.GetParent(Environment.CurrentDirectory).GetDirectories("Dapper.Samples.Data").Single();
            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = dataSource,
                AttachDBFilename = $@"{dataFolder.FullName}\DapperSample.mdf",
                IntegratedSecurity = true,
                ConnectTimeout = 30,
                ApplicationName = "Dapper.Samples.Advanced"

            };

            // Check for connectivity to SQL Server
            try {
                var dummy = new SqlConnection(builder.ConnectionString);
                dummy.Open();
            } catch (SqlException) {
                Console.WriteLine($"ERROR: Cannot open connection to {dataSource}");
                return;
            }

            // Wrap common code in an Action shell
            Action<string, Action<SqlConnection>> ExecuteSample = (message, action) =>
            {
                Console.WriteLine(message);
                Console.WriteLine(new String('-', message.Length));
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    action(conn);
                }
                Console.WriteLine("Done");   
                Console.WriteLine();                    
            };

            Console.WriteLine("Looking for available samples...");
            var samples = from t in Assembly.GetExecutingAssembly().GetTypes() 
                where t.GetInterfaces().Contains(typeof(ISample)) 
                select Activator.CreateInstance(t) as ISample;
            Console.WriteLine("Ready!");
            Console.WriteLine();
            
            var orderedSamples = from s in samples 
                                    orderby s.Order 
                                    where (
                                        args.Count()==0 || 
                                        (args.Count() > 0 && s.Name.Equals(args[0], StringComparison.InvariantCultureIgnoreCase))
                                    )
                                    select s;

            if (orderedSamples.Count() == 0) 
            {
                bool helpRequested = new string[] { "-help", "/help", "/h", "/?", "-?"}.Contains(args[0]);

                if (!helpRequested) {
                    Console.WriteLine("No sample found with the given name.");
                    Console.WriteLine();
                }

                Console.WriteLine("The available samples are:");     
                samples.OrderBy(s => s.Order).ToList().ForEach(s => Console.WriteLine($"{s.Order}. {s.Name}"));

                if (helpRequested) {
                    Console.WriteLine();
                    Console.WriteLine("Run the example you want by specifing the name.");
                    Console.WriteLine("Eg: dotnet run \"Multiple Mapping\" -f netcoreapp2.0");
                }
            } 
            else 
            {
                orderedSamples.ToList().ForEach(s => ExecuteSample(s.Name, conn => s.ShowSample(conn)));    
            }            

            Console.WriteLine();
        }
    }
}
