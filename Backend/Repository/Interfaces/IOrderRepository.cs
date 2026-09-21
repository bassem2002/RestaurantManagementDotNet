using Backend.Models;

namespace Backend.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrders();

        Task<Order> GetOrderById(int id);

        Task<Order> AddOrder(Order order);

        Task<bool> UpdateOrder(Order order);

        Task<bool> DeleteOrder(int id);
        Task<List<Order>> GetOrdersByUser(string userId);
    }
}
