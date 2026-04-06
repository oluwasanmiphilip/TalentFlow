using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Courses.Events;
using TalentFlow.Application.Notifications.Commands;
using TalentFlow.Infrastructure.Auth;
using TalentFlow.Infrastructure.Events;
using TalentFlow.Persistence;
using TalentFlow.Persistence.Interceptors;
using TalentFlow.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Kestrel BEFORE building the app
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// 2. Service registrations
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CourseCreatedEvent).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(SendNotificationCommandHandler).Assembly);
});

builder.Services.AddScoped<DomainEventSaveChangesInterceptor>();

builder.Services.AddDbContext<TalentFlowDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<DomainEventSaveChangesInterceptor>());
});

// Repositories & Unit of Work
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Messaging provider selection
var messagingProvider = builder.Configuration["Messaging:Provider"];
if (messagingProvider == "Kafka")
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    builder.Services.AddSingleton<IProducer<string, string>>(
        new ProducerBuilder<string, string>(config).Build());
    builder.Services.AddScoped<IEventStreamPublisher, KafkaEventStreamPublisher>();
}
else
{
    builder.Services.AddScoped<IEventStreamPublisher>(sp =>
        new RabbitMqEventStreamPublisher("localhost"));
}

// Notification service
builder.Services.AddScoped<INotificationService, TalentFlow.Application.Notifications.NotificationService>();

// Authentication & Authorization
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLearner", policy =>
        policy.RequireRole("Learner"));
});

// Controllers, Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Build the app
var app = builder.Build();

// 4. Middleware pipeline
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();