using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Dapper.Samples.Advanced
{
    public class MultipleExecution: ISample
    {
        public string Name => "Multiple Execution";

        public int Order => 1;

        public class ShoppingCart{
            public string UserName { get; set; }

            public List<ShoppingCartItem> Items { get; }

            public ShoppingCart(string userName) {
                UserName = userName;
                Items = new List<ShoppingCartItem>();
            }
        }

        public class ShoppingCartItem
        {
            public string ProductName { get; set; }

            public int Quantity { get; set; }    

            public decimal SellingPrice { get; set; }
        }
        
        public void ShowSample(SqlConnection conn)
        {
            ShoppingCart sc = new ShoppingCart("Davide");
            sc.Items.Add(new ShoppingCartItem() { ProductName = "Soccer Ball", Quantity = 1, SellingPrice = 10.99M });
            sc.Items.Add(new ShoppingCartItem() { ProductName = "Goals", Quantity = 2, SellingPrice = 24.99M });
            sc.Items.Add(new ShoppingCartItem() { ProductName = "Whistle", Quantity = 1, SellingPrice = 5.97M });
                    
            var parametersList = from item in sc.Items select new DynamicParameters(item);

            conn.Execute("INSERT INTO dbo.[ShoppingCartItems] VALUES (@productName, @quantity, @sellingPrice)", parametersList);
        }
    }
}