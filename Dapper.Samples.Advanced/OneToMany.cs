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
            public JArray Tags { get; set; }
            public Roles Roles { get; set; }
            public Company Company { get; set; }
            public Preferences Preferences { get; set; }

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

        public class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }

            public override string ToString()
            {
                string[] a = new string[] { Street, City, State, Country };

                var r = Array.FindAll<string>(a, s => !string.IsNullOrEmpty(s));

                return String.Format(string.Join(",", r));
            }
        }

        public class Role
        {
            public string RoleName { get; set; }

            public Role() { }

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
                return string.Join("|", this);
            }

            public static Roles FromString(string value)
            {
                Roles result = new Roles();

                string[] roles = value.ToString().Split('|');

                result.AddRange(roles.Select(r => new Role(r)));

                return result;
            }
        }

        public class Preferences
        {
            public string Theme;
            public string Style;
            public string Resolution;
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

        public class UserTypeHandler : SqlMapper.TypeHandler<User>
        {
            public override User Parse(object value)
            {
                return JsonConvert.DeserializeObject<User>(value.ToString());
            }

            public override void SetValue(IDbDataParameter parameter, User value)
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
            SqlMapper.AddTypeHandler(new JArrayTypeHandler());

            User u = new User()
            {
                FirstName = "Davide",
                LastName = "Mauri",
                EmailAddress = "davide.mauri@gmail.com",
                Tags = new JArray() { "alpha", "beta" },
                Roles = new Roles()
                {
                    new Role() { RoleName = "User" },
                    new Role() { RoleName = "Developer" },
                    new Role() { RoleName = "Administrator"}
                },
                Preferences = new Preferences()
                {
                    Resolution = "1920x1080",
                    Style = "Black",
                    Theme = "Modern"
                },
                Company = new Company()
                {
                    CompanyName = "Microsoft Corp.",
                    Address = new Address()
                    {
                        City = "Redmond",
                        State = "WA",
                        Country = "United States",
                        Street = "16225 NE 87th Street"
                    }
                }
            };

            //var executeResult = conn.Set<User>(u);
            Console.WriteLine();
        }
    }
}