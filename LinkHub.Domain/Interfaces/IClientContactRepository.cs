using LinkHub.Domain.Models;
using System.Threading.Tasks;

namespace LinkHub.Domain.Interfaces
{
    public interface IClientContactRepository
    {
        Task<bool> IsLinkedAsync(int clientId, int contactId);
        Task LinkAsync(int clientId, int contactId);
        Task UnlinkAsync(int clientId, int contactId);
    }
}
