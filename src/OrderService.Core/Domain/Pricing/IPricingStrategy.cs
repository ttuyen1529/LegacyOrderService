using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Domain.Pricing
{
    public interface IPricingStrategy
    {
        bool CanApply(Product product, PricingContext context);
        decimal CalculateUnitPrice(Product product, PricingContext context);
    }
}
