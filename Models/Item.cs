using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Range(0, 999999)]
        public decimal Price { get; set; }

        // Foreign Key
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]

        public virtual Category Category { get; set; }
        public string? PhotoPath { get; set; }
    }
}

