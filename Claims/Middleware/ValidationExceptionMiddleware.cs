using System.Net;
using System.Text.Json;
using Claims.Validation;

namespace Claims.Middleware
{
    /// <summary>
    /// Catches <see cref="ValidationException"/> thrown anywhere downstream in the pipeline
    /// (typically from a service, after a validator ran) and turns it into a 400 Bad Request
    /// with the list of validation errors, instead of letting it bubble up as a 500.
    /// </summary>
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors = ex.Errors }));
            }
        }
    }
}
