using System.Collections.Generic;

namespace LinkHub.Domain.Models
{
    /// <summary>
    /// Represents a client in the LinkHub system.
    /// </summary>
    public class Client
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ClientCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();
    }
}
