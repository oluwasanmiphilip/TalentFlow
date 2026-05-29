using Asp.Versioning;
using DotNetEnv;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
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
using TalentFlow.Infrastructure.Configuration;
using TalentFlow.Infrastructure.Email;
using TalentFlow.Infrastructure.Jobs;
using TalentFlow.Infrastructure.Messaging;
using TalentFlow.Infrastructure.Notifications;
using TalentFlow.Infrastructure.Security;
using TalentFlow.Infrastructure.Services;
using TalentFlow.Infrastructure.Sms;
using TalentFlow.Persistence;
using TalentFlow.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================
// CONFIG
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
builder.Services.AddHttpClient();

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
builder.Services.AddScoped<ILearningWorkRepository, LearningWorkRepository>();
builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<ICourseProgressRepository, CourseProgressRepository>();
builder.Services.AddScoped<IEventStreamPublisher, NullEventStreamPublisher>();


// ============================
// JOBS
// ============================
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
// SMTP
// ============================
//builder.Services.AddMediatR(cfg =>
//    cfg.RegisterServicesFromAssembly(typeof(TalentFlow.Application.DependencyInjection).Assembly));

// ============================
// SMTP
// ============================
builder.Services.Configure<SmtpSettings>(options =>
{
    options.Server = builder.Configuration["SMTP_SERVER"] ?? "localhost";
    options.Port = int.TryParse(builder.Configuration["SMTP_PORT"], out var p) ? p : 25;
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
    var logger = sp.GetRequiredService<ILogger<SmtpSmsService>>();
    return new SmtpSmsService(settings, logger);
});

// ============================
// CORE SERVICES
// ============================
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ============================
// HANGFIRE (SAFE - NO REDIS)
// ============================
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage(); // SAFE FOR RENDER FREE PLAN
});

builder.Services.AddHangfireServer();

// ============================
// NOTIFICATIONS
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
// JWT
// ============================
var jwtSecret = builder.Configuration["Jwt:Production:Secret"];

if (string.IsNullOrWhiteSpace(jwtSecret))
    jwtSecret = "superlongjwtsecretkeytokenhiddenfor_dev";

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ============================
// CORS
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://talent-flow-kappa-six.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================
// SWAGGER
// ============================
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "TalentFlow API";
    config.Version = "v1";

    config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    config.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

// ============================
// DB
// ============================
var connectionString = builder.Configuration.GetConnectionString("Production");

builder.Services.AddDbContext<TalentFlowDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString))
        options.UseNpgsql(connectionString);
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

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.UseOpenApi();
app.UseSwaggerUi();

app.MapControllers();

app.MapGet("/", () => "TalentFlow API Running");
app.MapGet("/health", () => "Healthy");

// ============================
// START
// ============================
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

Console.WriteLine($"Running on port {port}");

app.Run($"http://0.0.0.0:{port}");