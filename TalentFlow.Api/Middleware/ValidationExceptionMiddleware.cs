using Microsoft.AspNetCore.Http;
using System.Text.Json;
using FluentValidation;

namespace TalentFlow.API.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            int retryCount = 0;
            const int maxRetries = 3;

            while (true)
            {
                try
                {
                    await _next(context);
                    break; // ✅ success, exit loop
                }
                catch (ValidationException ex)
                {
                    retryCount++;

                    if (retryCount >= maxRetries)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";

                        var errors = ex.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            error = e.ErrorMessage
                        });

                        var response = new
                        {
                            message = "Validation failed after retries",
                            errors
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        break;
                    }

                    // 🔄 wait briefly before retry
                    await Task.Delay(200 * retryCount);
                }
            }
        }
    }
}
