namespace LinkHub.Application.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ClientCode { get; set; }
        public int NoOfLinkedContacts { get; set; }
        public List<ContactDto> Contacts { get; set; } = new List<ContactDto>();
    }
}
