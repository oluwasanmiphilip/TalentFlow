// File: src/TalentFlow.Api/Program.cs

using Asp.Versioning;
using DotNetEnv;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using Serilog;
using StackExchange.Redis;
using System.Text;
using TalentFlow.Infrastructure.Jobs;
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
using TalentFlow.Infrastructure.Configuration;
using TalentFlow.Infrastructure.Email;
using TalentFlow.Infrastructure.Notifications;
using TalentFlow.Infrastructure.Security;
using TalentFlow.Infrastructure.Services;
using TalentFlow.Infrastructure.Sms;
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

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
});

// ============================
// CONTROLLERS
// ============================
builder.Services.AddControllers();

// ============================
// HTTP CLIENT
// ============================
builder.Services.AddHttpClient();

// ============================
// DOMAIN EVENT DISPATCHER
// ============================
//builder.Services.AddScoped<DomainEventDispatcher>();

// ============================
// REPOSITORIES
// ============================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<OtpJobService>();

// ============================
// FILE STORAGE
// ============================
builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// ============================
// SMTP CONFIG
// ============================
builder.Services.Configure<SmtpSettings>(options =>
{
    options.Server =
        builder.Configuration["SMTP_SERVER"] ?? "localhost";

    options.Port =
        int.TryParse(builder.Configuration["SMTP_PORT"], out var port)
            ? port
            : 25;

    options.SenderName =
        builder.Configuration["SMTP_SENDER_NAME"]
            ?? "TalentFlow";

    options.SenderEmail =
        builder.Configuration["SMTP_SENDER_EMAIL"]
            ?? "no-reply@talentflow.com";

    options.Username =
        builder.Configuration["SMTP_USERNAME"] ?? "";

    options.Password =
        builder.Configuration["SMTP_PASSWORD"] ?? "";
});

// ============================
// EMAIL SERVICE
// ============================
builder.Services.AddTransient<IEmailService>(sp =>
{
    var settings =
        sp.GetRequiredService<IOptions<SmtpSettings>>().Value;

    return new SmtpEmailService(settings);
});

// ============================
// SMS SERVICE
// ============================
builder.Services.AddTransient<ISmsService>(sp =>
{
    var settings =
        sp.GetRequiredService<IOptions<SmtpSettings>>().Value;

    var logger =
        sp.GetRequiredService<ILogger<SmtpSmsService>>();

    return new SmtpSmsService(settings, logger);
});

// ============================
// SERVICES
// ============================
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<OtpDeliveryHandler>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<ICourseProgressRepository, CourseProgressRepository>();
builder.Services.AddScoped<ILeanersProgressRepository, LessonProgressRepository>();
builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<ILearningWorkRepository, LearningWorkRepository>();

// ============================
// HANGFIRE + REDIS
// ============================
var redisUrl =
    Environment.GetEnvironmentVariable("REDIS_URL");

if (string.IsNullOrWhiteSpace(redisUrl))
{
    throw new Exception("REDIS_URL environment variable is missing");
}

builder.Services.AddHangfire(config =>
{
    config.UseRedisStorage(redisUrl);
});

builder.Services.AddHangfireServer();

// ============================
// NOTIFICATION
// ============================
builder.Services.AddScoped<INotificationService, NotificationService>();

// ============================
// MEDIATR
// ============================
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(RegisterUserCommand).Assembly
    );
});

// ============================
// JWT AUTH
// ============================
var jwtSecret =
    builder.Configuration["Jwt:Production:Secret"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new Exception("JWT Secret is missing");
}

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,

                IssuerSigningKey =
                    new SymmetricSecurityKey(key),

                RoleClaimType =
                    System.Security.Claims.ClaimTypes.Role
            };
    });

// ============================
// API VERSIONING
// ============================
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);

        options.AssumeDefaultVersionWhenUnspecified = true;

        options.ReportApiVersions = true;
    });

// ============================
// CORS
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins =
            builder.Configuration
                .GetSection("AllowedOrigins")
                .Get<string[]>()
            ?? new[]
            {
                "http://localhost:5173",
                "https://talent-flow-kappa-six.vercel.app"
            };

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================
// SWAGGER / OPENAPI
// ============================
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "TalentFlow API";

    config.Version = "v1";

    config.AddSecurity("JWT",
        new NSwag.OpenApiSecurityScheme
        {
            Type =
                NSwag.OpenApiSecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            Name = "Authorization",

            In =
                NSwag.OpenApiSecurityApiKeyLocation.Header,

            Description =
                "Type: Bearer {your JWT token}"
        });

    config.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT")
    );
});

// ============================
// DATABASE CONFIG
// ============================
var connectionString =
    builder.Configuration.GetConnectionString("Production")
    ?? builder.Configuration["ConnectionStrings:Production"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception(
        "Production database connection string is missing");
}

builder.Services.AddDbContext<TalentFlowDbContext>(
    (serviceProvider, options) =>
    {
        options.UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(5);
            });

        options.UseApplicationServiceProvider(serviceProvider);
    });

// ============================
// BUILD APP
// ============================
var app = builder.Build();

// ============================
// MIDDLEWARE
// ============================
app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<AuthMiddleware>();

// ============================
// HANGFIRE DASHBOARD
// ============================
app.UseHangfireDashboard("/hangfire");

// ============================
// CORS
// ============================
app.UseCors("AllowFrontend");

// ============================
// OPENAPI / SWAGGER
// ============================
app.UseOpenApi();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/swagger";

    settings.DocumentPath =
        "/swagger/v1/swagger.json";
});

// ============================
// ROUTING
// ============================
app.UseRouting();

// ============================
// AUTH
// ============================
app.UseAuthentication();

app.UseAuthorization();

// ============================
// STATIC FILES
// ============================
app.UseStaticFiles();

// ============================
// ENDPOINTS
// ============================
app.MapControllers();

app.MapGet("/",
    () => Results.Ok("TalentFlow API Running"));

app.MapGet("/health",
    () => Results.Ok("Healthy"));

// ============================
// START APP
// ============================
var port =
    Environment.GetEnvironmentVariable("PORT")
    ?? "10000";

Console.WriteLine(
    $"Running in Production on port {port}");

app.Run($"http://0.0.0.0:{port}");