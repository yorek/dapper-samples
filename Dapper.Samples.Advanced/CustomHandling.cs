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
    public class CustomHandling : ISample
    {
        public int Order => 7;

        public string Name => "Custom Handling";

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

            public static Roles FromString(string value)
            {
                Roles result = new Roles();

                string[] roles = value.ToString().Split(',');

                result.AddRange(roles.Select(r => new Role(r)));

                return result;
            }
        }

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public JArray Tags { get; set; }
            public Roles Roles { get; set; }

            public override string ToString()
            {
                return 
                    $"USER => Id: {Id}, FirstName: {FirstName}, LastName: {LastName}" + Environment.NewLine + 
                    $"ROLES => {Roles?.ToString()}" + Environment.NewLine + 
                    $"TAGS => {Tags?.ToString()}";
            }
        }

        public class RolesTypeHandler : SqlMapper.TypeHandler<Roles>
        {
            // Handles how data is deserialized into object
            public override Roles Parse(object value)
            {
                return Roles.FromString(value.ToString());
            }

            // Handles how data is saved into the database
            public override void SetValue(IDbDataParameter parameter, Roles value)
            {
                parameter.Value = value.ToString();
            }
        }

        public class JArrayTypeHandler : SqlMapper.TypeHandler<JArray>
        {
            public override JArray Parse(object value)
            {
                string json = value.ToString();
                return JArray.Parse(value.ToString());
            }

            public override void SetValue(IDbDataParameter parameter, JArray value)
            {
                parameter.Value = value.ToString();
            }
        }

        private void PrepareDatabase(SqlConnection conn)
        {
            conn.Execute("DELETE FROM dbo.[UserTags] WHERE UserId = 1");
            conn.Execute("UPDATE dbo.[Users] SET Roles = NULL WHERE Id = 1");
        }

        public void ShowSample(SqlConnection conn)
        {
            Console.WriteLine();
            PrepareDatabase(conn);

            Console.WriteLine("Setting Type Handlers...");
            Console.WriteLine();
            SqlMapper.ResetTypeHandlers();
            SqlMapper.AddTypeHandler(new RolesTypeHandler());
            SqlMapper.AddTypeHandler(new JArrayTypeHandler());

            Console.WriteLine("Reading user '1'...");
            var u = conn.QuerySingle<User>("SELECT Id, FirstName, LastName, EmailAddress, Roles, Tags FROM dbo.UsersTagsView WHERE Id = 1");
            Console.WriteLine(u);
            Console.WriteLine();

            Console.WriteLine("Updating roles for user '1'...");

            Roles roles = new Roles
            {
                new Role("One"),
                new Role("Two"),
                new Role("Three")
            };

            conn.Execute("UPDATE dbo.Users SET Roles = @roles WHERE Id = @userId", new { @userId = 1, @roles = roles });
            
            Console.WriteLine("Adding tags for user '1'...");
            
            JArray tags = new JArray() { "Red", "Green", "Blue"  };

            conn.Execute("INSERT INTO dbo.UserTags (UserId, Tag) SELECT @userId, [value] FROM openjson(@tags) ", new { @userId = 1, @tags = tags });

            Console.WriteLine();
            Console.WriteLine("Reading user '1'...");
            u = conn.QuerySingle<User>("SELECT Id, FirstName, LastName, EmailAddress, Roles, Tags FROM dbo.UsersTagsView WHERE Id = 1");
            Console.WriteLine(u);

            Console.WriteLine();
        }
    }
}