using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Domain.Pricing.Strategies
{
    public sealed class DefaultPricingStrategy : Pricing.IPricingStrategy
    {
        public bool CanApply(Product product, Pricing.PricingContext context) => true;

        public decimal CalculateUnitPrice(Product product, Pricing.PricingContext context)
            => product.UnitPrice;
    }
}
