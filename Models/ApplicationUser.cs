using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Phone { get; set; }

        // 1 → 1 Cart
        public virtual Cart Cart { get; set; }

        public string? City { get; set; }
        public string? PostalCode { get; set; }

        // 1 → N Orders
        public virtual List<Order> Orders { get; set; }
    }
}
