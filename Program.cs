using Npgsql;
using Dapper;
using ShopCartAPI.Infrastructure.Data;
using ShopCartAPI.Services;
using ShopCartAPI.Models;
using System.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Definir política de CORS
var corsPolicy = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// Configurações de serviços
builder.Services.AddControllers();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();

// Configurar conexão do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDbConnectionFactory>(new PostgresConnectionFactory(connectionString));

// Injeção de dependência dos serviços
builder.Services.AddScoped<IProdutoService, ProdutoService>();

var app = builder.Build();


// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
    using var connection = connectionFactory.CreateConnection();
    connection.Open();

    const string createTablesScript = @"
        CREATE TABLE IF NOT EXISTS ""Produtos"" (
            ""Id"" SERIAL PRIMARY KEY,
            ""Nome"" VARCHAR(255) NOT NULL
        );
        CREATE TABLE IF NOT EXISTS ""Itens"" (
            ""Id"" SERIAL PRIMARY KEY,
            ""ProdutoId"" INT NOT NULL,
            ""Qtd"" INT NOT NULL,
            ""UnidadeDeMedida"" VARCHAR(255) NOT NULL,
            FOREIGN KEY (""ProdutoId"") REFERENCES ""Produtos""(""Id"")
        );
        CREATE TABLE IF NOT EXISTS ""Carrinhos"" (
            ""Id"" SERIAL PRIMARY KEY
        );
    ";

    await connection.ExecuteAsync(createTablesScript);
}

app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();