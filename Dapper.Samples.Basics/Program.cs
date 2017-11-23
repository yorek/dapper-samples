using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;

namespace Dapper.Samples.Basics
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
                ApplicationName = "Dapper.Samples.Basics"

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

            // Return a dynamic type list
            ExecuteSample("1. Return a list of dynamic objects", conn =>
            {
                var queryResult = conn.Query("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
                Console.WriteLine("{0} is a {1}", nameof(queryResult), queryResult.GetType());                
                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));                

            });
            
            // Return a strongly typed list
            ExecuteSample("2. Return a list of strongly typed objects", conn =>
            {
                var queryResult = conn.Query<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
                Console.WriteLine("{0} is a {1}", nameof(queryResult), queryResult.GetType());                
                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));                
            });           

            // Execute sample: return affected rows
            ExecuteSample("3. Execute a statement and return affected rows. Any resultset will be discared.", conn =>
            {
                int affectedRows = conn.Execute("UPDATE dbo.[Users] SET [FirstName] = 'John' WHERE [Id] = 3");
                Console.WriteLine("'UPDATE' Affected Rows: {0}", affectedRows);
            });                        
            
            // Return an untyped scalar
            ExecuteSample("4. Return an object scalar", conn =>
            {
                object firstName = conn.ExecuteScalar("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
                Console.WriteLine("{0} ({1}): {2}", nameof(firstName), firstName.GetType(), firstName);
            });

            // Return an typed scalar
            ExecuteSample("5. Return a typed object", conn =>
            {
                var firstName = conn.ExecuteScalar<string>("SELECT [FirstName] FROM dbo.[Users] WHERE [Id] = 1");
                Console.WriteLine("{0} ({1}): {2}", nameof(firstName), firstName.GetType(), firstName);
            });
            
            // Parameters sample
            ExecuteSample("6. Create and set parameter values using anonymous objects.", conn =>
            {
                var queryResult = conn.Query<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users] WHERE Id = @Id", new { @Id = 1 });
                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));                     
            });            

            ExecuteSample("6b. Use an IEnumerable as parameter.", conn =>
            {
                string[] lastNames = new string[] { "Black", "Green", "White", "Brown" };

                var queryResult = conn.Query<User>(
                        "SELECT [Id], [FirstName], [LastName] FROM dbo.[Users] WHERE LastName in @LastName", 
                        new { @LastName = lastNames }
                        );

                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));                     
            });            

            ExecuteSample("7. Create and set parameter values and properties (type and size).", conn =>
            {                
                DynamicParameters dp = new DynamicParameters();
                dp.Add("FirstName", "Davide", DbType.String, ParameterDirection.Input, 100);
                dp.Add("LastName", "Mauri");

                var queryResult = conn.Query<User>("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users] WHERE FirstName = @FirstName AND LastName = @LastName", dp);
                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));                          
            });

            // Stored procedure execution sample
            ExecuteSample("8. Execute a stored procedure that returns user info.", conn =>
            {
                var queryResult = conn.Query<User>("dbo.ProcedureBasic", new { @email = "info@davidemauri.it" }, commandType: CommandType.StoredProcedure);

                queryResult.ToList().ForEach(u => Console.WriteLine($"{u}"));                          
            });

            // Parameters
            ExecuteSample("9. Execute a stored procedure that has input and output params and a return value.", conn =>
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("email", "info@davidemauri.it");
                dp.Add("firstName", null, DbType.String, ParameterDirection.Output, 100); // Manually specify parameter details
                dp.Add("lastName", "", direction: ParameterDirection.Output); // Infer type and size from given value
                dp.Add("result", null, DbType.Int32, ParameterDirection.ReturnValue);

                conn.Execute("dbo.ProcedureWithOutputAndReturnValue", dp, commandType: CommandType.StoredProcedure);

                Console.WriteLine("User: {0}, {1}, {2}", dp.Get<int>("result"), dp.Get<string>("firstName"), dp.Get<string>("lastName"));                
            });

            // Return a DataReader
            ExecuteSample("10. Get a DataReader, if you really prefer.", conn => {
                var dataReader = conn.ExecuteReader("SELECT [Id], [FirstName], [LastName] FROM dbo.[Users]");
                while (dataReader.Read())
                {
                    Console.WriteLine("User: {0}, {1}, {2}", dataReader["Id"], dataReader["FirstName"], dataReader["LastName"]);
                }                
            });            
        }
    }
}
