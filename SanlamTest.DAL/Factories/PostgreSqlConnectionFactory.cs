using Microsoft.Extensions.Configuration;
using Npgsql;
using SanlamTest.DAL.Interfaces;
using System.Data;

namespace SanlamTest.DAL.Factories
{
    public class PostgreSqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public PostgreSqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SanlamDb");

            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new ArgumentNullException("PostgreSqlConnectionFactory connection string cannot be null or empty");
        }

        public IDbConnection CreateSanlamDbConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
