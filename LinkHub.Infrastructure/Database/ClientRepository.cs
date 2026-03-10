using LinkHub.Domain.Models;
using LinkHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LinkHub.Infrastructure.Database
{
    public class ClientRepository : IClientRepository
    {
        private readonly LinkHubDbContext _context;
        public ClientRepository(LinkHubDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> GetAllCodesAsync()
        {
            return await _context.Clients.Select(c => c.ClientCode!).ToListAsync();
        }

        public async Task AddAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.Include(c => c.Contacts).OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients.Include(c => c.Contacts).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Client>> SearchAvailableForContactAsync(int contactId, string query, int skip, int take)
        {
            query = (query ?? string.Empty).Trim();
            if (contactId <= 0)
                return new List<Client>();

            if (query.Length < 3)
                return new List<Client>();

            if (skip < 0)
                skip = 0;

            if (take <= 0)
                take = 5;

            if (take > 50)
                take = 50;

            var like = $"%{query}%";

            return await _context.Clients
                .AsNoTracking()
                .Where(c => !c.ClientContacts.Any(cc => cc.ContactId == contactId))
                .Where(c =>
                    EF.Functions.Like(c.Name ?? string.Empty, like) ||
                    EF.Functions.Like(c.ClientCode ?? string.Empty, like))
                .OrderBy(c => c.Name)
                .ThenBy(c => c.ClientCode)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}