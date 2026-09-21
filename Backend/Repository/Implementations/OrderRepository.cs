using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        public readonly AppDbContext context;
        // ctor 

        public OrderRepository(AppDbContext context)
        {
            this.context = context; 
        }

        public async Task<Order> AddOrder(Order order)
        {
            // Use AddAsync for EF Core where supported, but ensure entity is added before saving
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
            return order; 
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await context.Orders.FindAsync(id);
            if (order == null) return false;
            context.Orders.Remove(order); 
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await context.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<List<Order>> GetOrders()
        {
            return await context.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ToListAsync(); 
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            var existingOrder = await context.Orders.FindAsync(order.OrderId);
            if(existingOrder==null) return false;

            // update only properties we allow
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.UserId = order.UserId;
            // handle order items/payment updates elsewhere

            context.Orders.Update(existingOrder);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Order>> GetOrdersByUser(string userId)
        {
            return await context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

    }
}
