using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Domain.Pricing.Strategies
{
    public sealed class BulkDiscountPricingStrategy : Pricing.IPricingStrategy
    {
        public bool CanApply(Product product, Pricing.PricingContext context)
            => context.Quantity >= 100; //example

        public decimal CalculateUnitPrice(Product product, Pricing.PricingContext context)
            => product.UnitPrice * 0.9m;
    }
}
