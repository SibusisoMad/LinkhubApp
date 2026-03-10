using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
using LinkHub.UI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.UI.Controllers
{
    [Route("clients")]
    public class ClientsController : BaseController
    {
        private readonly IClientService _service;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(
            IClientService service,
            ILogger<ClientsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var clients = await _service.GetClientsAsync();
            return View(clients);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> Create(ClientCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return AjaxValidationError(ModelState);

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Name is required.");
                return AjaxValidationError(ModelState);
            }

            var success = await _service.CreateClientAsync(model.Name.Trim());

            if (!success)
                return AjaxError("Failed to create client.");

            TempData["Success"] = "Client created.";

            return Json(new { success = true, redirectUrl = Url.Action("List") });
        }

        [HttpGet("edit/{id:int?}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var model = await _service.GetClientEditViewModelAsync(id);

            return model == null ? NotFound() : View(model);
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> Edit(ClientEditViewModel model)
        {
            if (!ModelState.IsValid)
                return AjaxValidationError(ModelState);

            try
            {
                await _service.UpdateClientAsync(model);

                return AjaxSuccess();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update client {ClientId}", model.Id);
                return AjaxServerError("Update failed.");
            }
        }

        [HttpPost("link-contact")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> LinkContact(int clientId, int contactId)
        {
            if (clientId <= 0 || contactId <= 0)
                return AjaxError("Invalid client or contact id.");

            try
            {
                await _service.LinkContactAsync(clientId, contactId);
                return AjaxSuccess();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Link contact failed {ClientId} {ContactId}", clientId, contactId);
                return AjaxServerError("Failed to link contact.");
            }
        }

        [HttpPost("unlink-contact")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
        {
            if (clientId <= 0 || contactId <= 0)
                return AjaxError("Invalid client or contact id.");

            try
            {
                await _service.UnlinkContactAsync(clientId, contactId);
                return AjaxSuccess();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unlink contact failed {ClientId} {ContactId}", clientId, contactId);
                return AjaxServerError("Failed to unlink contact.");
            }
        }

        [HttpGet("search-available-contacts")]
        [AjaxOnly]
        public async Task<IActionResult> SearchAvailableContacts(
            int clientId,
            string query,
            int skip = 0,
            int take = 5)
        {
            if (clientId <= 0)
                return AjaxError("Invalid client id.");

            query = (query ?? string.Empty).Trim();
            if (query.Length < 3)
                return Json(new { items = Array.Empty<object>(), hasMore = false });

            var effectiveTake = take <= 0 ? 5 : take;
            var matches = await _service.SearchAvailableContactsAsync(
                clientId,
                query,
                skip < 0 ? 0 : skip,
                effectiveTake + 1);

            var hasMore = matches.Count > effectiveTake;
            var items = matches.Take(effectiveTake);

            return Json(new { items, hasMore });
        }
    }
}