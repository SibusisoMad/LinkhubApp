using LinkHub.Application.Interfaces;
using LinkHub.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<ActionResult<Client>> CreateClient([FromBody] string name)
        {
            var client = await _clientService.CreateClientAsync(name);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _clientService.GetClientsAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClientById(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
                return NotFound();
            return client;
        }

        [HttpPost("{clientId}/link-contact/{contactId}")]
        public async Task<IActionResult> LinkContact(int clientId, int contactId)
        {
            await _clientService.LinkContactAsync(clientId, contactId);
            return NoContent();
        }

        [HttpDelete("{clientId}/unlink-contact/{contactId}")]
        public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
        {
            await _clientService.UnlinkContactAsync(clientId, contactId);
            return NoContent();
        }
    }
}
