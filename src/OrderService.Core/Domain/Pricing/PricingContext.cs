using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Domain.Pricing
{
    public record PricingContext(string CustomerName, int Quantity);
}
