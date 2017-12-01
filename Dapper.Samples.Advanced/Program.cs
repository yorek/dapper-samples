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
                Console.WriteLine("Done");   
                Console.WriteLine();                    
            };

            Console.WriteLine("Looking for samples...");
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
                Console.WriteLine("No sample found with the given name");                                
            } 
            else 
            {
                orderedSamples.ToList().ForEach(s => ExecuteSample(s.Name, conn => s.ShowSample(conn)));    
            }            
        }
    }
}
