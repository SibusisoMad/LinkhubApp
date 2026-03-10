using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;

namespace LinkHub.UI.Services
{
    public class ClientService : IClientService
    {
        private readonly HttpClient _client;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ClientService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<List<ClientListViewModel>> GetClientsAsync()
        {
            var response = await _client.GetAsync("/api/clients");

            if (!response.IsSuccessStatusCode)
                return new();

            var apiClients = await DeserializeAsync<List<Client>>(response) ?? new();

            return apiClients
                .Select(c => new ClientListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ClientCode = c.ClientCode,
                    NoOfLinkedContacts = c.NoOfLinkedContacts,
                })
                .ToList();
        }

        public async Task<bool> CreateClientAsync(string name)
        {
            var payload = new { name };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _client.PostAsync("/api/clients", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<ClientEditViewModel?> GetClientEditViewModelAsync(int clientId)
        {
            var response = await _client.GetAsync($"/api/clients/{clientId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var apiClient = await DeserializeAsync<Client>(response);
            if (apiClient == null)
                return null;

            var linkedContacts = apiClient.Contacts?
                .Select(c => new LinkedContactInfo
                {
                    ContactId = c.Id,
                    FullName = $"{c.Surname} {c.Name}",
                    Email = c.Email
                })
                .ToList() ?? new();

            return new ClientEditViewModel
            {
                Id = clientId,
                Name = apiClient.Name,
                LinkedContacts = linkedContacts,
                AvailableContacts = new(),
            };
        }

        public async Task<List<LinkedContactInfo>> SearchAvailableContactsAsync(int clientId, string query, int skip, int take)
        {
            query = query ?? string.Empty;
            var url = $"/api/clients/{clientId}/available-contacts?query={Uri.EscapeDataString(query)}&skip={skip}&take={take}";
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new();

            var matches = await DeserializeAsync<List<Contact>>(response) ?? new();
            return matches
                .Select(c => new LinkedContactInfo
                {
                    ContactId = c.Id,
                    FullName = $"{c.Surname} {c.Name}",
                    Email = c.Email ?? string.Empty
                })
                .ToList();
        }

        public async Task UpdateClientAsync(ClientEditViewModel model)
        {
            var payload = new { name = model.Name };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );
            await _client.PutAsync($"/api/clients/{model.Id}", content);
        }

        public async Task LinkContactAsync(int clientId, int contactId)
        {
            var response = await _client.PostAsync($"/api/clients/{clientId}/link-contact/{contactId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
                    ? "Failed to link contact."
                    : message);
            }
        }

        public async Task UnlinkContactAsync(int clientId, int contactId)
        {
            var response = await _client.DeleteAsync($"/api/clients/{clientId}/unlink-contact/{contactId}");
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
                    ? "Failed to unlink contact."
                    : message);
            }
        }

        private static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
    }
}
