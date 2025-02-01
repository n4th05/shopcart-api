using ShopCartAPI.Models;

namespace ShopCartAPI.Services.Interfaces
{
    public interface ICarrinhoService
    {
        Task<IEnumerable<Carrinho>> GetAllCarrinhosAsync();
        Task<Carrinho> GetCarrinhoByIdAsync(int id, bool withItens);
        Task<Carrinho> CreateCarrinhoAsync(Carrinho carrinho);
        Task UpdateCarrinhoAsync(int id, Carrinho carrinho);
        Task DeleteCarrinhoAsync(int id);
    }
}