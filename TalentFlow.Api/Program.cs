using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Courses.Events;
using TalentFlow.Application.Notifications;
using TalentFlow.Application.Notifications.Commands;
using TalentFlow.Infrastructure.Events;
using TalentFlow.Infrastructure.Repositories;
using TalentFlow.Persistence;
using TalentFlow.Persistence.Interceptors;
using TalentFlow.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register MediatR (scan Application assembly for handlers)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CourseCreatedEvent>();
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<SendNotificationCommandHandler>();
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<EnrollCourseCommandHandler>();
});

//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssemblyContaining<CourseCreatedEvent>();
//});


// Register DbContext with interceptor
builder.Services.AddScoped<DomainEventSaveChangesInterceptor>();

builder.Services.AddDbContext<TalentFlowDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<DomainEventSaveChangesInterceptor>());
});

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Kafka producer
var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
var producer = new ProducerBuilder<string, string>(config).Build();
builder.Services.AddSingleton<IProducer<string, string>>(producer);

// Register NotificationService
builder.Services.AddScoped<INotificationService, NotificationService>();

// EventStream publishers
builder.Services.AddScoped<IEventStreamPublisher, EventStreamPublisher>();
builder.Services.AddScoped<IEventStreamPublisher, KafkaEventStreamPublisher>();

//Authentification
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


/// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TalentFlow API v1", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "TalentFlow API v2", Version = "v2" });
    c.SwaggerDoc("v3", new OpenApiInfo { Title = "TalentFlow API v3", Version = "v3" });
    c.SwaggerDoc("v4", new OpenApiInfo { Title = "TalentFlow API v4", Version = "v4" });
    c.SwaggerDoc("v5", new OpenApiInfo { Title = "TalentFlow API v5", Version = "v5" });
});

var app = builder.Build();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentFlow API v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "TalentFlow API v2");
    c.SwaggerEndpoint("/swagger/v3/swagger.json", "TalentFlow API v3");
    c.SwaggerEndpoint("/swagger/v4/swagger.json", "TalentFlow API v4");
    c.SwaggerEndpoint("/swagger/v5/swagger.json", "TalentFlow API v5");
    c.RoutePrefix = "swagger"; // UI available at /swagger
});

// Map Auth
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();
app.Run();

