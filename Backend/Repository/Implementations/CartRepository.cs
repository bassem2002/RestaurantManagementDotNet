// Backend/Repository/Implementations/CartRepository.cs
using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repository.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var existing = await GetCartByUserIdAsync(userId);
            if (existing != null) return existing;

            var cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> AddItemAsync(string userId, int itemId, int quantity)
        {
            if (quantity <= 0) quantity = 1;

            var cart = await GetCartByUserIdAsync(userId) ?? await CreateCartAsync(userId);

            // Check that the item exists to avoid FK constraint errors
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
            {
                throw new KeyNotFoundException($"Item with id {itemId} not found.");
            }

            // Ensure items navigation is loaded
            await _context.Entry(cart).Collection(c => c.Items).LoadAsync();

            // Try find existing CartItem
            var existingItem = cart.Items.FirstOrDefault(ci => ci.ItemId == itemId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ItemId = itemId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
                cart.Items.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Return fresh cart with items
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(c => c.CartId == cart.CartId);
        }

        public async Task<bool> RemoveItemAsync(string userId, int itemId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            await _context.Entry(cart).Collection(c => c.Items).LoadAsync();

            var existingItem = cart.Items.FirstOrDefault(ci => ci.ItemId == itemId);
            if (existingItem == null) return false;

            _context.CartItems.Remove(existingItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return false;
            _context.CartItems.RemoveRange(cart.Items);
            cart.Items.Clear();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Cart>> GetAllCartsAsync()
        {
            return await _context.Carts.Include(c => c.Items).ToListAsync();
        }

        public Task<bool> CartExistsAsync(string userId)
        {
            return _context.Carts.AnyAsync(c => c.UserId == userId);
        }
    }
}
