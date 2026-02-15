using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.UI.Models
{
    public class ClientEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public List<LinkedContactInfo> LinkedContacts { get; set; } = new List<LinkedContactInfo>();
        public List<LinkedContactInfo> AvailableContacts { get; set; } = new List<LinkedContactInfo>();
    }

    public class LinkedContactInfo
    {
        public int ContactId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
