namespace Front.Models
{
    public class AdminOrderDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }

        public List<AdminOrderItemDTO> OrderItems { get; set; } = new();
    }
    public class AdminOrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }
        public string Username { get; set; }

        public int ItemId { get; set; }

        public DateTime OrderDate { get; set; }
        public ItemDto Item { get; set; }

        public OrderDTO Order { get; set; }


    }
}
