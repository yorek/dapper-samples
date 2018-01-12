using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
#if !NETCOREAPP2_0
using Microsoft.SqlServer.Types;
#endif

namespace Dapper.Samples.Advanced
{
#if !NETCOREAPP2_0
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public SqlGeography Location { get; set; }

        public override string ToString()
        {
            string[] a = new string[] { Street, City, State, Country };

            var r = Array.FindAll<string>(a, s => !string.IsNullOrEmpty(s));

            return String.Format(string.Join(",", r));
        }
    }
#endif
    public class SQLServerFeatures : ISample
    {

        public string Name => "SQL Server Features";

        public int Order => 3;

        public void ShowSample(SqlConnection conn)
        {
            Console.WriteLine();
            PrepareDatabase(conn);            
            ShowTVP(conn);
            ShowSpatial(conn);
            ShowHierarchyId(conn);
        }

        private void PrepareDatabase(SqlConnection conn)
        {
            conn.Execute("DELETE FROM dbo.[UserTags]");
        }

        private void ShowTVP(SqlConnection conn)
        {
            Console.WriteLine("Testing TVP");
            Console.WriteLine();

            var ut = new DataTable("UserTagsType");
            ut.Columns.Add("UserId", typeof(int));
            ut.Columns.Add("Tag", typeof(string));

            ut.Rows.Add(5, "Developer");
            ut.Rows.Add(5, "Data Guy");
            ut.Rows.Add(1, "SysAdmin");

            int affectedRows = conn.Execute(
                "INSERT INTO dbo.[UserTags] SELECT * FROM @ut",
                new
                {
                    // As of Dapper 1.50.2 Doesn't work yet with .NET Core
                    //@ut = ut.AsTableValuedParameter("UserTagsType")

                    // But this works nice
                    ut = new TableValuedParameter(ut)
                });    

            Console.WriteLine($"Inserted {affectedRows} values into dbo.[UserTags] via TVP.");
            Console.WriteLine();
        }
#if !NETCOREAPP2_0
        private void ShowSpatial(SqlConnection conn)
        {
            Console.WriteLine("Testing SQL Server Spatial Data Types");
            Console.WriteLine();

            // Use a Spatial Data Type direcly
            // via parameter or a resulting object
            var p = SqlGeography.Point(47.6062, -122.3321, 4326);

            var result = conn.ExecuteScalar<SqlGeography>("SELECT @p AS TestPoint", new { @p = p });

            Console.WriteLine(result.STAsText().Value);

            // Test usage of Spatial Data Type
            // embedded in a more complex object
            var result2 = conn.QuerySingle<Address>("SELECT geography::STPointFromText('POINT(-122.3321 47.6062)', 4326) AS Location, 'Seattle' AS City, 'WA' AS State, 'US' AS Country");

            Console.WriteLine("{0}: {1}", result2.ToString(), result2.Location.ToString());

            Console.WriteLine();
        }


        private void ShowHierarchyId(SqlConnection conn)
        {
            Console.WriteLine("Testing SQL Server HiearachyID Data Type");
            Console.WriteLine();

            var n1 = SqlHierarchyId.Parse("/1/1.1/2/");

            var n2 = conn.ExecuteScalar<SqlHierarchyId>("SELECT @h.GetAncestor(2) AS HID", new { @h = n1 });

            Console.WriteLine("Is {0} a descendant of {1}? {2}", n1, n2, n1.IsDescendantOf(n2));            

            Console.WriteLine();
        }
#endif
#if NETCOREAPP2_0
        private void ShowSpatial(SqlConnection conn)
        {
            Console.WriteLine("SQL Server Spatial Data Types not yet supported in .NET Core");
        }

        private void ShowHierarchyId(SqlConnection conn)
        {
            Console.WriteLine("SQL Server HieararchyID Types not yet supported in .NET Core");
        }
#endif
    }

    public class TableValuedParameter : SqlMapper.ICustomQueryParameter
    {
        private DataTable _dataTable;
        private string _typeName;

        public TableValuedParameter(DataTable dataTable)
        {            
            this._typeName = dataTable.TableName;
            _dataTable = dataTable;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            var parameter = (SqlParameter)command.CreateParameter();

            parameter.ParameterName = name;
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.Value = _dataTable;
            parameter.TypeName = _dataTable.TableName;     

            command.Parameters.Add(parameter);
        }
    }
}