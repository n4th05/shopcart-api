using ShopCartAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopCartAPI.Services
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllItensAsync();
        Task<Item> CreateItemAsync(Item item);
        Task UpdateItemAsync(int id, Item item);
        Task DeleteItemAsync(int produtoId);
    }
}
