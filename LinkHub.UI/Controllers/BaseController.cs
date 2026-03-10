using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using LinkHub.UI.Models;

namespace LinkHub.UI.Controllers
{
    public abstract class BaseController : Controller
    {
       protected bool IsAjaxRequest()
        {
            return HttpContext.Request.Headers
                .TryGetValue("X-Requested-With", out var value)
                && value == "XMLHttpRequest";
        }

        protected IActionResult AjaxValidationError(ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
                success = false,
                errors
            });
        }

        protected IActionResult AjaxSuccess()
        {
            return Json(ErrorViewModel.Ok());
        }

        protected IActionResult AjaxError(string message)
        {
            return BadRequest(ErrorViewModel.Fail(message));
        }

        protected IActionResult AjaxServerError(string message = "Server error.")
        {
            return StatusCode(500, ErrorViewModel.Fail(message));
        }
    }
}
