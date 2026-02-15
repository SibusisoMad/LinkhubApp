using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LinkHub.UI.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await _contactService.GetContactsAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContactCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _contactService.CreateContactAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Contact created successfully.";
                return RedirectToAction("Create");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to create contact. Email may already exist or be invalid.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Unlink(int contactId, int clientId)
        {
            await _contactService.UnlinkClientAsync(contactId, clientId);
            TempData["SuccessMessage"] = "Client unlinked successfully.";
            return RedirectToAction("List");
        }
    }
}
