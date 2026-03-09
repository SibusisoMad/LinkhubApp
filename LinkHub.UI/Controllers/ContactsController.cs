using LinkHub.UI.Models;
using LinkHub.UI.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LinkHub.UI.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
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
                return RedirectToAction(nameof(Create));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to create contact. Email may already exist or be invalid.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await LoadEditModel(id);
            return model == null ? NotFound() : View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ContactUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return await ReloadEditView(model);

            var success = await _contactService.UpdateContactAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to update contact. Email may already exist or be invalid.");
                return await ReloadEditView(model);
            }

            TempData["ContactUpdatedMessage"] = "Contact updated successfully.";
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> LinkClient(int contactId, int clientId)
        {
            await _contactService.LinkClientAsync(contactId, clientId);
            TempData["SuccessMessage"] = "Client linked successfully.";
            return RedirectToAction(nameof(Edit), new { id = contactId });
        }

        [HttpPost]
        public async Task<IActionResult> Unlink(int contactId, int clientId)
        {
            await _contactService.UnlinkClientAsync(contactId, clientId);
            TempData["SuccessMessage"] = "Client unlinked successfully.";
            return RedirectToAction(nameof(Edit), new { id = contactId });
        }

        private async Task<ContactEditViewModel?> LoadEditModel(int id)
        {
            var model = await _contactService.GetContactEditViewModelAsync(id);
            if (model == null)
                return null;

            model.ClientsSuccessMessage = TempData["SuccessMessage"]?.ToString();
            model.ContactUpdatedMessage = TempData["ContactUpdatedMessage"]?.ToString();

            return model;
        }

        private async Task<IActionResult> ReloadEditView(ContactUpdateViewModel model)
        {
            var editModel = await LoadEditModel(model.Id);
            if (editModel == null)
                return NotFound();

            editModel.Name = model.Name;
            editModel.Surname = model.Surname;
            editModel.Email = model.Email;

            return View("Edit", editModel);
        }
    }
}
