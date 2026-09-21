using Backend.Models;

namespace Backend.Repository.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetOrderItems();

        Task<OrderItem> AddOrderItem(OrderItem orderItem);

        Task<OrderItem> GetOrderItemById(int id);

        Task<bool> UpdateOrderItem(OrderItem orderItem);  

        Task<bool> DeleteOrderItem(int id); 


    }
}
