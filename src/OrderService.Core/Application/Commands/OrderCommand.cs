using MediatR;
using OrderService.Core.Domain;

namespace OrderService.Core.Application.Commands
{
    public sealed record PlaceOrderCommand(string CustomerName, Product Product, int Quantity) : IRequest<Order>;
    public sealed record SaveOrderCommand(Order Order) : IRequest;
}
