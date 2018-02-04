using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Dapper.Samples.Advanced
{
    public class CustomMapping : ISample
    {
        public int Order => 5;

        public string Name => "Custom Mapping";

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public Company Company { get; set; }

            public override string ToString()
            {
                return String.Format($"USER => ({Id}) {FirstName},{LastName},{EmailAddress}");
            }
        }

        public class Company
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }

            public override string ToString()
            {
                return String.Format($"COMPANY => ({Id}) {CompanyName}");
            }
        }

        public void ShowSample(SqlConnection conn)
        {
            // For sake of simplicity use only one dictionary
            // since all column names are unique
            Dictionary<string, string> columnMaps = new Dictionary<string, string>
            {
                // Column => Property
                { "UserId", "Id" },
                { "CompanyId", "Id" }
            };

            // Create mapping function
            var mapper = new Func<Type, string, PropertyInfo>((type, columnName) =>
            {
                if (columnMaps.ContainsKey(columnName))
                    return type.GetProperty(columnMaps[columnName]);
                else
                    return type.GetProperty(columnName);
            });

            // Create customer mapper for User object
            var userMap = new CustomPropertyTypeMap(
                typeof(User), 
                (type, columnName) => mapper(type, columnName)
                );

            // Create customer mapper for Company object
            var companyMap = new CustomPropertyTypeMap(
                typeof(Company),
                (type, columnName) => mapper(type, columnName)
                );

            // Notify Dapper to use the mappers
            SqlMapper.SetTypeMap(typeof(User), userMap);
            SqlMapper.SetTypeMap(typeof(Company), companyMap);

            // Use custom mapping
            Console.WriteLine();
            Console.WriteLine("Single Object Test");
            var user1 = conn.Query<User>("SELECT * FROM [dbo].[UsersCompanies] WHERE UserId = 5").First();
            Console.WriteLine(user1);

            // Once mapping is set, it will be used
            // every time the target object is deserialized
            Console.WriteLine();
            Console.WriteLine("Multiple Object Test");
            var user2 = conn.Query<User, Company, User>(
                "SELECT * FROM [dbo].[UsersCompanies] WHERE UserId = 5",
                (u, c) =>
                {
                    u.Company = c;
                    return u;
                },
                splitOn: "CompanyId").First();
            Console.WriteLine(user2);
            Console.WriteLine(user2.Company);

            Console.WriteLine();
        }
    }
}