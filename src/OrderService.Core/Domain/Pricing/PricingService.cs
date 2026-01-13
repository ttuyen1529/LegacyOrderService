using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Domain.Pricing
{
    public sealed class PricingService : IPricingService
    {
        private readonly IReadOnlyCollection<IPricingStrategy> _strategies;

        public PricingService(IEnumerable<IPricingStrategy> strategies)
            => _strategies = strategies.ToList();

        public decimal GetUnitPrice(Product product, PricingContext context)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanApply(product, context))
                ?? throw new DomainException("No pricing strategy available.");

            return strategy.CalculateUnitPrice(product, context);
        }
    }
}
