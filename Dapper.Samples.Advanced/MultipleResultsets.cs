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

        public class Customer
        {
            public int Id { get; private set; }
            public string Name { get; private set; }
            public List<CustomerOrder> Orders => _orders;
            public decimal TotalOrdersValue => _orders.Sum(o => o.OrderAmount);

            private List<CustomerOrder> _orders = new List<CustomerOrder>();

            public override string ToString()
            {
                return $"CUSTOMER => Id: {Id}, Name: {Name}, TotalOrdersValue: {TotalOrdersValue}";
            }
        }

        public class CustomerOrder
        {
            public int Id { get; private set; }
            public DateTime OrderDate { get; private set; }
            public decimal OrderAmount { get; private set; }
            
            public override string ToString()
            {
                return $"ORDER => Id: {Id}, OrderDate: {OrderDate}, OrderAmout: {OrderAmount}";
            }
        }

        public void ShowSample(SqlConnection conn)
        {
            var results = conn.QueryMultiple(@"
                SELECT Id, [Name] FROM dbo.Customers WHERE Id = @customerId; 
                SELECT Id, [OrderDate], [OrderAmount] FROM dbo.Orders WHERE CustomerId = @customerId;
                ",
                new {
                    @customerId = 1
                });

            var customer = results.ReadSingle<Customer>();
            var orders = results.Read<CustomerOrder>();

            customer.Orders.AddRange(orders);

            Console.WriteLine($"{customer}");     
            foreach(var o in customer.Orders)
            {
                Console.WriteLine($"{o}");     
            }
        }
    }
}