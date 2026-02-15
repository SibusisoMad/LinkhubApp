using System;
using System.Threading.Tasks;
using LinkHub.Application.Interfaces;
using LinkHub.Domain.Models;
using LinkHub.Domain.Interfaces;

namespace LinkHub.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
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

        public async Task UnlinkClientAsync(int contactId, int clientId)
        {
            await _contactRepository.UnlinkClientAsync(contactId, clientId);
        }
    }
}
