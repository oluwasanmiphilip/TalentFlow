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

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("⚠️ WARNING: Database connection string is missing");
}

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

// ============================
// FILE STORAGE
// ============================
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("FileStorage"));
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
    options.Server = builder.Configuration["SMTP_SERVER"] ?? "localhost";
    options.Port = int.TryParse(builder.Configuration["SMTP_PORT"], out var port) ? port : 25;
    options.SenderName = builder.Configuration["SMTP_SENDER_NAME"] ?? "TalentFlow";
    options.SenderEmail = builder.Configuration["SMTP_SENDER_EMAIL"] ?? "no-reply@talentflow.com";
    options.Username = builder.Configuration["SMTP_USERNAME"] ?? "";
    options.Password = builder.Configuration["SMTP_PASSWORD"] ?? "";
});

builder.Services.AddTransient<IEmailService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
    return new SmtpEmailService(settings);
});

builder.Services.AddTransient<ISmsService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
    return new SmtpSmsService(settings);
});

// ============================
// SERVICES
// ============================
builder.Services.AddScoped<IEventStreamPublisher, EventStreamPublisher>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<OtpDeliveryHandler>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICourseProgressRepository, CourseProgressRepository>();
builder.Services.AddScoped<ILeanersProgressRepository, LessonProgressRepository>();

builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ILearningWorkRepository, LearningWorkRepository>();

// ============================
// MESSAGING
// ============================
var rabbitSection = builder.Configuration.GetSection("RabbitMQ:Production");

var rabbitHost = rabbitSection["Host"] ?? "localhost";
var rabbitUser = rabbitSection["UserName"] ?? "guest";
var rabbitPass = rabbitSection["Password"] ?? "guest";
var rabbitPort = int.TryParse(rabbitSection["Port"], out var rp) ? rp : 5672;

builder.Services.AddSingleton<IMessageBus>(sp =>
    new RabbitMqMessageBus(rabbitHost, rabbitPort, rabbitUser, rabbitPass));

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

var jwtSecret = builder.Configuration["Jwt:Production:Secret"] ?? "default_dev_secret_key";

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

// ============================
// CORS
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? new[]
            {
                "http://localhost:5173",
                "https://talent-flow-kappa-six.vercel.app"
            };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ============================
// SWAGGER (✅ FIXED)
// ============================
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "TalentFlow API";
    config.Version = "v1";

    // 🔐 ADD JWT SECURITY DEFINITION
    config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Type: Bearer {your JWT token}"
    });

    // 🔐 APPLY SECURITY TO ALL ENDPOINTS
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

// ✅ FIXED Swagger config
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