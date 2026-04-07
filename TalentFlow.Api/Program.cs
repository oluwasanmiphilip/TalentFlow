using Confluent.Kafka;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Courses.Events;
using TalentFlow.Application.Notifications.Commands;
using TalentFlow.Infrastructure.Auth;
using TalentFlow.Infrastructure.Security;
using TalentFlow.Infrastructure.Events;
using TalentFlow.Persistence;
using TalentFlow.Persistence.Interceptors;
using TalentFlow.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================
// LOGGING (IMPORTANT)
// ============================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ============================
// SERVICES
// ============================

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CourseCreatedEvent).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(SendNotificationCommandHandler).Assembly);
});

// Interceptors
builder.Services.AddScoped<DomainEventSaveChangesInterceptor>();

// Database
builder.Services.AddDbContext<TalentFlowDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<DomainEventSaveChangesInterceptor>());
});


//Seeder
builder.Services.AddScoped<RoleSeeder>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<RoleSeeder>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Password
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// JWT service
//builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// ============================
// AUTH SERVICE + AUTHENTICATION
// ============================
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new Exception("JWT Secret not configured");

// Register JwtTokenService with secret
builder.Services.AddSingleton<IJwtTokenService, TalentFlow.Infrastructure.Auth.JwtTokenService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "TalentFlow",
            ValidAudience = "TalentFlowApi",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLearner", policy =>
        policy.RequireRole("Learner"));
});

//Identity
/*builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TalentFlowDbContext>()
    .AddDefaultTokenProviders();*/


// ============================
// MESSAGING (EVENT-DRIVEN)
// ============================
var messagingProvider = builder.Configuration["Messaging:Provider"];

if (messagingProvider == "Kafka")
{
    var kafkaServers = builder.Configuration["Kafka:BootstrapServers"]
        ?? throw new Exception("Kafka BootstrapServers not configured");

    var config = new ProducerConfig
    {
        BootstrapServers = kafkaServers
    };

    builder.Services.AddSingleton<IProducer<string, string>>(
        new ProducerBuilder<string, string>(config).Build());

    builder.Services.AddScoped<IEventStreamPublisher, KafkaEventStreamPublisher>();
}
else
{
    var rabbitHost = builder.Configuration["RabbitMQ:Host"]
        ?? throw new Exception("RabbitMQ Host not configured");

    builder.Services.AddScoped<IEventStreamPublisher>(sp =>
        new RabbitMqEventStreamPublisher(rabbitHost));
}

// ============================
// NOTIFICATIONS
// ============================
builder.Services.AddScoped<INotificationService, TalentFlow.Application.Notifications.NotificationService>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================
// BUILD APP
// ============================
var app = builder.Build();
// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRolesAsync();
}

// ============================
// MIDDLEWARE PIPELINE
// ============================

// Global error handling
app.UseExceptionHandler("/error");
app.MapGet("/error", () => Results.Problem("An unexpected error occurred"));

// Favicon placeholder
app.MapGet("/favicon.ico", () => Results.NoContent());

// Health check
app.MapGet("/", () => Results.Ok("TalentFlow API is running"));
app.MapGet("/health", () => Results.Ok("Healthy"));

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentFlow API v1");
});

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// ============================
// RUN APP (Render-friendly)
// ============================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
