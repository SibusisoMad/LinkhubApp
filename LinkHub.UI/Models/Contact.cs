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
    }
}
