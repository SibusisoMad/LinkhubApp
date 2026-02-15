namespace LinkHub.UI.Models
{
    public class ContactListViewModel
    {
        public int ContactId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int NoOfLinkedClients { get; set; }
        public List<LinkedClientInfo> LinkedClients { get; set; } = new List<LinkedClientInfo>();
    }

    public class LinkedClientInfo
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
    }
}
