using LinkHub.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LinkHub.UI.Filters
{
    public class AjaxExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiFieldValidationException fieldEx)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError(fieldEx.FieldName, fieldEx.Message);

                context.Result = new JsonResult(new
                {
                    success = false,
                    validation = modelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage)
                    )
                });

                context.ExceptionHandled = true;
                return;
            }

            if (context.Exception is InvalidOperationException ex)
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    error = ex.Message
                });

                context.ExceptionHandled = true;
            }
        }
    }
}
        
