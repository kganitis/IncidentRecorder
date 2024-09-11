using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace IncidentRecorder
{
    public class CustomValidationProblemDetailsFactory
    {
        public static ValidationProblemDetails CreateProblemDetails(ActionContext context)
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "The request contains missing or invalid data.",
                Instance = context.HttpContext.Request.Path
            };

            var errors = new Dictionary<string, List<string>>();

            foreach (var (key, value) in context.ModelState)
            {
                // Determine the sanitized key
                var sanitizedKey = key.Contains("DTO", StringComparison.OrdinalIgnoreCase) ? "$" : key;

                foreach (var error in value.Errors)
                {
                    // Sanitize error messages
                    var errorMessage = error.ErrorMessage.Contains("JSON deserialization")
                        ? Regex.Replace(error.ErrorMessage, @"JSON deserialization for type '.*?' ", "JSON deserialization ")
                        : error.ErrorMessage.Contains("Dto") ? null : error.ErrorMessage;

                    // Skip null or empty messages
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        // Append the error messages to the same key if it already exists
                        if (errors.ContainsKey(sanitizedKey))
                        {
                            errors[sanitizedKey].Add(errorMessage);
                        }
                        else
                        {
                            errors[sanitizedKey] = new List<string> { errorMessage };
                        }
                    }
                }
            }

            // Assign filtered errors to problemDetails
            problemDetails.Errors = errors.ToDictionary(k => k.Key, k => k.Value.ToArray());

            return problemDetails;
        }
    }
}
