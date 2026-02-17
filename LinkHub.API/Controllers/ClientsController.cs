using LinkHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LinkHub.Application.Dtos;

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
        public async Task<ActionResult> CreateClient([FromBody] string name)
        {
            var client = await _clientService.CreateClientAsync(name);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clientDtos = await _clientService.GetClientsAsync();
            return Ok(clientDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            var clientDto = await _clientService.GetClientByIdAsync(id);
            if (clientDto == null)
                return NotFound();
            return clientDto;
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
