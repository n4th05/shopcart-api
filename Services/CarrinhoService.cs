using Dapper;
using ShopCartAPI.Infrastructure.Data;
using System.Data;
using ShopCartAPI.Services.Interfaces;
using ShopCartAPI.Models;

namespace ShopCartAPI.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Lazy<IItemService> _itemService;

    public CarrinhoService(
        IDbConnectionFactory connectionFactory,
        Lazy<IItemService> itemService)
    {
        _connectionFactory = connectionFactory;
        _itemService = itemService;
    }

    public async Task<IEnumerable<Carrinho>> GetAllCarrinhosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var carrinhos = (await connection.QueryAsync<Carrinho>(@"SELECT ""Id"" FROM ""Carrinhos""")).ToList();

        foreach (var carrinho in carrinhos)
        {
            var itens = await connection.QueryAsync<dynamic>(@"
                SELECT 
                    i.""Id"" as ""Id"", 
                    i.""Qtd"" as ""Quantidade"", 
                    i.""UnidadeDeMedida"" as ""UnidadeDeMedida"",
                    p.""Id"" as ""ProdutoId"", 
                    p.""Nome"" as ""ProdutoNome""
                FROM ""Itens"" i
                JOIN ""Produtos"" p ON i.""ProdutoId"" = p.""Id""
                WHERE i.""CarrinhoId"" = @CarrinhoId",
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

    public async Task<Carrinho> GetCarrinhoByIdAsync(int id, bool withItens)
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
                i.""Id"" as ""Id"", 
                i.""Qtd"" as ""Quantidade"", 
                i.""UnidadeDeMedida"" as ""UnidadeDeMedida"",
                p.""Id"" as ""ProdutoId"", 
                p.""Nome"" as ""ProdutoNome""
                FROM ""Itens"" i
                JOIN ""Produtos"" p ON i.""ProdutoId"" = p.""Id""
                WHERE i.""CarrinhoId"" = @CarrinhoId",
        new { CarrinhoId = id });

        if (withItens)
        {
            carrinho.ItensCarrinho = itens.Select(item => new Item
            {
                Id = item.Id,
                Quantidade = item.Quantidade,
                UnidadeDeMedida = item.UnidadeDeMedida,
                Produto = new Produto
                {
                    Id = item.ProdutoId,
                    Nome = item.ProdutoNome
                },
                Carrinho = new Carrinho { Id = id }
            }).ToList();
        }

        return carrinho;
    }

public async Task<Carrinho> CreateCarrinhoAsync(Carrinho carrinho)
{
    using var connection = _connectionFactory.CreateConnection();
    try
    {
        var carrinhoId = await connection.ExecuteScalarAsync<int>("INSERT INTO \"Carrinhos\" DEFAULT VALUES RETURNING \"Id\"");
        carrinho.Id = carrinhoId;

        if (carrinho.ItensCarrinho?.Any() == true)
        {
            foreach (var item in carrinho.ItensCarrinho)
            {
                item.Carrinho = new Carrinho
                {
                    Id = carrinho.Id
                };
                var savedItem = await _itemService.Value.CreateItemAsync(item);
            }
        }

        return carrinho;
    }
    catch
    {
        throw;
    }
}

    public async Task UpdateCarrinhoAsync(int id, Carrinho carrinho)
    {
        using var connection = _connectionFactory.CreateConnection();

        try
        {
            var existingCarrinho = await connection.QuerySingleOrDefaultAsync<Carrinho>(
                "SELECT \"Id\" FROM \"Carrinhos\" WHERE \"Id\" = @Id",
                new { Id = id }
            );

            if (existingCarrinho == null)
                throw new KeyNotFoundException($"Carrinho with id {id} not found");

            await connection.ExecuteAsync(
                "DELETE FROM \"Itens\" WHERE \"CarrinhoId\" = @CarrinhoId",
                new { CarrinhoId = id }
            );

            if (carrinho.ItensCarrinho?.Any() == true)
            {
                foreach (var item in carrinho.ItensCarrinho)
                {
                    item.Carrinho = new Carrinho
                    {
                        Id = id
                    };
                    var savedItem = await _itemService.Value.CreateItemAsync(item);
                }
            }

        }
        catch
        {
            throw;
        }
    }

    public async Task DeleteCarrinhoAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        try
        {
            await connection.ExecuteAsync(
                "DELETE FROM \"Itens\" WHERE \"CarrinhoId\" = @Id",
                new { Id = id }
            );

            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM \"Carrinhos\" WHERE \"Id\" = @Id",
                new { Id = id }
            );

            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Carrinho with id {id} not found");

        }
        catch
        {
            throw;
        }
    }
}
