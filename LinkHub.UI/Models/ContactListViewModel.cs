namespace LinkHub.UI.Models
{
    public class ContactListViewModel
    {
        public int ContactId { get; set; }
        public string? FullName { get; set; } 
        public string? Email { get; set; } 
        public int NoOfLinkedClients { get; set; }
        public List<LinkedClientInfo> LinkedClients { get; set; } = new List<LinkedClientInfo>();
    }

    public class LinkedClientInfo
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
