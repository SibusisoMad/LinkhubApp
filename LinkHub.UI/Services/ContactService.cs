using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
using System.Text;
using System.Text.Json;



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


        public async Task LinkClientAsync(int contactId, int clientId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/contacts/{contactId}/link-client/{clientId}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ContactListViewModel>> GetContactsAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.GetAsync("/api/contacts");

            if (!response.IsSuccessStatusCode)
                return new List<ContactListViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            var apiContacts = JsonSerializer.Deserialize<List<Contact>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return apiContacts?.Select(c => new ContactListViewModel
            {
                ContactId = c.Id,
                FullName = $"{c.Surname} {c.Name}",
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
        public async Task<ContactEditViewModel> GetContactEditViewModelAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/contacts/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var apiContact = JsonSerializer.Deserialize<Contact>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (apiContact == null)
                return null;

            
            var clientsResponse = await client.GetAsync("/api/clients");
            var clientsJson = await clientsResponse.Content.ReadAsStringAsync();
            var allClients = JsonSerializer.Deserialize<List<Client>>(clientsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Client>();

            var linkedClientIds = apiContact.LinkedClients?.Select(lc => lc.ClientId).ToHashSet() ?? new HashSet<int>();

            var availableClients = allClients
                .Where(c => !linkedClientIds.Contains(c.Id))
                .Select(c => new LinkedClientInfo { ClientId = c.Id, ClientName = c.Name, ClientCode = c.ClientCode })
                .ToList();

            return new ContactEditViewModel
            {
                Id = apiContact.Id,
                Name = apiContact.Name,
                Surname = apiContact.Surname,
                Email = apiContact.Email,
                LinkedClients = apiContact.LinkedClients ?? new List<LinkedClientInfo>(),
                AvailableClients = availableClients
            };
        }
        public async Task<bool> UpdateContactAsync(ContactUpdateViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );
            var response = await client.PutAsync($"/api/contacts/{model.Id}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
