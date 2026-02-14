using Microsoft.EntityFrameworkCore;
using LinkHub.Domain.Models;

namespace LinkHub.Database
{
    public class LinkHubDbContext : DbContext
    {
        public LinkHubDbContext(DbContextOptions<LinkHubDbContext> options) : base(options) {}

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.ClientCode)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Contacts)
                .WithMany(c => c.Clients);
        }
    }
}