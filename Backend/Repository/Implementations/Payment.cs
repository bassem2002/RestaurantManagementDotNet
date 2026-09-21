using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllPayments()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<Payment?> GetPaymentById(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<Payment> AddPayment(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> UpdatePayment(Payment payment)
        {
            var existingPayment = await _context.Payments.FindAsync(payment.PaymentId);

            if (existingPayment == null)
                return false;

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
