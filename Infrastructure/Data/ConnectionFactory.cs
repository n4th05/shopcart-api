using Npgsql;
using System.Data;

namespace ShopCartAPI.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        NpgsqlConnection CreateConnection(); // Retorna NpgsqlConnection em vez de IDbConnection
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}