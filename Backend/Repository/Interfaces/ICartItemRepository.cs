using Backend.Models;

namespace Backend.Repository.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetAllAsync();
        Task<CartItem?> GetByIdAsync(int id);
        Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId);
        Task<CartItem> AddAsync(CartItem cartItem);
        Task<CartItem?> UpdateAsync(int id, CartItem cartItem);
        Task<bool> DeleteAsync(int id);
    }
}
