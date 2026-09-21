using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }  // Visa, Cash, etc.

        public bool IsPaid { get; set; }

        // FK Order (1 → 1)
        
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }

}
