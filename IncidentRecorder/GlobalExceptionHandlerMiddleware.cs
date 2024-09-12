using System.Net;

namespace IncidentRecorder
{
    internal class GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch
            {
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var problemDetails = new
            {
                Title = "An unexpected error occurred.",
                Status = context.Response.StatusCode,
                TraceId = context.TraceIdentifier
            };

            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
