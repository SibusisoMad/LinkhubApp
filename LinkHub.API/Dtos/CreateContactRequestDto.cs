using System.ComponentModel.DataAnnotations;

namespace LinkHub.API.Dtos
{
    public class CreateContactRequestDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
