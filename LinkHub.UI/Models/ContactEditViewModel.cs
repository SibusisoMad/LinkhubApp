using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkHub.UI.Models
{
    public class ContactEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public List<LinkedClientInfo> LinkedClients { get; set; } = new List<LinkedClientInfo>();
        public List<LinkedClientInfo> AvailableClients { get; set; } = new List<LinkedClientInfo>();
    }

    public class LinkedClientInfo
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientCode { get; set; }
    }
}
