using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
using LinkHub.UI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.UI.Controllers
{
    [Route("contacts")]
    public class ContactsController : BaseController
    {
        private readonly IContactService _service;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(
            IContactService service,
            ILogger<ContactsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var contacts = await _service.GetContactsAsync();

            return View(contacts);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> Create(ContactCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return AjaxValidationError(ModelState);

            var success = await _service.CreateContactAsync(model);

            if (!success)
                return AjaxError("Failed to create contact.");

            TempData["Success"] = "Contact created.";

            return Json(new { success = true, redirectUrl = Url.Action("List") });
        }

        [HttpGet("edit/{id:int?}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var model = await _service.GetContactEditViewModelAsync(id);

            return model == null ? NotFound() : View(model);
        }

        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> Update(ContactUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return AjaxValidationError(ModelState);

            var success = await _service.UpdateContactAsync(model);

            if (!success)
                return AjaxError("Failed to update contact.");

            return AjaxSuccess();
        }

        [HttpPost("link-client")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> LinkClient(int contactId, int clientId)
        {
            if (contactId <= 0 || clientId <= 0)
                return AjaxError("Invalid contact or client id.");

            try
            {
                await _service.LinkClientAsync(contactId, clientId);

                return AjaxSuccess();
            }
            catch (InvalidOperationException ex)
            {
                return AjaxError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to link client {ClientId} to contact {ContactId}",
                    clientId,
                    contactId);

                return AjaxServerError("Failed to link client.");
            }
        }

        [HttpPost("unlink-client")]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public async Task<IActionResult> UnlinkClient(int contactId, int clientId)
        {
            if (contactId <= 0 || clientId <= 0)
                return AjaxError("Invalid contact or client id.");

            try
            {
                await _service.UnlinkClientAsync(contactId, clientId);

                return AjaxSuccess();
            }
            catch (InvalidOperationException ex)
            {
                return AjaxError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to unlink client {ClientId} from contact {ContactId}",
                    clientId,
                    contactId);

                return AjaxServerError("Failed to unlink client.");
            }
        }

        [HttpGet("search-available-clients")]
        [AjaxOnly]
        public async Task<IActionResult> SearchAvailableClients(
            int contactId,
            string query,
            int skip = 0,
            int take = 5)
        {
            if (contactId <= 0)
                return AjaxError("Invalid contact id.");

            query = (query ?? string.Empty).Trim();
            if (query.Length < 3)
                return Json(new { items = Array.Empty<object>(), hasMore = false });

            var effectiveTake = take <= 0 ? 5 : take;
            var matches = await _service.SearchAvailableClientsAsync(
                contactId,
                query,
                skip < 0 ? 0 : skip,
                effectiveTake + 1);

            var hasMore = matches.Count > effectiveTake;
            var items = matches.Take(effectiveTake);

            return Json(new { items, hasMore });
        }
    }
}