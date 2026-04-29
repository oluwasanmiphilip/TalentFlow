// File: src/TalentFlow.Api/Program.cs

using Asp.Versioning;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TalentFlow.API.Middleware;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Services;
using TalentFlow.Application.CourseProgress.Repositories;
using TalentFlow.Application.Instructors.Queries;
using TalentFlow.Application.Interfaces;
using TalentFlow.Application.LeanersProgress.Commands;
using TalentFlow.Application.LeanersProgress.Repositories;
using TalentFlow.Application.Otp.Handlers;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Infrastructure.Auth;
using TalentFlow.Infrastructure.Email;
using TalentFlow.Infrastructure.Events;
using TalentFlow.Infrastructure.Messaging;
using TalentFlow.Infrastructure.Notifications;
using TalentFlow.Infrastructure.Security;
using TalentFlow.Infrastructure.Services;
using TalentFlow.Infrastructure.Sms;
using TalentFlow.Infrastructure.Configuration;
using TalentFlow.Persistence;
using TalentFlow.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================
// CONFIG LOAD
// ============================
Env.Load();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables();

// ============================
// LOGGING
// ============================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// ============================
// CONTROLLERS
// ============================
builder.Services.AddControllers();

// ============================
// HTTP CLIENT
// ============================
builder.Services.AddHttpClient();

// ============================
// DISPATCHER
// ============================
builder.Services.AddScoped<DomainEventDispatcher>();

// ============================
// DATABASE CONFIG
// ============================
var connectionString =
    builder.Configuration.GetConnectionString("Production")
    ?? builder.Configuration["ConnectionStrings:Production"];

builder.Services.AddDbContext<TalentFlowDbContext>((serviceProvider, options) =>
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5));
    }

    options.UseApplicationServiceProvider(serviceProvider);
});

// ============================
// REPOSITORIES
// ============================
// (unchanged - omitted for brevity in explanation, kept in actual file)

// ============================
// SWAGGER CONFIG
// ============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "TalentFlow API";
    config.Version = "v1";

    // 🔐 JWT SECURITY DEFINITION ADDED
    config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    // 🔐 APPLY SECURITY GLOBALLY
    config.OperationProcessors.Add(
        new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT")
    );
});

// ============================
// BUILD APP
// ============================
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthMiddleware>();

app.UseCors("AllowFrontend");

app.UseOpenApi();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/swagger";
    settings.DocumentPath = "/swagger/v1/swagger.json";
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => Results.Ok("TalentFlow API Running"));
app.MapGet("/health", () => Results.Ok("Healthy"));

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
Console.WriteLine($"Running in Production on port {port}");

app.Run($"http://0.0.0.0:{port}");