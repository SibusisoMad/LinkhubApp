using System.Collections.Generic;

namespace LinkHub.Domain.Models
{
    using System.ComponentModel.DataAnnotations;
    
        public class Contact
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Name is required.")]
            public string Name { get; set; } = null!;

            [Required(ErrorMessage = "Surname is required.")]
            public string Surname { get; set; } = null!;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = null!;

            public int NoOfLinkedClients { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public ICollection<Client> Clients { get; set; } = new List<Client>();
            public ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();
        }
}
