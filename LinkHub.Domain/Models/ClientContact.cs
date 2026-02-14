using LinkHub.Domain.Models;

public class ClientContact
{
    public int ClientId { get; set; }
    public Client Client { get; set; }

    public int ContactId { get; set; }
    public Contact Contact { get; set; }

    public DateTime LinkedAt { get; set; }
}
