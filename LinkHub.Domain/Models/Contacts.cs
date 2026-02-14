using System.Collections.Generic;

namespace LinkHub.Domain.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Contacts
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;
    }
}
