namespace Backend.DTOs
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }

        public int ItemId { get; set; }

        // ✅ Ajouter le nom de l'article
        public string ItemName { get; set; }

        public DateTime OrderDate { get; set; } 
        public ItemDTO Item { get; set; }

        public OrderDTO Order { get; set; }
    }
}
