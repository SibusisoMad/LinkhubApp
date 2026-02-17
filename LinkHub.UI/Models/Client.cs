namespace LinkHub.UI.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string? Name { get; set; } 
        public string? ClientCode { get; set; } 
        public int NoOfLinkedContacts { get; set; }
        public List<LinkedContactInfo>? Contacts { get; set; }
    }
}
