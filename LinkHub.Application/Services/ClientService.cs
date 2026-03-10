using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkHub.Domain.Models;
using LinkHub.Domain.Interfaces;
using LinkHub.Application.Interfaces;
using LinkHub.Application.Dtos;
using System;

namespace LinkHub.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IClientContactRepository _clientContactRepository;
        private readonly IClientCodeGenerator _codeGenerator;

        public ClientService(
            IClientRepository clientRepository,
            IContactRepository contactRepository,
            IClientContactRepository clientContactRepository,
            IClientCodeGenerator codeGenerator)
        {
            _clientRepository = clientRepository;
            _contactRepository = contactRepository;
            _clientContactRepository = clientContactRepository;
            _codeGenerator = codeGenerator;
        }

        public async Task<Client> CreateClientAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Client name is required.", nameof(name));

            var existingCodes = await _clientRepository.GetAllCodesAsync();
            var client = new Client
            {
                Name = name,
                ClientCode = _codeGenerator.GenerateCode(name, existingCodes),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _clientRepository.AddAsync(client);
            return client;
        }

        public async Task<IEnumerable<ClientDto>> GetClientsAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return clients.Select(MapToDto).ToList();
        }

        public async Task<ClientDto?> GetClientByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return client != null ? MapToDto(client) : null;
        }

        public async Task UpdateClientNameAsync(int id, string name)
        {
            if (id <= 0) throw new ArgumentException("Client id is required.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Client name is required.", nameof(name));

            var client = await GetClientOrThrowAsync(id);
            client.Name = name;
            client.UpdatedAt = DateTime.UtcNow;
            await _clientRepository.UpdateAsync(client);
        }

        public async Task LinkContactAsync(int clientId, int contactId)
        {
            var client = await GetClientOrThrowAsync(clientId);
            var contact = await GetContactOrThrowAsync(contactId);

            await _clientContactRepository.LinkAsync(client.Id, contact.Id);
        }

        public async Task UnlinkContactAsync(int clientId, int contactId)
        {
            var client = await GetClientOrThrowAsync(clientId);
            var contact = await GetContactOrThrowAsync(contactId);

            await _clientContactRepository.UnlinkAsync(client.Id, contact.Id);
        }

        public async Task<IEnumerable<ContactDto>> SearchAvailableContactsAsync(int clientId, string query, int skip, int take)
        {
            if (clientId <= 0)
                throw new ArgumentException("Client id is required.", nameof(clientId));

            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));

            if (take <= 0)
                throw new ArgumentOutOfRangeException(nameof(take));

            var results = await _contactRepository.SearchAvailableForClientAsync(clientId, query ?? string.Empty, skip, take);
            return results.Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Surname = c.Surname,
                Email = c.Email
            }).ToList();
        }

        

        private static ClientDto MapToDto(Client client) => new()
        {
            Id = client.Id,
            Name = client.Name,
            ClientCode = client.ClientCode,
            NoOfLinkedContacts = client.NoOfLinkedContacts,
            Contacts = client.Contacts?
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Surname = c.Surname,
                    Email = c.Email
                }).ToList() ?? new List<ContactDto>()
        };

        private async Task<Client> GetClientOrThrowAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null) throw new KeyNotFoundException("Client not found.");
            return client;
        }

        private async Task<Contact> GetContactOrThrowAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null) throw new KeyNotFoundException("Contact not found.");
            return contact;
        }

        
    }
}