using System.ComponentModel.DataAnnotations;

namespace LinkHub.UI.Models
{
    public class ContactCreateViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;
    }
}
