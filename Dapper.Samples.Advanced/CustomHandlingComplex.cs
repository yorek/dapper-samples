using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dapper.Samples.Advanced
{
    public class CustomHandlingComplex : ISample
    {
        public int Order => 9;

        public string Name => "Complex Custom Handling";

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public JArray Tags { get; set; }
            public Roles Roles { get; set; }
            public Company Company { get; set; }
            public Preferences Preferences { get; set; }

            public override string ToString()
            {
               return 
                    $"USER => Id: {Id}, FirstName: {FirstName}, LastName: {LastName}" + Environment.NewLine + 
                    $"ROLES => {Roles?.ToString()}" + Environment.NewLine + 
                    $"TAGS => {Tags?.ToString()}" + Environment.NewLine + 
                    $"COMPANY => {Company?.ToString()}";
            }
        }

        public class Company
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public Address Address { get; set; }

            public override string ToString()
            {
                return String.Format($"Id: {Id}, Name: {CompanyName}, Address: {Address.ToString()}");
            }
        }

        public class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }

            public override string ToString()
            {
                string[] a = new string[] { Street, City, State, Country };

                var r = Array.FindAll<string>(a, s => !string.IsNullOrEmpty(s));

                return String.Format(string.Join(",", r));
            }
        }

        public class Role
        {
            public string RoleName { get; set; }

            public Role()
            {
            }

            public Role(string name)
            {
                RoleName = name;
            }

            public override string ToString()
            {
                return RoleName;
            }
        }

        public class Roles : List<Role>
        {
            public override string ToString()
            {
                return string.Join(",", this);
            }
        }

        public class Preferences
        {
            public string Theme;
            public string Style;
            public string Resolution;
        }
       
        public class CustomTypeHandler<T> : SqlMapper.TypeHandler<T>
        {
            public override T Parse(object value)
            {
                return JsonConvert.DeserializeObject<T>(value.ToString());
            }

            public override void SetValue(IDbDataParameter parameter, T value)
            {
                parameter.Value = JsonConvert.SerializeObject(value);
            }
        }

        private void PrepareDatabase(SqlConnection conn)
        {
            conn.Execute("DELETE ut FROM dbo.[UserTags] ut INNER JOIN dbo.[Users] u ON ut.UserId = u.Id WHERE EMailAddress = 'davide.mauri@gmail.com'");
            conn.Execute("DELETE FROM dbo.[Users] WHERE EMailAddress = 'davide.mauri@gmail.com'");
            conn.Execute("DELETE FROM dbo.[Companies] WHERE CompanyName = 'Microsoft'");
        }

        public void ShowSample(SqlConnection conn)
        {
            Console.WriteLine();
            PrepareDatabase(conn);

            Console.WriteLine("Setting Type Handlers...");
            Console.WriteLine();
            SqlMapper.ResetTypeHandlers();
            SqlMapper.AddTypeHandler(new CustomTypeHandler<User>());

            Console.WriteLine("Creating User Object...");
            User u = new User()
            {
                FirstName = "Davide",
                LastName = "Mauri",
                EmailAddress = "davide.mauri@gmail.com",
                Tags = new JArray() { "alpha", "beta" },
                Roles = new Roles()
                {
                    new Role() { RoleName = "User" },
                    new Role() { RoleName = "Developer" },
                    new Role() { RoleName = "Administrator"}
                },
                Preferences = new Preferences()
                {
                    Resolution = "1920x1080",
                    Style = "Black",
                    Theme = "Modern"
                },
                Company = new Company()
                {
                    CompanyName = "Microsoft Corp.",
                    Address = new Address()
                    {
                        City = "Redmond",
                        State = "WA",
                        Country = "United States",
                        Street = "350 157th Place NE"
                    }
                }
            };

            Console.WriteLine("Saving User Object to Database via Set<User>...");
            Console.WriteLine();
            var savedUser = conn.Set<User>(u);            
            Console.WriteLine(savedUser.ToString());
            Console.WriteLine();

            Console.WriteLine("Retreving another User Object from Database via Get<User>...");
            Console.WriteLine();
            var anotherUser = conn.Get<User>(5);            
            Console.WriteLine(anotherUser.ToString());

            Console.WriteLine();
        }
    }

    public static class ExtensionHelper
    {
        public static T Set<T>(this SqlConnection conn, T payload)
        {
            return conn.ExecuteScalar<T>($"dbo.Set{typeof(T).Name}", new { payload }, commandType: CommandType.StoredProcedure);
        }

        public static T Get<T>(this SqlConnection conn, int id)
        {
            return conn.ExecuteScalar<T>($"dbo.Get{typeof(T).Name}", new { id }, commandType: CommandType.StoredProcedure);
        }
    }
}