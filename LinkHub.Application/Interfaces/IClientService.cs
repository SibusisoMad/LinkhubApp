using LinkHub.Domain.Models;
using LinkHub.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkHub.Application.Interfaces
{
    public interface IClientService
    {
        Task<Client> CreateClientAsync(string name);
        Task<IEnumerable<ClientDto>> GetClientsAsync();
        Task<ClientDto?> GetClientByIdAsync(int id);
        Task LinkContactAsync(int clientId, int contactId);
        Task UnlinkContactAsync(int clientId, int contactId);
    }
}