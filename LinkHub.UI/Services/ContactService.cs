using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
using System.Text;
using System.Text.Json;



namespace LinkHub.UI.Services
{
    public class ContactService : IContactService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

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
            var response = await client.PostAsync($"/api/contacts/{contactId}/link-client/{clientId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
                    ? "Failed to link client."
                    : message);
            }
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
                JsonOptions
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
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
                    ? "Failed to unlink client."
                    : message);
            }
        }
        public async Task<ContactEditViewModel?> GetContactEditViewModelAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/contacts/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var apiContact = JsonSerializer.Deserialize<Contact>(json, JsonOptions);
            if (apiContact == null)
                return null;

            var linkedClients = apiContact.LinkedClients ?? new List<LinkedClientInfo>();
            if (linkedClients.Count == 0 && apiContact.ClientContacts?.Count > 0)
            {
                linkedClients = apiContact.ClientContacts
                    .Where(cc => cc.Client != null)
                    .Select(cc => new LinkedClientInfo
                    {
                        ClientId = cc.ClientId,
                        ClientName = cc.Client!.Name ?? string.Empty,
                        ClientCode = cc.Client!.ClientCode ?? string.Empty,
                    })
                    .ToList();
            }

            if (linkedClients.Count == 0 && apiContact.Clients?.Count > 0)
            {
                linkedClients = apiContact.Clients
                    .Select(c => new LinkedClientInfo
                    {
                        ClientId = c.Id,
                        ClientName = c.Name,
                        ClientCode = c.ClientCode
                    })
                    .ToList();
            }

            
            return new ContactEditViewModel
            {
                Id = apiContact.Id,
                Name = apiContact.Name,
                Surname = apiContact.Surname,
                Email = apiContact.Email,
                LinkedClients = linkedClients,
                AvailableClients = new()
            };
        }

        public async Task<List<LinkedClientInfo>> SearchAvailableClientsAsync(int contactId, string query, int skip, int take)
        {
            query = query ?? string.Empty;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = $"/api/contacts/{contactId}/available-clients?query={Uri.EscapeDataString(query)}&skip={skip}&take={take}";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new();

            var json = await response.Content.ReadAsStringAsync();
            var matches = JsonSerializer.Deserialize<List<Client>>(json, JsonOptions) ?? new();
            return matches
                .Select(c => new LinkedClientInfo
                {
                    ClientId = c.Id,
                    ClientName = c.Name ?? string.Empty,
                    ClientCode = c.ClientCode ?? string.Empty
                })
                .ToList();
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
