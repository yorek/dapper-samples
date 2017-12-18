using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Dapper.Samples.Advanced
{
    public class MultipleMapping: ISample
    {
        public string Name => "Multiple Mapping";

        public int Order => 3;

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Company Company { get; set; }

            public override string ToString()
            {
                return 
                    $"USER => Id: {Id}, FirstName: {FirstName}, LastName: {LastName}" + Environment.NewLine + Company.ToString();
            }
        }

        public class Company
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }            
            public Address Address { get; set; }

            public override string ToString()
            {
                return $"COMPANY => Id: {Id}, CompanyName: {CompanyName}" + Environment.NewLine + Address.ToString();
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
        
        public void ShowSample(SqlConnection conn)
        {
            var result = conn.Query<User, Company, Address, User>(
                "SELECT * FROM [dbo].[UsersCompanies] WHERE [UserId] = @userId", 
                map: (u, c, a) => {
                    u.Company = c;
                    c.Address = a;

                    return u;
                },                
                splitOn: "CompanyName,Street",
                param: new { @userId = 5 }
            );

            result.ToList().ForEach(u => Console.WriteLine(u));
        }
    }
}