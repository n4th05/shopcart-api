using System.Data;
using Dapper;
using ShopCartAPI.Infrastructure.Data; 

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços de controle, como controllers e autorização
builder.Services.AddControllers();

// Adiciona a configuração de autorização
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton(sp =>
    new PostgresConnectionFactory(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


var app = builder.Build();

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


// Inicialização do banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var connectionFactory = services.GetRequiredService<PostgresConnectionFactory>();
        using var connection = connectionFactory.CreateConnection();

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Iniciando a criação das tabelas.");

        await InitializeDatabase(connection);

        logger.LogInformation("Tabelas criadas com sucesso.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
        throw;
    }
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static async Task InitializeDatabase(IDbConnection connection)
{
    const string createTablesScript = @"
        CREATE TABLE IF NOT EXISTS Produtos (
            Id SERIAL PRIMARY KEY,
            Nome VARCHAR(255) NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Itens (
            Id SERIAL PRIMARY KEY,
            ProdutoId INT NOT NULL,
            Qtd INT NOT NULL,
            UnidadeDeMedida VARCHAR(255) NOT NULL,
            FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
        );

        CREATE TABLE IF NOT EXISTS Carrinhos (
            Id SERIAL PRIMARY KEY,
            Identificador INT NOT NULL
        );
    ";

    await connection.ExecuteAsync(createTablesScript);
}
