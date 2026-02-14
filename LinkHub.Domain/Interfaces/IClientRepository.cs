using LinkHub.Domain.Models;

namespace LinkHub.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<string>> GetAllCodesAsync();
        Task AddAsync(Client client);
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
    }

}