using LinkHub.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkHub.Infrastructure.Database
{
    public class LinkHubDbContext : DbContext
    {
        public LinkHubDbContext(DbContextOptions<LinkHubDbContext> options)
            : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ClientContact> ClientContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);

                entity.Property(c => c.ClientCode).IsRequired().HasMaxLength(6);

                entity.HasIndex(c => c.ClientCode).IsUnique(); // ClientCode must be unique
                entity.Property(c => c.NoOfLinkedContacts).HasDefaultValue(0);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Surname).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(c => c.Email).IsUnique();
                entity.Property(c => c.NoOfLinkedClients).HasDefaultValue(0);
                entity.Property(c => c.CreatedAt).IsRequired();
                entity.Property(c => c.UpdatedAt).IsRequired();
            });


            modelBuilder.Entity<ClientContact>(entity =>
            {
                
                entity.HasKey(cc => new { cc.ClientId, cc.ContactId });

                entity.Property(cc => cc.LinkedAt).IsRequired();

                
                entity
                    .HasOne(cc => cc.Client)
                    .WithMany(c => c.ClientContacts)
                    .HasForeignKey(cc => cc.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);

                
                entity
                    .HasOne(cc => cc.Contact)
                    .WithMany(c => c.ClientContacts)
                    .HasForeignKey(cc => cc.ContactId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(cc => cc.ClientId);
                entity.HasIndex(cc => cc.ContactId);
            });

            LinkHubDbSeeder.Seed(modelBuilder);
        }
    }
}
