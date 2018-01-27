using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Dapper.Samples.Advanced
{
    public class CustomMapping : ISample
    {
        public int Order => 5;

        public string Name => "Custom Mapping";

        public void ShowSample(SqlConnection conn)
        {
            Console.WriteLine("Still working on it...");
        }
    }
}