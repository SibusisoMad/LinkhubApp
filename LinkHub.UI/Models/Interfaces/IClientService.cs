using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkHub.UI.Models.Interfaces
{
    public interface IClientService
    {
        Task<List<ClientListViewModel>> GetClientsAsync();
        Task<bool> CreateClientAsync(string name);
        Task<ClientEditViewModel?> GetClientEditViewModelAsync(int clientId);
        Task UpdateClientAsync(ClientEditViewModel model);
        Task LinkContactAsync(int clientId, int contactId);
        Task UnlinkContactAsync(int clientId, int contactId);
    }
}
