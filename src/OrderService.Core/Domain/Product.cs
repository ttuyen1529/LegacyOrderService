using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Domain
{
    public class Product
    {
        public Product(string productName, decimal unitPrice)
        {
            ProductName = productName;
            UnitPrice = unitPrice;
        }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
