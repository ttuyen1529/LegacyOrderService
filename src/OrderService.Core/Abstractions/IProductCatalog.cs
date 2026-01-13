using OrderService.Core.Domain;

namespace OrderService.Core.Abstractions
{
    public interface IProductCatalog
    {
        Task<Product> GetProduct(string productName);
    }
}
