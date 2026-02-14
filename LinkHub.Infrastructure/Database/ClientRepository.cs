using LinkHub.Domain.Models;
using LinkHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.Include(c => c.Contacts).OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients.Include(c => c.Contacts).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}