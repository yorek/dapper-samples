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
    public class OneToMany : ISample
    {
        public int Order => 8;

        public string Name => "One-To-Many";

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public Tags Tags { get; set; }

            public override string ToString()
            {
                return String.Format($"User: ({Id}) {FirstName},{LastName},{EmailAddress} ({Tags})");
            }
        }

        public class Tag
        {
            public string Name { get; set; }

            public override string ToString()
            {
                return String.Format($"{Name}");
            }

        }

        public class Tags: List<Tag>
        {
            public override string ToString()
            {
                return String.Format(string.Join(",", this));
            }
        }

        public class TypeHandler<T> : SqlMapper.TypeHandler<T>
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


        public void ShowSample(SqlConnection conn)
        {
            Console.WriteLine();

            Console.WriteLine("Setting Type Handlers...");
            Console.WriteLine();

            SqlMapper.ResetTypeHandlers();
            SqlMapper.AddTypeHandler(new TypeHandler<User>());

            var users = conn.Query<User>("dbo.GetUsersJson", commandType: CommandType.StoredProcedure);
            users.ToList().ForEach(u => {
                Console.WriteLine(u);                                
            });

            Console.WriteLine();
        }
    }
}