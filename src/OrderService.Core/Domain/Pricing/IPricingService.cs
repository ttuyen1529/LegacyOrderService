using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Domain.Pricing
{
    public interface IPricingService
    {
        decimal GetUnitPrice(Product product, PricingContext context);
    }
}
