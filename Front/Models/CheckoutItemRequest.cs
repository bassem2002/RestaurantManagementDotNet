namespace Front.Models
{
    public class CheckoutItemRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
