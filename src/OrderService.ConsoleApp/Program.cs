using OrderService.Core.Application.Commands;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Logging;
using OrderService.Core.Application.Query;


namespace OrderService.ConsoleApp;

internal static class Program
{
    private const int MaxRetries = 3;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(1);

    static async Task Main(string[] args)
    {
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            var services = new ServiceCollection();

            // App + Core registrations
            var relativeDbPath = configuration.GetConnectionString("OrdersDb"); // "App_data/orders.db"
            var fullDbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativeDbPath));
            services.AddOrderService(
                $"Data Source={fullDbPath}"
            );

            using var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILoggerFactory>()
                           .CreateLogger("Program");
            var mediator = sp.GetRequiredService<IMediator>();

            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine("Welcome to Order Processor!");

                    var customer = ReadRequired("Enter customer name:");
                    var productName = ReadRequired("Enter product name:");
                    var product = await mediator.Send(
                        new GetProductByNameQuery(productName)
                    );
                    var quantity = ReadInt("Enter quantity:", min: 1);
                    var order = await mediator.Send(
                        new PlaceOrderCommand(customer, product, quantity)
                    );
                    Console.WriteLine("Order complete!");
                    Console.WriteLine($"Customer: {order.CustomerName}");
                    Console.WriteLine($"Product: {order.ProductName}");
                    Console.WriteLine($"Quantity: {order.Quantity}");
                    Console.WriteLine($"Total: ${order.Price}");

                    Console.WriteLine("Saving order to database...");
                    await mediator.Send(new SaveOrderCommand(order));
                    Console.WriteLine("Done.");

                    logger.LogInformation(
                        "Order processed successfully."
                    );

                    return; // success → exit app
                }
                catch (SqliteException ex) when (IsFatalDbError(ex))
                {
                    logger.LogCritical(ex, "Fatal database error. Exiting.");
                    Console.Error.WriteLine("Fatal database error. See logs.");
                    Environment.ExitCode = 2;
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Error occurred (attempt {Attempt}/{MaxRetries})",
                        attempt,
                        MaxRetries
                    );

                    Console.Error.WriteLine($"Error: {ex.Message}");

                    if (attempt == MaxRetries)
                    {
                        Console.Error.WriteLine("Max retries reached. Exiting.");
                        Environment.ExitCode = 1;
                        return;
                    }

                    Console.WriteLine($"Retrying in {RetryDelay.TotalSeconds:0}s...\n");
                    await Task.Delay(RetryDelay);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled fatal exception");
            Environment.ExitCode = -1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // -------------------------------------------------
    // Helpers (App-level, no business logic)
    // -------------------------------------------------
    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.Error.WriteLine("Input is required.");
        }
    }

    private static int ReadInt(string prompt, int min)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();

            if (int.TryParse(input, out var value) && value >= min)
                return value;

            Console.Error.WriteLine($"Please enter a valid integer >= {min}.");
        }
    }

    private static bool IsFatalDbError(SqliteException ex)
    {
        var msg = ex.Message.ToLowerInvariant();
        return msg.Contains("no such table")
            || msg.Contains("unable to open database file")
            || msg.Contains("readonly")
            || msg.Contains("disk i/o error");
    }
}
