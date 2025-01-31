using Dapper;
using ShopCartAPI.Infrastructure.Data;
using System.Data;

namespace ShopCartAPI.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProdutoService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Produto>> GetAllProdutosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Produto>("SELECT * FROM \"Produtos\"");
        }

        public async Task<Produto> GetProdutoByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM \"Produtos\" WHERE \"Id\" = @Id";
            return await connection.QueryFirstOrDefaultAsync<Produto>(sql, new { Id = id });
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(
                "INSERT INTO \"Produtos\" (\"Nome\") VALUES (@Nome) RETURNING \"Id\"",
                produto
            );
            produto.Id = id;
            return produto;
        }

        public async Task UpdateProdutoAsync(int id, Produto produto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(
                "UPDATE \"Produtos\" SET \"Nome\" = @Nome WHERE \"Id\" = @Id",
                new { Id = id, produto.Nome }
            );
            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Produto with id {id} not found");
        }

        public async Task DeleteProdutoAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM \"Produtos\" WHERE \"Id\" = @Id",
                new { Id = id }
            );
            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Produto with id {id} not found");
        }
    }
}
