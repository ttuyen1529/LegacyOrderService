using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.ConsoleApp.Infrastructure.Persistence
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async ValueTask<DbConnection> CreateOpenConnectionAsync(CancellationToken ct = default)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);
            return connection;
        }
    }
}
