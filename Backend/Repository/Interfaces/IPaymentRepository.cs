using Backend.Models;

namespace Backend.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task <List<Payment >> GetAllPayments();

        Task<Payment?> GetPaymentById(int id);

        Task<Payment> AddPayment(Payment payment);

        Task<bool> UpdatePayment(Payment payment);

        Task<bool> DeletePayment(int id);
    }
}
