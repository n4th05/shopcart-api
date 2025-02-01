using Dapper;
using Npgsql;
using ShopCartAPI.Infrastructure.Data;
using ShopCartAPI.Models;
using ShopCartAPI.Services.Interfaces;

namespace ShopCartAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IProdutoService _produtoService;
        private readonly Lazy<ICarrinhoService> _carrinhoService;

        public ItemService(
            IDbConnectionFactory connectionFactory,
            IProdutoService produtoService,
            Lazy<ICarrinhoService> carrinhoService)
        {
            _connectionFactory = connectionFactory;
            _produtoService = produtoService;
            _carrinhoService = carrinhoService;
        }

        public async Task<IEnumerable<Item>> GetAllItensAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
        SELECT 
            i.""Id"" AS ""Id"", -- Item's Id
            i.""Qtd"" AS ""Quantidade"",
            i.""UnidadeDeMedida"",
            p.""Id"" AS ""ProdutoId"", -- Flattened alias for Produto.Id
            p.""Nome"" AS ""ProdutoNome"", -- Flattened alias for Produto.Nome
            c.""Id"" AS ""CarrinhoId"" -- Flattened alias for Carrinho.Id
        FROM ""Itens"" i
        JOIN ""Produtos"" p ON i.""ProdutoId"" = p.""Id""
        JOIN ""Carrinhos"" c ON i.""CarrinhoId"" = c.""Id""
    ";

            var result = await connection.QueryAsync<Item, dynamic, dynamic, Item>(
                sql,
                (item, produtoData, carrinhoData) =>
                {
                    // Map Produto
                    item.Produto = new Produto
                    {
                        Id = produtoData.ProdutoId,
                        Nome = produtoData.ProdutoNome
                    };

                    // Map Carrinho
                    item.Carrinho = new Carrinho
                    {
                        Id = carrinhoData.CarrinhoId,
                        ItensCarrinho = new List<Item>() // Initialize as empty list
                    };

                    return item;
                },
                splitOn: "ProdutoId,CarrinhoId" // Split on flattened aliases
            );

            return result;
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            if (item.Produto == null)
                throw new ArgumentNullException(nameof(item.Produto), "Produto cannot be null");
            if (item.Carrinho == null)
                throw new ArgumentNullException(nameof(item.Carrinho), "Carrinho cannot be null");

            // Verifica se o Produto existe
            var produto = await _produtoService.GetProdutoByIdAsync(item.Produto.Id);
            if (produto == null)
                throw new KeyNotFoundException($"Produto with id {item.Produto.Id} not found");

            // Verifica se o Carrinho existe
            var carrinho = await _carrinhoService.Value.GetCarrinhoByIdAsync(item.Carrinho.Id, false);
            if (carrinho == null)
                throw new KeyNotFoundException($"Carrinho with id {item.Carrinho.Id} not found");

            using var connection = _connectionFactory.CreateConnection();

            // Insere o item
            const string sql = @"
                INSERT INTO ""Itens"" 
                    (""ProdutoId"", ""CarrinhoId"", ""Qtd"", ""UnidadeDeMedida"")
                    VALUES (@ProdutoId, @CarrinhoId, @Quantidade, @UnidadeDeMedida)
                RETURNING ""Id"";";

            var newId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                ProdutoId = item.Produto.Id,
                CarrinhoId = item.Carrinho.Id,
                item.Quantidade,
                item.UnidadeDeMedida
            });

            item.Id = newId;
            item.Produto = produto;
            item.Carrinho = carrinho;

            return item;
        }

        public async Task UpdateItemAsync(int id, Item item)
        {
            if (item.Produto == null)
                throw new ArgumentNullException(nameof(item.Produto), "Produto cannot be null");
            if (item.Carrinho == null)
                throw new ArgumentNullException(nameof(item.Carrinho), "Carrinho cannot be null");

            // Verifica se o Produto existe
            var produto = await _produtoService.GetProdutoByIdAsync(item.Produto.Id);
            if (produto == null)
                throw new KeyNotFoundException($"Produto with id {item.Produto.Id} not found");

            // Verifica se o Carrinho existe
            var carrinho = await _carrinhoService.Value.GetCarrinhoByIdAsync(item.Carrinho.Id, false);
            if (carrinho == null)
                throw new KeyNotFoundException($"Carrinho with id {item.Carrinho.Id} not found");

            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE ""Itens"" SET
                    ""ProdutoId"" = @ProdutoId,
                    ""CarrinhoId"" = @CarrinhoId,
                    ""Qtd"" = @Quantidade,
                    ""UnidadeDeMedida"" = @UnidadeDeMedida
                WHERE ""Id"" = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                ProdutoId = item.Produto.Id,
                CarrinhoId = item.Carrinho.Id,
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