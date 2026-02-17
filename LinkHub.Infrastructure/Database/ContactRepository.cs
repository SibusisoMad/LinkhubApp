using LinkHub.Domain.Models;
using LinkHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkHub.Infrastructure.Database
{
    public class ContactRepository : IContactRepository
    {
        private readonly LinkHubDbContext _context;
        public ContactRepository(LinkHubDbContext context)
        {
            _context = context;
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts
                .Include(c => c.ClientContacts)
                .ThenInclude(cc => cc.Client)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _context.Contacts
                .OrderBy(c => c.Surname)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contact contact)
        {
            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task LinkClientAsync(int contactId, int clientId)
        {
            if (await _context.ClientContacts.AnyAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId))
                return;
            var clientContact = new ClientContact
            {
                ClientId = clientId,
                ContactId = contactId,
                LinkedAt = DateTime.UtcNow
            };
            await _context.ClientContacts.AddAsync(clientContact);
            await _context.SaveChangesAsync();

            
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact != null)
            {
                contact.NoOfLinkedClients = await _context.ClientContacts.CountAsync(cc => cc.ContactId == contactId);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnlinkClientAsync(int contactId, int clientId)
        {
            var clientContact = await _context.ClientContacts.FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);
            if (clientContact != null)
            {
                _context.ClientContacts.Remove(clientContact);
                await _context.SaveChangesAsync();

                var contact = await _context.Contacts.FindAsync(contactId);
                if (contact != null)
                {
                    contact.NoOfLinkedClients = await _context.ClientContacts.CountAsync(cc => cc.ContactId == contactId);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
