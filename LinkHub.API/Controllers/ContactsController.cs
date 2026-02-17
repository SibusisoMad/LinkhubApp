using System.Collections.Generic;
using System.Threading.Tasks;
using LinkHub.Application.Interfaces;
using LinkHub.Domain.Interfaces;
using LinkHub.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private readonly IContactService _contactService;

        public ContactsController(
            IContactRepository contactRepository,
            IContactService contactService
        )
        {
            _contactRepository = contactRepository;
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact([FromBody] Contacts contact)
        {
            var createdContact = await _contactService.CreateContactAsync(
                contact.Name,
                contact.Surname,
                contact.Email
            );
            return CreatedAtAction(
                nameof(GetContactById),
                new { id = createdContact.Id },
                createdContact
            );
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var contacts = await _contactService.GetContactsAsync();
            if (!contacts.Any())
                return NotFound("No contacts found.");
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContactById(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                return NotFound();
            return contact;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] Contact updatedContact)
        {
            var result = await _contactService.UpdateContactAsync(id, updatedContact);
            
            return NoContent();
        }

        [HttpPost("{contactId}/link-client/{clientId}")]
        public async Task<IActionResult> LinkClient(int contactId, int clientId)
        {
            await _contactService.LinkClientAsync(contactId, clientId);
            return NoContent();
        }

        [HttpDelete("{contactId}/unlink-client/{clientId}")]
        public async Task<IActionResult> UnlinkClient(int contactId, int clientId)
        {
            await _contactService.UnlinkClientAsync(contactId, clientId);
            return NoContent();
        }
    }
}
