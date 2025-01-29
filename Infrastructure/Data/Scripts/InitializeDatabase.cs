using Dapper;
using System.Data;

namespace ShopCartAPI.Infrastructure 
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabase(IDbConnection connection)
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
                )";

            await connection.ExecuteAsync(createTablesScript);
            }
        }
    }
