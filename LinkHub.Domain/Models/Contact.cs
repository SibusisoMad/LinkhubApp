using System.Collections.Generic;

namespace LinkHub.Domain.Models
{
    /// <summary>
    /// Represents a contact that can be linked to multiple clients.
    /// </summary>
    public class Contact
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();
    }
}
