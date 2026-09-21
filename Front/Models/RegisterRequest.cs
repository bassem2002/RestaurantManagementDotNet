using System.ComponentModel.DataAnnotations;

namespace Front.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; }
    }
}
