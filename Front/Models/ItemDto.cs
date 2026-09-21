namespace Front.Models
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string? PhotoUrl { get; set; }
    }
}