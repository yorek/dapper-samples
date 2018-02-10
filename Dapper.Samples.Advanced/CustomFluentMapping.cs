using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;

namespace Dapper.Samples.Advanced
{
    public class CustomFluentMapping : ISample
    {
        public int Order => 6;

        public string Name => "Custom Fluent Mapping";

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Company Company { get; set; }

            public override string ToString()
            {
                return 
                    $"USER => Id: {Id}, FirstName: {FirstName}, LastName: {LastName}" + Environment.NewLine + Company?.ToString();
            }
        }

        public class Company
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }            
            public Address Address { get; set; }

            public override string ToString()
            {
                return $"COMPANY => Id: {Id}, CompanyName: {CompanyName}" + Environment.NewLine + Address?.ToString();
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
                return $"ADDRESS => {Street} {City} {State} ({Country})";
            }
        }

        internal class UserMap : EntityMap<User>
        {
            internal UserMap()
            {
                Map(u => u.Id).ToColumn("UserID");
            }
        }

        internal class CompanyMap : EntityMap<Company>
        {
            internal CompanyMap()
            {
                Map(c => c.Id).ToColumn("CompanyId");
            }
        }

        public void ShowSample(SqlConnection conn)
        {
            FluentMapper.Initialize(config => {
                config.AddMap(new UserMap());
                config.AddMap(new CompanyMap());
            });

            // Use custom mapping
            Console.WriteLine();
            Console.WriteLine("Single Object Test");
            var user1 = conn.Query<User>("SELECT * FROM [dbo].[UsersCompanies] WHERE UserId = 5").First();
            Console.WriteLine(user1);

            // Once mapping is set, it will be used
            // every time the target object is deserialized
            Console.WriteLine();
            Console.WriteLine("Multiple Object Test");
            var user2 = conn.Query<User, Company, Address, User>(
                "SELECT * FROM [dbo].[UsersCompanies] WHERE UserId = 5",
                (u, c, a) =>
                {
                    u.Company = c;
                    u.Company.Address = a;
                    return u;
                },
                splitOn: "CompanyId,Street").First();
            Console.WriteLine(user2);

            Console.WriteLine();
        }
    }
}