using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;

namespace LinkHub.UI.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await _clientService.GetClientsAsync();
            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClientCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _clientService.CreateClientAsync(model.Name);
            if (success)
            {
                TempData["SuccessMessage"] = "Client created";
                TempData["ShowModalAndRedirect"] = true;
                return RedirectToAction("Create");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to create client.");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _clientService.GetClientEditViewModelAsync(id);
            if (model == null)
                return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClientEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            await _clientService.UpdateClientAsync(model);
            TempData["SuccessMessage"] = "Client updated.";
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> LinkContact(int clientId, int contactId)
        {
            try
            {
                await _clientService.LinkContactAsync(clientId, contactId);
                TempData["LinkSuccess"] = "Contact linked successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["LinkError"] = ex.Message;
            }
            return RedirectToAction("Edit", new { id = clientId });
        }

        [HttpGet]
        public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
        {
            await _clientService.UnlinkContactAsync(clientId, contactId);
            return RedirectToAction("Edit", new { id = clientId });
        }
    }
}
