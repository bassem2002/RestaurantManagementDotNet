using Backend.Models;

namespace Backend.Repository.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(int id);
        Task<Item> AddAsync(Item item);
        Task<Item> UpdateAsync(Item item);
        Task<bool> DeleteAsync(int id);
    }
}
