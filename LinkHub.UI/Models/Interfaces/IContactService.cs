using System.Threading.Tasks;
using LinkHub.UI.Models;

namespace LinkHub.UI.Models.Interfaces
{
    public interface IContactService
    {
        Task<bool> CreateContactAsync(ContactCreateViewModel model);
        Task<List<ContactListViewModel>> GetContactsAsync();
        Task UnlinkClientAsync(int contactId, int clientId);
    }
}
