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

        public List<Client>? Clients { get; set; }

        public List<ContactClientContact>? ClientContacts { get; set; }
    }

    public class ContactClientContact
    {
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime LinkedAt { get; set; }
    }
}
