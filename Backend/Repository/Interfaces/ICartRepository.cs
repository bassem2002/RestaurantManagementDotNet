using Backend.Models;

namespace Backend.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart> CreateCartAsync(string userId);
        Task<bool> CartExistsAsync(string userId);

        Task<Cart> AddItemAsync(string userId, int itemId, int quantity);
        Task<bool> RemoveItemAsync(string userId, int itemId);
        Task<bool> ClearCartAsync(string userId);

        Task<List<Cart>> GetAllCartsAsync(); // Admin
    }
}
