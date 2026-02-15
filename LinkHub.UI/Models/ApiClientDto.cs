namespace LinkHub.UI.Models
{
    public class ApiClientDto
    {
        public string Name { get; set; } = string.Empty;
        public string ClientCode { get; set; } = string.Empty;
        public int NoOfLinkedContacts { get; set; }
        public List<LinkedContactInfo>? Contacts { get; set; }
    }
}
