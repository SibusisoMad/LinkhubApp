using System.ComponentModel.DataAnnotations;

namespace LinkHub.API.Dtos
{
    public class UpdateClientNameRequestDto
    {
        [Required]
        [MinLength(2)]
        public string? Name { get; set; }
    }
}
