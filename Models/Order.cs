using Backend.Models;
using System.ComponentModel.DataAnnotations;

public class Order
{
    [Key]
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public string Status { get; set; } = "Pending"; // ✅ ICI

    public virtual Payment Payment { get; set; }
    public virtual List<OrderItem> OrderItems { get; set; } = new();
    // 🔴 NOUVELLES PROPRIÉTÉS
    public string ShippingAddress { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Phone { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
}
