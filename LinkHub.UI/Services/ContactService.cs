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
            response.EnsureSuccessStatusCode();
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

            
            var clientsResponse = await client.GetAsync("/api/clients");
            var clientsJson = await clientsResponse.Content.ReadAsStringAsync();
            var allClients = JsonSerializer.Deserialize<List<Client>>(clientsJson, JsonOptions) ?? new List<Client>();

            var linkedClientIds = linkedClients.Select(lc => lc.ClientId).ToHashSet();

            var availableClients = allClients
                .Where(c => !linkedClientIds.Contains(c.Id))
                .Select(c => new LinkedClientInfo { ClientId = c.Id, ClientName = c.Name ?? string.Empty, ClientCode = c.ClientCode ?? string.Empty })
                .ToList();

            return new ContactEditViewModel
            {
                Id = apiContact.Id,
                Name = apiContact.Name,
                Surname = apiContact.Surname,
                Email = apiContact.Email,
                LinkedClients = linkedClients,
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
