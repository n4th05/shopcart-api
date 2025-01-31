namespace ShopCartAPI.Services
{
    public interface ICarrinhoService
    {
        Task<IEnumerable<Carrinho>> GetAllCarrinhosAsync();
        Task<Carrinho> GetCarrinhoByIdAsync(int id);
        Task<Carrinho> CreateCarrinhoAsync(Carrinho carrinho);
        Task UpdateCarrinhoAsync(int id, Carrinho carrinho);
        Task DeleteCarrinhoAsync(int id);
    }
}