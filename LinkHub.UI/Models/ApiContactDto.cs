namespace LinkHub.UI.Models
{
    public class ApiContactDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int NoOfLinkedClients { get; set; }
        public List<LinkedClientInfo> LinkedClients { get; set; } = new List<LinkedClientInfo>();
    }
}
