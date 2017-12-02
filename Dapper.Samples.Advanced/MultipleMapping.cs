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
            public string FirstName { get; private set; }
            public string LastName { get; private set; }

            public override string ToString()
            {
                return $"USER => FirstName: {FirstName}, LastName: {LastName}";
            }
        }

        public class Company
        {
            public int CompanyId { get; private set; }
            public string CompanyName { get; private set; }

            public override string ToString()
            {
                return $"COMPANY => FirstName: {CompanyName}";
            }
        }

        public class Address
        {
            public string Street { get; private set; }

            public string City { get; private set; }

            public string State { get; private set; }

            public string Country { get; private set; }

            public override string ToString()
            {
                return $"Address => {Street} {City} {State} ({Country})";
            }
        }
        
        public void ShowSample(SqlConnection conn)
        {
            var result = conn.QueryFirst<User, Company, Address, User>(
                "SELECT * FROM [dbo].[UsersCompanies] WHERE UserId = @userId", 
                new { @userId = 5 },
                (u, c, a) = {},
                splitOn: "CompanyName,Street"
            );
        }
    }
}