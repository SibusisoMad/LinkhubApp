namespace LinkHub.UI.Models
{
    using System.ComponentModel.DataAnnotations;
    public class ClientCreateViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
