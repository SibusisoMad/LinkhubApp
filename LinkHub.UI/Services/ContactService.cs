using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinkHub.UI.Services
{
    public class ContactService : IContactService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ContactService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateContactAsync(ContactCreateViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );
            var response = await client.PostAsync("/api/contacts", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ContactListViewModel>> GetContactsAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("/api/contacts");
            if (!response.IsSuccessStatusCode)
                return new List<ContactListViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            var apiContacts = JsonSerializer.Deserialize<List<ApiContactDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            return apiContacts?.Select(c => new ContactListViewModel
            {
                ContactId = c.Id,
                FullName = $"{c.Name} {c.Surname}",
                Email = c.Email,
                NoOfLinkedClients = c.NoOfLinkedClients,
                LinkedClients = c.LinkedClients ?? new List<LinkedClientInfo>()
            })
            .ToList() ?? new List<ContactListViewModel>();
        }

        public async Task UnlinkClientAsync(int contactId, int clientId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"/api/contacts/{contactId}/unlink-client/{clientId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
