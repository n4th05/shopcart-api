using Dapper;
using ShopCartAPI.Infrastructure.Data;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCartAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IProdutoService _produtoService;

        public ItemService(IDbConnectionFactory connectionFactory, IProdutoService produtoService)
        {
            _connectionFactory = connectionFactory;
            _produtoService = produtoService;
        }

        public async Task<IEnumerable<Item>> GetAllItensAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
        SELECT 
            i.""Id"" as Id, 
            i.""Qtd"" as Quantidade, 
            i.""UnidadeDeMedida"" as UnidadeDeMedida,
            i.""ProdutoId"" as ProdutoId,
            p.""Id"" as Produto_Id, -- Id do Produto
            p.""Nome"" as Nome
        FROM ""Itens"" i
        JOIN ""Produtos"" p ON i.""ProdutoId"" = p.""Id""";

            var result = await connection.QueryAsync<Item, Produto, Item>(
                sql,
                (item, produto) =>
                {
                    Console.WriteLine($"Produto Id: {produto.Id}, Nome: {produto.Nome}"); // Log
                    item.Produto = produto;
                    return item;
                },
                splitOn: "ProdutoId"
            );

            return result;
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            if (item.Produto == null)
                throw new ArgumentNullException(nameof(item.Produto), "Produto cannot be null");
            var produto = await _produtoService.GetProdutoByIdAsync(item.Produto.Id);
            if (produto == null)
                throw new KeyNotFoundException($"Produto with id {item.Produto.Id} not found");
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
        INSERT INTO ""Itens"" (""ProdutoId"", ""Qtd"", ""UnidadeDeMedida"")
        VALUES (@ProdutoId, @Quantidade, @UnidadeDeMedida)";
            await connection.ExecuteScalarAsync(sql, new
            {
                ProdutoId = item.Produto.Id,
                item.Quantidade,
                item.UnidadeDeMedida
            });
            item.Produto = produto;
            return item;
        }

        public async Task UpdateItemAsync(int id, Item item)
        {
            if (item.Produto == null)
                throw new ArgumentNullException(nameof(item.Produto), "Produto cannot be null");

            // Primeiro, busque o produto completo
            var produto = await _produtoService.GetProdutoByIdAsync(item.Produto.Id);
            if (produto == null)
                throw new KeyNotFoundException($"Produto with id {item.Produto.Id} not found");

            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
        UPDATE ""Itens"" 
        SET ""ProdutoId"" = @ProdutoId,
            ""Qtd"" = @Quantidade,
            ""UnidadeDeMedida"" = @UnidadeDeMedida
        WHERE ""Id"" = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                ProdutoId = item.Produto.Id,
                item.Quantidade,
                item.UnidadeDeMedida
            });

            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Item with ID {id} not found");
        }

        public async Task DeleteItemAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM \"Itens\" WHERE \"Id\" = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Item with ID {id} not found");
        }
    }
}