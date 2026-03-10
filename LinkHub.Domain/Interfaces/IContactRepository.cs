using LinkHub.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkHub.Domain.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(int id);
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<List<Contact>> SearchAvailableForClientAsync(int clientId, string query, int skip, int take);
        Task AddAsync(Contact contact);
        Task LinkClientAsync(int contactId, int clientId);
        Task UnlinkClientAsync(int contactId, int clientId);
        Task UpdateAsync(Contact contact);
    }
}
