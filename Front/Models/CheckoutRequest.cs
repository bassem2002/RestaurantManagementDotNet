namespace Front.Models
{
    public class CheckoutRequest
    {
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CheckoutItemRequest> Items { get; set; } = new();

    }
}
