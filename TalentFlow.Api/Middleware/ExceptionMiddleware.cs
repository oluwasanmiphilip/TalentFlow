using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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

                switch (ex)
                {
                    case DuplicateEmailException dupEx:
                        statusCode = (int)HttpStatusCode.Conflict;
                        payload = new
                        {
                            success = false,
                            data = (object?)null,
                            message = "Email already registered",
                            statusCode,
                            errors = new { Email = new[] { $"The email '{dupEx.Email}' is already in use." } }
                        };
                        break;

                    case NotFoundException nfEx:
                        statusCode = (int)HttpStatusCode.NotFound;
                        payload = new
                        {
                            success = false,
                            data = (object?)null,
                            message = nfEx.Message,
                            statusCode,
                            errors = new { Resource = new[] { nfEx.Message } }
                        };
                        break;

                    case ValidationException valEx:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        payload = new
                        {
                            success = false,
                            data = (object?)null,
                            message = "Validation failed",
                            statusCode,
                            errors = new { Validation = new[] { valEx.Message } }
                        };
                        break;

                    case ArgumentException or InvalidOperationException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        payload = new
                        {
                            success = false,
                            data = (object?)null,
                            message = "Bad request",
                            statusCode,
                            errors = new { General = new[] { ex.Message } }
                        };
                        break;

                    case DbUpdateException dbEx:
                        statusCode = (int)HttpStatusCode.Conflict;
                        payload = new
                        {
                            success = false,
                            data = (object?)null,
                            message = "Database update failed (duplicate or constraint violation).",
                            statusCode,
                            errors = new { Database = new[] { dbEx.InnerException?.Message ?? dbEx.Message } }
                        };
                        break;

                    case UnauthorizedAccessException unauthEx:
                        statusCode = (int)HttpStatusCode.Unauthorized;
                        payload = new
                        {
                            success = false,
                            data = (object?)null,
                            message = "Unauthorized",
                            statusCode,
                            errors = new { Auth = new[] { unauthEx.Message } }
                        };
                        break;

                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        payload = new
                        {
                            success = false,
                            data = (object?)null,
                            message = "An unexpected error occurred",
                            statusCode
                        };
                        break;
                }

                context.Response.StatusCode = statusCode;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                if (_env.IsDevelopment())
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        JsonSerializer.Serialize(payload, options), options
                    ) ?? new Dictionary<string, object>();

                    dict["detail"] = ex.ToString(); // full stack trace in dev
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
