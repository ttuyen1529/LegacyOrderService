using FluentAssertions;
using OrderService.Core.Domain;
using OrderService.Core.Domain.Pricing;
using OrderService.Core.Domain.Pricing.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Tests.Domain
{
    public class PricingServiceTests
    {
        [Fact]
        public void GetUnitPrice_should_use_bulk_discount_when_quantity_is_high()
        {
            // Arrange
            var strategies = new IPricingStrategy[]
            {
            new BulkDiscountPricingStrategy(),
            new DefaultPricingStrategy()
            };
            var service = new PricingService(strategies);

            var product = new Product("mouse", 10m);
            var ctx = new PricingContext("Tuyen", Quantity: 100);

            // Act
            var price = service.GetUnitPrice(product, ctx);

            // Assert
            price.Should().Be(9m); // 10 * 0.9
        }

        [Fact]
        public void GetUnitPrice_should_fallback_to_default_when_no_other_strategy_applies()
        {
            // Arrange
            var strategies = new IPricingStrategy[]
            {
            // no bulk discount because qty small
            new BulkDiscountPricingStrategy(),
            new DefaultPricingStrategy()
            };
            var service = new PricingService(strategies);

            var product = new Product("coffee", 5m);
            var ctx = new PricingContext("Bob", Quantity: 1);

            // Act
            var price = service.GetUnitPrice(product, ctx);

            // Assert
            price.Should().Be(5m);
        }

        [Fact]
        public void GetUnitPrice_should_throw_when_no_strategy_available()
        {
            // Arrange
            var service = new PricingService(Array.Empty<IPricingStrategy>());

            var product = new Product("x", 1m);
            var ctx = new PricingContext("Bob", 1);

            // Act
            var act = () => service.GetUnitPrice(product, ctx);

            // Assert
            act.Should().Throw<DomainException>()
               .WithMessage("*No pricing strategy*");
        }
    }
}
