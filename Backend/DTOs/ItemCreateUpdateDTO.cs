using System.ComponentModel.DataAnnotations;

public class ItemCreateUpdateDTO
{
    [Required]
    public string Name { get; set; }
    [Required]
    public double Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public IFormFile? Photo { get; set; }
}