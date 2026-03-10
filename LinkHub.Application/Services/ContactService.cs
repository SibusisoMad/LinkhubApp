using System;
using System.Threading.Tasks;
using LinkHub.Application.Interfaces;
using LinkHub.Domain.Models;
using LinkHub.Domain.Interfaces;
using LinkHub.Application.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace LinkHub.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IClientRepository _clientRepository;

        public ContactService(IContactRepository contactRepository, IClientRepository clientRepository)
        {
            _contactRepository = contactRepository;
            _clientRepository = clientRepository;
        }


        public async Task<Contact> CreateContactAsync(string name, string surname, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(surname))
                throw new ArgumentException("Surname is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            var contacts = await _contactRepository.GetAllAsync();

            if (contacts.Any(c => c.Email.ToLower() == email.ToLower()))
                throw new InvalidOperationException("A contact with this email already exists.");

            var contact = new Contact
            {
                Name = name,
                Surname = surname,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _contactRepository.AddAsync(contact);
            return contact;
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync()
        {
            return await _contactRepository.GetAllAsync();
        }

        public async Task<Contact?> GetContactByIdAsync(int id)
        {
            return await _contactRepository.GetByIdAsync(id);
        }

        public async Task LinkClientAsync(int contactId, int clientId)
        {
            await _contactRepository.LinkClientAsync(contactId, clientId);
        }

        public async Task<Contact> UpdateContactAsync(int id, Contact updatedContact)
        {
            if (id <= 0)
                throw new ArgumentException("Contact id is required.", nameof(id));

            if (updatedContact == null)
                throw new ArgumentNullException(nameof(updatedContact));

            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                throw new KeyNotFoundException("Contact not found.");

            contact.Name = updatedContact.Name;
            contact.Surname = updatedContact.Surname;
            contact.Email = updatedContact.Email;
            contact.UpdatedAt = DateTime.UtcNow;

            await _contactRepository.UpdateAsync(contact);

            return contact;
           
        }

        public async Task UnlinkClientAsync(int contactId, int clientId)
        {
            await _contactRepository.UnlinkClientAsync(contactId, clientId);
        }

        public async Task<IEnumerable<ClientDto>> SearchAvailableClientsAsync(int contactId, string query, int skip, int take)
        {
            if (contactId <= 0)
                throw new ArgumentException("Contact id is required.", nameof(contactId));

            query = (query ?? string.Empty).Trim();
            if (query.Length < 3)
                return Enumerable.Empty<ClientDto>();

            if (skip < 0)
                skip = 0;

            if (take <= 0)
                take = 5;

            if (take > 50)
                take = 50;

            var matches = await _clientRepository.SearchAvailableForContactAsync(contactId, query, skip, take);
            return matches.Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ClientCode = c.ClientCode,
                NoOfLinkedContacts = c.NoOfLinkedContacts,
                Contacts = new List<ContactDto>()
            });
        }
    }
}
