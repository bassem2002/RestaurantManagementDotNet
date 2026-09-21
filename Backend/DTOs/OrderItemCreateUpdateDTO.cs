namespace Backend.DTOs
{
    public class OrderItemCreateUpdateDTO
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }


    }
}
