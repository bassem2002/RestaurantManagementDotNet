using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implementations
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> AddOrderItem(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            
            // Reload the created OrderItem with all navigation properties
            var createdOrderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.User)
                .Include(oi => oi.Item)
                .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItem.OrderItemId);
            
            return createdOrderItem;
        }

        public async Task<bool> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return false;
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrderItem> GetOrderItemById(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.User)
                .Include(oi => oi.Item)
                .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
        }

        public async Task<List<OrderItem>> GetOrderItems()
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.User)
                .Include(oi => oi.Item)
                .ThenInclude(i => i.Category)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderItem(OrderItem orderItem)
        {
            var existingOrderItem = await _context.OrderItems.FindAsync(orderItem.OrderItemId);
            if (existingOrderItem == null) return false;
            
            existingOrderItem.Quantity = orderItem.Quantity;
            existingOrderItem.UnitPrice = orderItem.UnitPrice;
            
            _context.OrderItems.Update(existingOrderItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
