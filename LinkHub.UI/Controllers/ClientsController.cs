using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Name is required");
                return View(model);
            }

            var success = await _clientService.CreateClientAsync(model.Name);
            if (success)
            {
                TempData["SuccessMessage"] = "Client created";
                TempData["ShowModalAndRedirect"] = true;
                return RedirectToAction(nameof(Create));
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

            LoadMessages();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClientEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            await _clientService.UpdateClientAsync(model);
            TempData["SuccessMessage"] = "Client updated.";
            return RedirectToAction(nameof(Edit), new { id = model.Id });
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
            return RedirectToAction(nameof(Edit), new { id = clientId });
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
        {
            await _clientService.UnlinkContactAsync(clientId, contactId);
            TempData["UnlinkSuccess"] = "Contact unlinking is successful.";
            return RedirectToAction(nameof(Edit), new { id = clientId });
        }

        private void LoadMessages()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"]?.ToString();
            ViewBag.LinkSuccess = TempData["LinkSuccess"]?.ToString();
            ViewBag.UnlinkSuccess = TempData["UnlinkSuccess"]?.ToString();
            ViewBag.LinkError = TempData["LinkError"]?.ToString();
        }
    }
}
