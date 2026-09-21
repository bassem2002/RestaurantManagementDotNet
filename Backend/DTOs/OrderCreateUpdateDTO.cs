using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    public class OrderCreateUpdateDTO
    {
        public DateTime OrderDate { get; set; } = DateTime.Now;


        public string UserId { get; set; }
    }
}
