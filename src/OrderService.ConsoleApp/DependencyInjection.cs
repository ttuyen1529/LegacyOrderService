using LegacyOrderService.Core.Observability;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.ConsoleApp.Data.InMemory;
using OrderService.ConsoleApp.Data.SQLite;
using OrderService.ConsoleApp.Infrastructure.Persistence;
using OrderService.Core.Abstractions;
using OrderService.Core.Application.Commands;
using OrderService.Core.Domain.Pricing;
using OrderService.Core.Domain.Pricing.Strategies;
using Serilog;

namespace OrderService.ConsoleApp
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers application services for the console app.
        /// Keep Core free of infrastructure concerns; wire everything here.
        /// </summary>
        public static IServiceCollection AddOrderService(this IServiceCollection services, string connectionString)
        {
            //Register logging
            services.AddLogging(b =>
            {
                b.ClearProviders();
                b.AddSerilog(Log.Logger, dispose: true);
            });

            //Register MediatR handlers in Core
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(PlaceOrderCommand).Assembly));

            // Register pipeline behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // Pricing (Strategy)
            services.AddSingleton<IPricingService, PricingService>();
            // Register pricing strategies in order of precedence
            //services.AddSingleton<IPricingStrategy, BulkDiscountPricingStrategy>();  // optional
            services.AddSingleton<IPricingStrategy, DefaultPricingStrategy>();       // fallback LAST

            // Register infrastructure implementations
            services.AddSingleton<IProductCatalog, InMemoryProductCatalog>();
            services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
            services.AddSingleton<IOrderRepository, SqliteOrderRepository>();

            return services;
        }
    }

}
