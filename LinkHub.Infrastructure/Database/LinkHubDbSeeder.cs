using LinkHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace LinkHub.Infrastructure.Database
{
    public static class LinkHubDbSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var client1 = new Client { Id = 1, Name = "Udemy", ClientCode = "UDE001", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var client2 = new Client { Id = 2, Name = "weThinkCode", ClientCode = "WTC002", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            var contact1 = new Contact {
                Id = 1,
                Name = "Kwazi",
                Surname = "Nkosi",
                Email = "KN@gmail.com",
                NoOfLinkedClients = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var contact2 = new Contact {
                Id = 2,
                Name = "Zano",
                Surname = "Kuhle",
                Email = "ZK@gmail.com",
                NoOfLinkedClients = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cc1 = new ClientContact { ClientId = 1, ContactId = 1, LinkedAt = DateTime.UtcNow };
            var cc2 = new ClientContact { ClientId = 1, ContactId = 2, LinkedAt = DateTime.UtcNow };
            var cc3 = new ClientContact { ClientId = 2, ContactId = 1, LinkedAt = DateTime.UtcNow };

            modelBuilder.Entity<Client>().HasData(client1, client2);
            modelBuilder.Entity<Contact>().HasData(contact1, contact2);
            modelBuilder.Entity<ClientContact>().HasData(cc1, cc2, cc3);
        }
    }
}