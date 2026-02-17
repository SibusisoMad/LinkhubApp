using System.Threading.Tasks;
using LinkHub.Domain.Models;

namespace LinkHub.Application.Interfaces
{
    public interface IContactService
    {
        Task<Contact> CreateContactAsync(string name, string surname, string email);
        Task<IEnumerable<Contact>> GetContactsAsync();
        Task<Contact?> GetContactByIdAsync(int id);
        Task<Contact> UpdateContactAsync(int id, Contact updatedContact);

        Task LinkClientAsync(int contactId, int clientId);
        Task UnlinkClientAsync(int contactId, int clientId);
    }
}
