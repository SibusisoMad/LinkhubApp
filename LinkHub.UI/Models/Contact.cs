using System;

namespace LinkHub.UI.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; } 
        public string? Email { get; set; } 
        public int NoOfLinkedClients { get; set; }
        public List<LinkedClientInfo> LinkedClients { get; set; } = new List<LinkedClientInfo>();

        // The API returns linked client info via the join entity (clientContacts).
        public List<ContactClientContact>? ClientContacts { get; set; }
    }

    public class ContactClientContact
    {
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime LinkedAt { get; set; }
    }
}
