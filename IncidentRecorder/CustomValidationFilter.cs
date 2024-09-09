using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errorMessages = context.ModelState
                .SelectMany(ms => ms.Value.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var errorResponse = new
            {
                Title = "One or more validation errors occurred.",
                Status = 400,
                Errors = errorMessages // Include only the error messages, without the field names.
            };

            context.Result = new BadRequestObjectResult(errorResponse);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
