using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            return await _context.CartItems
                .Include(ci => ci.Item)
                .Include(ci => ci.Cart)
                .ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(int id)
        {
            return await _context.CartItems
                .Include(ci => ci.Item)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == id);
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Item)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        public async Task<CartItem> AddAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem?> UpdateAsync(int id, CartItem cartItem)
        {
            var existing = await _context.CartItems.FindAsync(id);

            if (existing == null)
                return null;

            existing.CartId = cartItem.CartId;
            existing.ItemId = cartItem.ItemId;
            existing.Quantity = cartItem.Quantity;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.CartItems.FindAsync(id);

            if (existing == null)
                return false;

            _context.CartItems.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
