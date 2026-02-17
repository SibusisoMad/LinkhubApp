using System.Collections.Generic;
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

        public ClientService(IClientRepository clientRepository, IContactRepository contactRepository, IClientContactRepository clientContactRepository, IClientCodeGenerator codeGenerator)
        {
            _clientRepository = clientRepository;
            _contactRepository = contactRepository;
            _clientContactRepository = clientContactRepository;
            _codeGenerator = codeGenerator;
        }

        public async Task<Client> CreateClientAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Client name is required.");

            var existingCodes = await _clientRepository.GetAllCodesAsync();
            var clientCode = _codeGenerator.GenerateCode(name, existingCodes);

            var client = new Client
            {
                Name = name,
                ClientCode = clientCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _clientRepository.AddAsync(client);
            return client;
        }


        public async Task<IEnumerable<ClientDto>> GetClientsAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return clients.Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ClientCode = c.ClientCode,
                NoOfLinkedContacts = c.NoOfLinkedContacts,
                Contacts = c.Contacts.Select(contact => new ContactDto
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Surname = contact.Surname,
                    Email = contact.Email
                }).ToList()
            }).ToList();
        }

        public async Task<ClientDto?> GetClientByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
                return null;

            return new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                ClientCode = client.ClientCode,
                NoOfLinkedContacts = client.NoOfLinkedContacts,
                Contacts = client.Contacts.Select(contact => new ContactDto
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Surname = contact.Surname,
                    Email = contact.Email
                }).ToList()
            };
        }

        public async Task LinkContactAsync(int clientId, int contactId)
        {
            
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
                throw new KeyNotFoundException("Client not found.");

            var contact = await _contactRepository.GetByIdAsync(contactId);
            if (contact == null)
                throw new KeyNotFoundException("Contact not found.");

            await _clientContactRepository.LinkAsync(clientId, contactId);
        }

        public async Task UnlinkContactAsync(int clientId, int contactId)
        {
            var client = await _clientRepository.GetByIdAsync(clientId);

            if (client == null)
                throw new KeyNotFoundException("Client not found.");

            var contact = await _contactRepository.GetByIdAsync(contactId);
            if (contact == null)
                throw new KeyNotFoundException("Contact not found.");

            await _clientContactRepository.UnlinkAsync(clientId, contactId);
        }
    }
}