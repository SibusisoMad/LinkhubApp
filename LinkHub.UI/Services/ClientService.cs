using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
// ...existing code...

namespace LinkHub.UI.Services
{
    public class ClientService : IClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ClientListViewModel>> GetClientsAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("/api/clients");
            if (!response.IsSuccessStatusCode)
                return new List<ClientListViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var apiClients = JsonSerializer.Deserialize<List<ApiClientDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return apiClients == null
                ? new List<ClientListViewModel>()
                : apiClients
                    .Select(c => new ClientListViewModel
                    {
                        Name = c.Name,
                        ClientCode = c.ClientCode,
                        NoOfLinkedContacts = c.NoOfLinkedContacts,
                    })
                    .ToList();
        }

        public async Task<bool> CreateClientAsync(string name)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(
                JsonSerializer.Serialize(name),
                Encoding.UTF8,
                "application/json"
            );
            var response = await client.PostAsync("/api/clients", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<ClientEditViewModel?> GetClientEditViewModelAsync(int clientId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/clients/{clientId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var apiClient = JsonSerializer.Deserialize<ApiClientDto>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (apiClient == null)
                return null;

            // Linked contacts (already strongly-typed)
            var linkedContacts = apiClient.Contacts ?? new List<LinkedContactInfo>();

            // Get all contacts for dropdown
            var allContactsResponse = await client.GetAsync("/api/contacts");
            var allContactsJson = await allContactsResponse.Content.ReadAsStringAsync();
            var allContacts =
                JsonSerializer.Deserialize<List<ApiContactDto>>(
                    allContactsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<ApiContactDto>();

            var availableContacts = allContacts
                .Where(c => !linkedContacts.Any(lc => lc.ContactId == c.Id))
                .Select(c => new LinkedContactInfo
                {
                    ContactId = c.Id,
                    FullName = $"{c.Name} {c.Surname}",
                    Email = c.Email,
                })
                .ToList();

            return new ClientEditViewModel
            {
                Id = clientId,
                Name = apiClient.Name,
                LinkedContacts = linkedContacts,
                AvailableContacts = availableContacts,
            };
        }

        public async Task UpdateClientAsync(ClientEditViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(
                JsonSerializer.Serialize(model.Name),
                Encoding.UTF8,
                "application/json"
            );
            await client.PostAsync($"/api/clients", content);
        }

        public async Task LinkContactAsync(int clientId, int contactId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            await client.PostAsync($"/api/clients/{clientId}/link-contact/{contactId}", null);
        }

        public async Task UnlinkContactAsync(int clientId, int contactId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            await client.DeleteAsync($"/api/clients/{clientId}/unlink-contact/{contactId}");
        }
    }
}
