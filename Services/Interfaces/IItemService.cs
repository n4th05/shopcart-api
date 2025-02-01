using Npgsql;
using ShopCartAPI.Models;

namespace ShopCartAPI.Services.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllItensAsync();
        Task<Item> CreateItemAsync(Item item);
        Task UpdateItemAsync(int id, Item item);
        Task DeleteItemAsync(int produtoId);
    }
}
