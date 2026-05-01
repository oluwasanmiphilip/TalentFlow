using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TalentFlow.Application.Common.Exceptions;

namespace TalentFlow.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";

                int statusCode;
                object payload;

                if (ex is DuplicateEmailException dupEx)
                {
                    statusCode = (int)HttpStatusCode.Conflict;
                    payload = new
                    {
                        success = false,
                        data = (object?)null,
                        message = "Email already registered",
                        statusCode,
                        errors = new { Email = new[] { $"The email '{dupEx.Email}' is already in use." } }
                    };
                }
                else if (ex is NotFoundException nfEx)
                {
                    statusCode = (int)HttpStatusCode.NotFound;
                    payload = new
                    {
                        success = false,
                        data = (object?)null,
                        message = nfEx.Message,
                        statusCode,
                        errors = new { Resource = new[] { nfEx.Message } }
                    };
                }
                else if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    payload = new
                    {
                        success = false,
                        data = (object?)null,
                        message = "Bad request",
                        statusCode,
                        errors = new { General = new[] { ex.Message } }
                    };
                }
                else
                {
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    var message = "An unexpected error occurred";
                    payload = new
                    {
                        success = false,
                        data = (object?)null,
                        message,
                        statusCode
                    };
                }

                context.Response.StatusCode = statusCode;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                if (_env.IsDevelopment())
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(payload, options), options)
                               ?? new Dictionary<string, object>();
                    dict["detail"] = ex.Message;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(dict, options));
                }
                else
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(payload, options));
                }
            }
        }
    }
}
