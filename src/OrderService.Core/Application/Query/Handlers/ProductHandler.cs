using MediatR;
using OrderService.Core.Abstractions;
using OrderService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Application.Query.Handlers
{
    public sealed class GetProductByNameQueryHandler : IRequestHandler<GetProductByNameQuery, Product>
    {
        private readonly IProductCatalog _productCatalog;
        public GetProductByNameQueryHandler(IProductCatalog productCatalog)
        {
            _productCatalog = productCatalog;
        }
        public async Task<Product> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.ProductName, nameof(request.ProductName));
            var product = await _productCatalog.GetProduct(request.ProductName);
            return product;
        }
    }
}
