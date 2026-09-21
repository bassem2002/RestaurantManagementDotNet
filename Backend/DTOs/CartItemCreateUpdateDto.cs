namespace Backend.DTOs
{
    public class CartItemCreateUpdateDto
    {
        public int CartId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
