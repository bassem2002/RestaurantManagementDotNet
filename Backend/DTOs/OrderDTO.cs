using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [JsonPropertyName("UserId")]

        public string UserId { get; set; }
        public string Username { get; set; }

        // ✅ Statut de la commande
        public string Status { get; set; }

        // ✅ Nouveaux champs pour la livraison
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        
        // Ajouter TotalAmount calculé
        public decimal TotalAmount => OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
        public List<OrderItemDTO> OrderItems { get; set; } = new();
    }
}
