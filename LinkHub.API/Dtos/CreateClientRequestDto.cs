using System.ComponentModel.DataAnnotations;

namespace LinkHub.API.Dtos
{
    public class CreateClientRequestDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;
    }
}
