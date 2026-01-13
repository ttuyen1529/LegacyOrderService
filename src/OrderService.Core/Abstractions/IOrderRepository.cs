using OrderService.Core.Domain;

namespace OrderService.Core.Abstractions
{
    public interface IOrderRepository
    {
        public Task SaveAsync(Order order, CancellationToken ct = default);
    }
}
