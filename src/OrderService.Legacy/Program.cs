using System;
using System.Threading.Tasks;
using LegacyOrderService.Models;
using LegacyOrderService.Data;
using Microsoft.Data.Sqlite;

namespace LegacyOrderService
{
    class Program
    {
        // Retry configuration
        private const int MaxRetries = 3;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

        static async Task Main(string[] args)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine("Welcome to Order Processor!");

                    var name = ReadRequired("Enter customer name:");
                    var product = ReadRequired("Enter product name:");

                    var productRepo = new ProductRepository();
                    var price = productRepo.GetPrice(product);
                    var qty = ReadInt("Enter quantity:", min: 1);
                    Console.WriteLine("Processing order...");

                    var order = new Order
                    {
                        CustomerName = name,
                        ProductName = product,
                        Quantity = qty,
                        Price = price
                    };

                    var total = order.Quantity * order.Price;
                    Console.WriteLine("Order complete!");
                    Console.WriteLine("Customer: " + order.CustomerName);
                    Console.WriteLine("Product: " + order.ProductName);
                    Console.WriteLine("Quantity: " + order.Quantity);
                    Console.WriteLine("Total: $" + total);

                    Console.WriteLine("Saving order to database...");
                    var repo = new OrderRepository();
                    repo.Save(order);
                    Console.WriteLine("Done.");
                    return; // success
                }
                catch (SqliteException ex) when (IsFatalDbError(ex))
                {
                    // Fail-fast: kind of error cannot be recovered by retrying
                    Console.Error.WriteLine("Fatal database error: " + ex.Message);
                    Environment.ExitCode = 2;
                    return;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred (attempt {attempt}/{MaxRetries}): {ex.Message}");

                    if (attempt == MaxRetries)
                    {
                        Console.Error.WriteLine("Max retries reached. Exiting.");
                        Environment.ExitCode = 1;
                        return;
                    }

                    Console.WriteLine($"Retrying in {RetryDelay.TotalSeconds:0} second(s)...");
                    await Task.Delay(RetryDelay);
                    Console.WriteLine("Let's try again.\n");
                }
            }
        }

        private static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                Console.Error.WriteLine("Input is required. Please try again.");
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
            // Common non-recoverable SQLite issues in this assessment context:
            // - missing table/schema
            // - cannot open database file
            // - read-only or permission issues
            var msg = ex.Message?.ToLowerInvariant() ?? "";
            return msg.Contains("no such table")
                || msg.Contains("unable to open database file")
                || msg.Contains("readonly")
                || msg.Contains("disk i/o error");
        }
    }
}
