using System.ComponentModel.DataAnnotations;
namespace Backend.Models 
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required, MaxLength(50)]
        public string Name
        {
            get; set;
        }

    }
}
        // Pas de List<Item> } }