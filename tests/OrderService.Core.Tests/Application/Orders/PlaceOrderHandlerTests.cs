using FluentAssertions;
using Moq;
using OrderService.Core.Abstractions;
using OrderService.Core.Application.Commands;
using OrderService.Core.Application.Commands.Handlers;
using OrderService.Core.Domain;
using OrderService.Core.Domain.Pricing;
using OrderService.Core.Domain.Pricing.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Core.Tests.Application.Orders
{
    public class PlaceOrderHandlerTests
    {
        [Fact]
        public async Task Handle_should_create_order_and_calculate_total()
        {
            // Arrange
            var pricing = new Mock<IPricingService>(MockBehavior.Strict);
            var product = new Mock<IProductCatalog>(MockBehavior.Strict);
            var productMock = new Product("mouse", 10m);
            product.Setup(p => p.GetProduct("mouse"))
                   .ReturnsAsync(productMock);

            pricing.Setup(p => p.GetUnitPrice(
                        productMock,
                        It.Is<PricingContext>(c => c.CustomerName == "Alice" && c.Quantity == 3)))
                   .Returns(10m);

            var handler = new PlaceOrderHandler(product.Object, pricing.Object);

            // record command (positional)
            var cmd = new PlaceOrderCommand("Alice", productMock, 3);

            // Act
            var order = await handler.Handle(cmd, default);

            // Assert
            order.CustomerName.Should().Be("Alice");
            order.ProductName.Should().Be("mouse");
            order.Quantity.Should().Be(3);
            order.Price.Should().Be(30m);

            pricing.VerifyAll();
        }

        [Fact]
        public async Task Handle_should_throw_when_request_is_null()
        {
            // Arrange
            var product = new Mock<IProductCatalog>();
            var pricing = new Mock<IPricingService>();
            var handler = new PlaceOrderHandler(product.Object, pricing.Object);

            // Act
            var act = async () => await handler.Handle(null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                     .WithParameterName("request");
        }

        [Fact]
        public async Task Handle_should_throw_when_customer_name_is_null()
        {
            // Arrange
            var product = new Mock<IProductCatalog>();
            var pricing = new Mock<IPricingService>();
            var handler = new PlaceOrderHandler(product.Object, pricing.Object);

            var productMock = new Product("mouse", 10m);
            var cmd = new PlaceOrderCommand(CustomerName: null!, Product: productMock, Quantity: 1);

            // Act
            var act = async () => await handler.Handle(cmd, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                     .WithParameterName("CustomerName");
        }

        [Fact]
        public async Task Handle_should_throw_when_product_is_null()
        {
            // Arrange
            var pricing = new Mock<IPricingService>();
            var product = new Mock<IProductCatalog>();
            var handler = new PlaceOrderHandler(product.Object, pricing.Object);

            var cmd = new PlaceOrderCommand("Alice", Product: null!, Quantity: 1);

            // Act
            var act = async () => await handler.Handle(cmd, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                     .WithParameterName("Product");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_should_throw_domain_exception_when_quantity_is_not_positive(int qty)
        {
            // Arrange
            var pricing = new Mock<IPricingService>();
            var product = new Mock<IProductCatalog>();
            var handler = new PlaceOrderHandler(product.Object, pricing.Object);

            var productMock = new Product("mouse", 10m);
            var cmd = new PlaceOrderCommand("Alice", productMock, qty);

            // Act
            var act = async () => await handler.Handle(cmd, default);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                     .WithMessage("*Quantity must be greater than zero*");

            // should fail-fast before pricing
            pricing.Verify(p => p.GetUnitPrice(It.IsAny<Product>(), It.IsAny<PricingContext>()), Times.Never);
        }
    }
}
