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
                return String.Format($"({Id}) {FirstName},{LastName},{EmailAddress}");
            }
        }

        public class Company
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public Address Address { get; set; }

            public override string ToString()
            {
                return String.Format($"({Id}) {CompanyName}");
            }
        }

        public void ShowSample(SqlConnection conn)
        {
            // For sake of simplicity use only one dictionary
            // since all column names are unique
            Dictionary<string, string> columnMaps = new Dictionary<string, string>
            {
                // Column, Property
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

            // Same query as before (just the split column is changed)
            var queryResult = conn.Query<User, Company, Address, User>(
                                "SELECT * FROM [dbo].[UsersAndCompanyAndAddress] WHERE UserId = 5",
                                (u, c, a) =>
                                {
                                    u.Company = c;
                                    u.Company.Address = a;
                                    return u;
                                },
                                splitOn: "CompanyId,Street").First();

            Console.WriteLine(queryResult);
            Console.WriteLine(queryResult.Company);
            Console.WriteLine(queryResult.Company.Address);
        }
    }
}