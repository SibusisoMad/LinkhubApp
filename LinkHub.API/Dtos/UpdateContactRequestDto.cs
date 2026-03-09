using System.ComponentModel.DataAnnotations;

namespace LinkHub.API.Dtos
{
    public class UpdateContactRequestDto
    {
        [Required]
        [MinLength(2)]
        public string? Name { get; set; }

        [Required]
        [MinLength(2)]
        public string? Surname { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
