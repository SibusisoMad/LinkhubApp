using LinkHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LinkHub.Application.Dtos;
using LinkHub.API.Dtos;
using System;
using System.Collections.Generic;

namespace LinkHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Client name is required.");

            var created = await _clientService.CreateClientAsync(dto.Name);
            var createdDto = await _clientService.GetClientByIdAsync(created.Id);
            return CreatedAtAction(nameof(GetClientById), new { id = created.Id }, createdDto);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clientDtos = await _clientService.GetClientsAsync();
            return Ok(clientDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            var clientDto = await _clientService.GetClientByIdAsync(id);
            return clientDto is { } client
                ? Ok(client)
                : NotFound();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClientName(int id, [FromBody] UpdateClientNameRequestDto dto)
        {
            if (id <= 0)
                return BadRequest("Client id is required.");

            if (dto == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Client name is required.");

            try
            {
                await _clientService.UpdateClientNameAsync(id, dto.Name);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{clientId}/link-contact/{contactId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> LinkContact(int clientId, int contactId)
        {
            if (clientId <= 0 || contactId <= 0)
                return BadRequest("Client id and contact id are required.");

            try
            {
                await _clientService.LinkContactAsync(clientId, contactId);
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

        [HttpDelete("{clientId}/unlink-contact/{contactId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
        {
            if (clientId <= 0 || contactId <= 0)
                return BadRequest("Client id and contact id are required.");

            try
            {
                await _clientService.UnlinkContactAsync(clientId, contactId);
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

        [HttpGet("{clientId}/available-contacts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> SearchAvailableContacts(
            int clientId,
            [FromQuery] string query,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 5)
        {
            if (clientId <= 0)
                return BadRequest("Client id is required.");

            if (take <= 0 || take > 50)
                return BadRequest("Take must be between 1 and 50.");

            if (skip < 0)
                return BadRequest("Skip must be 0 or greater.");

            query = query ?? string.Empty;
            var results = await _clientService.SearchAvailableContactsAsync(clientId, query, skip, take);
            return Ok(results);
        }
    }
}
