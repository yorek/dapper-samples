using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Dapper.Samples.Advanced
{
    public class MultipleResultsets: ISample
    {
        
        public string Name => "Multiple Resultsets";

        public int Order => 2;

        public class User
        {
            public int Id { get; private set; }
            public string FirstName { get; private set; }
            public string LastName { get; private set; }

            public override string ToString()
            {
                return $"USER => Id: {Id}, FirstName: {FirstName}, LastName: {LastName}";
            }
        }

        public class Company
        {
            public int Id { get; private set; }
            public string CompanyName { get; private set; }
            public override string ToString()
            {
                return $"COMPANY => Id: {Id}, FirstName: {CompanyName}";
            }
        }

        public void ShowSample(SqlConnection conn)
        {
            var results = conn.QueryMultiple("SELECT Id, FirstName, LastName FROM dbo.Users; SELECT Id, CompanyName FROM dbo.Companies");
            var users = results.Read<User>();
            var companies = results.Read<Company>();

            users.ToList().ForEach(u => Console.WriteLine($"{u}"));     
            companies.ToList().ForEach(c => Console.WriteLine($"{c}"));     
        }
    }
}