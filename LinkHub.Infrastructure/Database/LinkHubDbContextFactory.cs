using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LinkHub.Domain.Models;

namespace LinkHub.Infrastructure.Database
{
    public class LinkHubDbContextFactory : IDesignTimeDbContextFactory<LinkHubDbContext>
    {
        public LinkHubDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LinkHubDbContext>();
            // Update the connection string as needed
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LinkHubDb;Trusted_Connection=True;");
            return new LinkHubDbContext(optionsBuilder.Options);
        }
    }
}
