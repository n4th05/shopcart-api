using Dapper;
using ShopCartAPI.Infrastructure.Data;
using ShopCartAPI.Services;
using ShopCartAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Definir pol�tica de CORS
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

builder.Services.AddControllers();

// Configura��es Singleton
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();


// Configura��es de servi�os
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
// Depend�ncias circulares
builder.Services.AddScoped(provider => new Lazy<ICarrinhoService>(provider.GetRequiredService<ICarrinhoService>));
builder.Services.AddScoped(provider => new Lazy<IItemService>(provider.GetRequiredService<IItemService>));

// Configurar conex�o do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var app = builder.Build();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
    using var connection = connectionFactory.CreateConnection();
    connection.Open();

    Console.WriteLine($"Database connection test: {connection.State}");

    const string createTablesScript = @"
    CREATE TABLE IF NOT EXISTS ""Produtos"" (
        ""Id"" SERIAL PRIMARY KEY,
        ""Nome"" VARCHAR(255) NOT NULL
    );
    CREATE TABLE IF NOT EXISTS ""Carrinhos"" (
        ""Id"" SERIAL PRIMARY KEY
    );
    CREATE TABLE IF NOT EXISTS ""Itens"" (
        ""Id"" SERIAL PRIMARY KEY,
        ""ProdutoId"" INT NOT NULL,
        ""CarrinhoId"" INT NOT NULL,
        ""Qtd"" INT NOT NULL,
        ""UnidadeDeMedida"" VARCHAR(255) NOT NULL,

        FOREIGN KEY (""ProdutoId"") REFERENCES ""Produtos""(""Id""),
        FOREIGN KEY (""CarrinhoId"") REFERENCES ""Carrinhos""(""Id"")
    );";

    await connection.ExecuteAsync(createTablesScript);
}

app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthorization();
app.MapControllers();
app.Run();