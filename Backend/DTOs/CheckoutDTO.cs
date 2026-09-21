namespace Backend.DTOs
{
    public class CheckoutDTO
    {
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemCreateUpdateDTO> Items { get; set; }= new();
    }

}
