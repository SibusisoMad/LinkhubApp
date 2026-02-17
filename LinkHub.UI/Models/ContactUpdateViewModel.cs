using System.ComponentModel.DataAnnotations;

namespace LinkHub.UI.Models
{
    public class ContactUpdateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Surname { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
