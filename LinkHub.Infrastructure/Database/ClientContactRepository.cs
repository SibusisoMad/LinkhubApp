using LinkHub.Domain.Interfaces;
using LinkHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkHub.Infrastructure.Database
{
    public class ClientContactRepository : IClientContactRepository
    {
        private readonly LinkHubDbContext _context;
        public ClientContactRepository(LinkHubDbContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<ClientContact>> GetAllAsync()
        {
            return await _context.ClientContacts
                .Include(cc => cc.Client)
                .Include(cc => cc.Contact)
                .ToListAsync();
        }

        public async Task AddAsync(ClientContact clientContact)
        {
            await _context.ClientContacts.AddAsync(clientContact);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ClientContact>> GetByClientIdAsync(int clientId)
        {
            return await _context.ClientContacts
                .Where(cc => cc.ClientId == clientId)
                .Include(cc => cc.Contact)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClientContact>> GetByContactIdAsync(int contactId)
        {
            return await _context.ClientContacts
                .Where(cc => cc.ContactId == contactId)
                .Include(cc => cc.Client)
                .ToListAsync();
        }


        public async Task<bool> IsLinkedAsync(int clientId, int contactId)
        {
            return await _context.ClientContacts.AnyAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);
        }

        public async Task LinkAsync(int clientId, int contactId)
        {
            if (await IsLinkedAsync(clientId, contactId))
                return;
            var clientContact = new ClientContact
            {
                ClientId = clientId,
                ContactId = contactId,
                LinkedAt = DateTime.UtcNow
            };
            await _context.ClientContacts.AddAsync(clientContact);

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
            if (client != null)
            {
                client.NoOfLinkedContacts = await _context.ClientContacts.CountAsync(cc => cc.ClientId == clientId) + 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UnlinkAsync(int clientId, int contactId)
        {
            var link = await _context.ClientContacts.FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);
            if (link != null)
            {
                _context.ClientContacts.Remove(link);

                var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
                if (client != null)
                {
                    client.NoOfLinkedContacts = await _context.ClientContacts.CountAsync(cc => cc.ClientId == clientId) - 1;
                    if (client.NoOfLinkedContacts < 0) client.NoOfLinkedContacts = 0;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}