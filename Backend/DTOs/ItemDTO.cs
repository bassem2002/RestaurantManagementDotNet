using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    public class ItemDTO
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        public string? PhotoUrl { get; set; }
        public string CategoryName { get; set; }
    }
}
