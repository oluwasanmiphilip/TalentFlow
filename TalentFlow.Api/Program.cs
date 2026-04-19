using Asp.Versioning;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Sprache;
using System.Text;
using TalentFlow.API.Middleware;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Services;
using TalentFlow.Application.CourseProgress.Repositories;
using TalentFlow.Application.Instructors.Queries;
using TalentFlow.Application.LeanersProgress.Commands;
using TalentFlow.Application.LeanersProgress.Repositories;
using TalentFlow.Application.Otp.Handlers;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Users.Validators;
using TalentFlow.Infrastructure.Auth;
using TalentFlow.Infrastructure.Email;
using TalentFlow.Infrastructure.Events;
using TalentFlow.Infrastructure.Messaging;
using TalentFlow.Infrastructure.Notifications;
using TalentFlow.Infrastructure.Security;
using TalentFlow.Infrastructure.Services;
using TalentFlow.Infrastructure.Sms;
using TalentFlow.Persistence;
using TalentFlow.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================
// CONFIG LOAD (UNCHANGED)
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
var connectionString = builder.Configuration.GetSection("ConnectionStrings")["Production"];

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Database connection string (Production) is missing");
}

builder.Services.AddDbContext<TalentFlowDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5));
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
// SMTP CONFIG (🔥 FIXED)
// ============================
builder.Services.Configure<SmtpSettings>(options =>
{
    options.Server = builder.Configuration["SMTP_SERVER"]
        ?? throw new Exception("SMTP_SERVER not set");

    options.Port = int.Parse(builder.Configuration["SMTP_PORT"]
        ?? throw new Exception("SMTP_PORT not set"));

    options.SenderName = builder.Configuration["SMTP_SENDER_NAME"]
        ?? throw new Exception("SMTP_SENDER_NAME not set");

    options.SenderEmail = builder.Configuration["SMTP_SENDER_EMAIL"]
        ?? throw new Exception("SMTP_SENDER_EMAIL not set");

    options.Username = builder.Configuration["SMTP_USERNAME"]
        ?? throw new Exception("SMTP_USERNAME not set");

    options.Password = builder.Configuration["SMTP_PASSWORD"]
        ?? throw new Exception("SMTP_PASSWORD not set");
});

// Register Email + SMS
builder.Services.AddTransient<IEmailService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
    return new SmtpEmailService(settings);
});


//SMPT SERVICE
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


// ============================
// MESSAGING
// ============================
var rabbitSection = builder.Configuration.GetSection("RabbitMQ:Production");

if (!int.TryParse(rabbitSection["Port"], out var rabbitPort))
{
    throw new Exception("RabbitMQ Port is not configured correctly");
}

var rabbitHost = rabbitSection["Host"];
var rabbitUser = rabbitSection["UserName"];
var rabbitPass = rabbitSection["Password"];

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
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
cfg.RegisterServicesFromAssembly(typeof(SaveLoginTokenCommand).Assembly);
cfg.RegisterServicesFromAssembly(typeof(UpdateVideoPositionCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAllInstructorsQuery).Assembly);
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GenerateOtpCommandHandler).Assembly)
);

// ============================
// JWT AUTH
// ============================
var jwtSecret = builder.Configuration["Jwt:Production:Secret"];

if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("JWT Secret not configured");
}

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
// AUTHORIZATION
// ============================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireInstructor", p => p.RequireRole("Instructor"));
    options.AddPolicy("RequireLearner", p => p.RequireRole("Learner"));
});

// ============================
// CORS
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ============================
// API VERSIONING
// ============================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// ============================
// SWAGGER
// ============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "TalentFlow API";
    config.Version = "v1";

    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Name = "Authorization",
        Description = "Enter: Bearer {your JWT token}"
    });

    config.OperationProcessors.Add(
        new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("Bearer")
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
app.UseSwaggerUi(settings => settings.Path = "/swagger");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok("TalentFlow API Running"));
app.MapGet("/health", () => Results.Ok("Healthy"));

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Console.WriteLine($"Running in Production on port {port}");

app.Run($"http://0.0.0.0:{port}");