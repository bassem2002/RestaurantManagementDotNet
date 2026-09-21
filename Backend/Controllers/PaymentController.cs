using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepo;

        public PaymentController(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentRepo.GetAllPayments();
            return Ok(payments);
        }

        // GET: api/payment/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentRepo.GetPaymentById(id);

            if (payment == null)
                return NotFound($"Aucun paiement trouvé avec l'id {id}");

            return Ok(payment);
        }

        // POST: api/payment
        [HttpPost]
        public async Task<IActionResult> AddPayment(Payment payment)
        {
            var newPayment = await _paymentRepo.AddPayment(payment);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { id = newPayment.PaymentId },
                newPayment
            );
        }

        // PUT: api/payment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, Payment payment)
        {
            if (id != payment.PaymentId)
                return BadRequest("L'ID dans l'URL ne correspond pas à l'objet Payment");

            var updated = await _paymentRepo.UpdatePayment(payment);

            if (!updated)
                return NotFound($"Aucun paiement trouvé avec l'id {id}");

            return Ok("Paiement mis à jour avec succès");
        }

        // DELETE: api/payment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var deleted = await _paymentRepo.DeletePayment(id);

            if (!deleted)
                return NotFound($"Aucun paiement trouvé avec l'id {id}");

            return Ok("Paiement supprimé avec succès");
        }
    }
}
