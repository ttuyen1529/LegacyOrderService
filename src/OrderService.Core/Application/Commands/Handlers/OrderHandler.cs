using MediatR;
using OrderService.Core.Abstractions;
using OrderService.Core.Application.Commands;
using OrderService.Core.Domain;
using OrderService.Core.Domain.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Application.Commands.Handlers
{
    public sealed class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Order>
    {
        private readonly IProductCatalog _productCatalog;
        private readonly IPricingService _pricingService;
        public PlaceOrderHandler(IProductCatalog productCatalog, IPricingService pricingService)
        {
            _productCatalog = productCatalog;
            _pricingService = pricingService;
        }
        public async Task<Order> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.CustomerName, nameof(request.CustomerName));
            ArgumentNullException.ThrowIfNull(request.Product, nameof(request.Product));
            if (request.Quantity <= 0)
            {
                throw new DomainException("Quantity must be greater than zero.");
            }
            var unitPrice = _pricingService.GetUnitPrice(request.Product, new PricingContext(request.CustomerName, request.Quantity));
            var order = new Order
            {
                CustomerName = request.CustomerName,
                ProductName = request.Product.ProductName,
                Quantity = request.Quantity,
                Price = unitPrice * request.Quantity
            };
            return order;
        }
    }

    public sealed class SaveOrderHandler : IRequestHandler<SaveOrderCommand>
    {
        private readonly IOrderRepository _repo;

        public SaveOrderHandler(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(SaveOrderCommand req, CancellationToken ct)
        {
            await _repo.SaveAsync(req.Order, ct);
        }
    }
}
