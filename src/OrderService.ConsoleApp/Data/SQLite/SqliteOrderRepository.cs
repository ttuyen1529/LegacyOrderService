using OrderService.Core.Abstractions;
using OrderService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.ConsoleApp.Data.SQLite
{
    public sealed class SqliteOrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public SqliteOrderRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task SaveAsync(Order order, CancellationToken ct = default)
        {
            await using var connection = await _dbConnectionFactory.CreateOpenConnectionAsync(ct);
            await using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Orders (CustomerName, ProductName, Quantity, Price)
                VALUES (@CustomerName, @ProductName, @Quantity, @Price);
            ";
            AddParam(command, "@CustomerName", order.CustomerName);
            AddParam(command, "@ProductName", order.ProductName);
            AddParam(command, "@Quantity", order.Quantity);
            AddParam(command, "@Price", order.Price);
            await command.ExecuteNonQueryAsync(ct);
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }
    }
}
