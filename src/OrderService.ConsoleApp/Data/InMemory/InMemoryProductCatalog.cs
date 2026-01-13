using OrderService.Core.Abstractions;
using OrderService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.ConsoleApp.Data.InMemory
{
    public class InMemoryProductCatalog : IProductCatalog
    {
        private readonly Dictionary<string, Product> _product = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Widget"] = new Product("Widget", 12.99M),
            ["Gadget"] = new Product("Gadget", 15.49M),
            ["Doohickey"] = new Product("Doohickey", 8.75M)
        };
        public async Task<Product> GetProduct(string productName)
        {
            // Simulate an expensive lookup
            Thread.Sleep(500);

            if (_product.TryGetValue(productName, out var p))
                return p;

            throw new DomainException($"Unknown product: {productName}");
        }
    }
}
