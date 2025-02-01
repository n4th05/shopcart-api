using ShopCartAPI.Models;

namespace ShopCartAPI.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> GetAllProdutosAsync();
        Task<Produto> GetProdutoByIdAsync(int id);
        Task<Produto> CreateProdutoAsync(Produto produto);
        Task UpdateProdutoAsync(int id, Produto produto);
        Task DeleteProdutoAsync(int id);
    }
}