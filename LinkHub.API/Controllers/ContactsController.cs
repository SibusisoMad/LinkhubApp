using System.Collections.Generic;
using System.Threading.Tasks;
using LinkHub.Application.Interfaces;
using LinkHub.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using LinkHub.API.Dtos;
using Microsoft.AspNetCore.Http;
using System;

namespace LinkHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(
            IContactService contactService
        )
        {
            _contactService = contactService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Contact>> CreateContact([FromBody] CreateContactRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            if (string.IsNullOrWhiteSpace(dto.Surname))
                return BadRequest("Surname is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            try
            {
                var createdContact = await _contactService.CreateContactAsync(
                    dto.Name,
                    dto.Surname,
                    dto.Email
                );

                return CreatedAtAction(
                    nameof(GetContactById),
                    new { id = createdContact.Id },
                    createdContact
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var contacts = await _contactService.GetContactsAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Contact>> GetContactById(int id)
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound();
            return contact;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] UpdateContactRequestDto dto)
        {
            if (id <= 0)
                return BadRequest("Contact id is required.");

            if (dto == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            if (string.IsNullOrWhiteSpace(dto.Surname))
                return BadRequest("Surname is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            try
            {
                await _contactService.UpdateContactAsync(id, new Contact
                {
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email
                });

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("{contactId}/link-client/{clientId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> LinkClient(int contactId, int clientId)
        {
            if (contactId <= 0 || clientId <= 0)
                return BadRequest("Contact id and client id are required.");

            try
            {
                await _contactService.LinkClientAsync(contactId, clientId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{contactId}/unlink-client/{clientId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UnlinkClient(int contactId, int clientId)
        {
            if (contactId <= 0 || clientId <= 0)
                return BadRequest("Contact id and client id are required.");

            try
            {
                await _contactService.UnlinkClientAsync(contactId, clientId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
