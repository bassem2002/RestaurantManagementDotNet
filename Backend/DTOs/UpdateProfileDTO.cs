using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class UpdateProfileDTO
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        // Ces propriétés apparaissent dans ta réponse Ok(...), 
        // donc il est logique de les inclure dans le DTO même si elles 
        // ne sont pas directement dans ApplicationUser actuellement.
        // Tu peux les ajouter à ApplicationUser plus tard ou les gérer séparément.
        public string? City { get; set; }

        public string? PostalCode { get; set; }

        // Optionnel : permettre la mise à jour du nom d'utilisateur et de l'email
        // (décommenter si tu veux autoriser ces changements)
        // public string? UserName { get; set; }

        // [EmailAddress]
        // public string? Email { get; set; }
    }
}
    
