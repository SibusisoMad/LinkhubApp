using LinkHub.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkHub.Application.Interfaces
{
    public interface IClientService
    {
        Task<Client> CreateClientAsync(string name);
        Task<IEnumerable<Client>> GetClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
        Task LinkContactAsync(int clientId, int contactId);
        Task UnlinkContactAsync(int clientId, int contactId);
    }
}