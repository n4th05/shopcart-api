using Dapper;
using ShopCartAPI.Infrastructure.Data;
using System.Data;

namespace ShopCartAPI.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IItemService _itemService;

    public CarrinhoService(IDbConnectionFactory connectionFactory, IItemService itemService)
    {
        _connectionFactory = connectionFactory;
        _itemService = itemService;
    }

    public async Task<IEnumerable<Carrinho>> GetAllCarrinhosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var carrinhos = (await connection.QueryAsync<Carrinho>(@"
        SELECT ""Id"" FROM ""Carrinhos""")).ToList();

        foreach (var carrinho in carrinhos)
        {
            var itens = await connection.QueryAsync<dynamic>(@"
            SELECT 
                i.""Id"", 
                i.""Qtd"" as Quantidade, 
                i.""UnidadeDeMedida"",
                p.""Id"" as ProdutoId, 
                p.""Nome"" as ProdutoNome
            FROM ""Itens"" i
            JOIN ""Produtos"" p ON i.""ProdutoId"" = p.""Id""
            JOIN ""CarrinhoItens"" ci ON i.""Id"" = ci.""ItemId""
            WHERE ci.""CarrinhoId"" = @CarrinhoId",
                new { CarrinhoId = carrinho.Id });

            carrinho.ItensCarrinho = itens.Select(item => new Item
            {
                Id = item.Id,
                Quantidade = item.Quantidade,
                UnidadeDeMedida = item.UnidadeDeMedida,
                Produto = new Produto
                {
                    Id = item.ProdutoId,
                    Nome = item.ProdutoNome
                }
            }).ToList();
        }

        return carrinhos;
    }

    public async Task<Carrinho> GetCarrinhoByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var carrinho = await connection.QuerySingleOrDefaultAsync<Carrinho>(
             "SELECT \"Id\" FROM \"Carrinhos\" WHERE \"Id\" = @Id",
            new { Id = id }
        );

        if (carrinho == null)
            return null;

        var itens = await connection.QueryAsync<dynamic>(@"
        SELECT 
            i.""Id"", 
            i.""Qtd"" as Quantidade, 
            i.""UnidadeDeMedida"",
            p.""Id"" as ProdutoId, 
            p.""Nome"" as ProdutoNome
        FROM ""Itens"" i
        JOIN ""Produtos"" p ON i.""ProdutoId"" = p.""Id""
        JOIN ""CarrinhoItens"" ci ON i.""Id"" = ci.""ItemId""
        WHERE ci.""CarrinhoId"" = @CarrinhoId",
            new { CarrinhoId = id });

        carrinho.ItensCarrinho = itens.Select(item => new Item
        {
            Id = item.Id,
            Quantidade = item.Quantidade,
            UnidadeDeMedida = item.UnidadeDeMedida,
            Produto = new Produto
            {
                Id = item.ProdutoId,
                Nome = item.ProdutoNome
            }
        }).ToList();

        return carrinho;
    }

    public async Task<Carrinho> CreateCarrinhoAsync(Carrinho carrinho)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            var carrinhoId = await connection.ExecuteScalarAsync<int>(
               "INSERT INTO \"Carrinhos\" DEFAULT VALUES RETURNING \"Id\"",
                transaction: transaction
            );

            carrinho.Id = carrinhoId;

            if (carrinho.ItensCarrinho?.Any() == true)
            {
                foreach (var item in carrinho.ItensCarrinho)
                {
                    var savedItem = await _itemService.CreateItemAsync(item);

                    await connection.ExecuteAsync(
                        "INSERT INTO \"CarrinhoItens\" (\"CarrinhoId\", \"ItemId\") VALUES (@CarrinhoId, @ItemId)",
                        new { CarrinhoId = carrinhoId, ItemId = savedItem.Id },
                        transaction
                    );
                }
            }

            transaction.Commit();
            return carrinho;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateCarrinhoAsync(int id, Carrinho carrinho)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            var existingCarrinho = await connection.QuerySingleOrDefaultAsync<Carrinho>(
                "SELECT \"Id\" FROM \"Carrinhos\" WHERE \"Id\" = @Id",
                new { Id = id }
            );

            if (existingCarrinho == null)
                throw new KeyNotFoundException($"Carrinho with id {id} not found");

            await connection.ExecuteAsync(
                "DELETE FROM \"CarrinhoItens\" WHERE \"CarrinhoId\" = @CarrinhoId",
                new { CarrinhoId = id },
                transaction
            );

            if (carrinho.ItensCarrinho?.Any() == true)
            {
                foreach (var item in carrinho.ItensCarrinho)
                {
                    var savedItem = await _itemService.CreateItemAsync(item);

                    await connection.ExecuteAsync(
                        "INSERT INTO \"CarrinhoItens\" (\"CarrinhoId\", \"ItemId\") VALUES (@CarrinhoId, @ItemId)",
                        new { CarrinhoId = id, ItemId = savedItem.Id },
                        transaction
                    );
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task DeleteCarrinhoAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(
                "DELETE FROM \"CarrinhoItens\" WHERE \"CarrinhoId\" = @Id",
                new { Id = id },
                transaction
            );

            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM \"Carrinhos\" WHERE \"Id\" = @Id",
                new { Id = id },
                transaction
            );

            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Carrinho with id {id} not found");

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
