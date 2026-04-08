using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Infrastructure.Auth;
using TalentFlow.Infrastructure.Events;
using TalentFlow.Infrastructure.Security;
using TalentFlow.Infrastructure.Services;
using TalentFlow.Persistence;
using TalentFlow.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================
// LOGGING
// ============================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ============================
// CONTROLLERS
// ============================
builder.Services.AddControllers();

// ============================
// SWAGGER
// ============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================
// DATABASE CONFIG (Render + Local)
// ============================
string BuildConnectionString(string databaseUrl)
{
    // Example: postgres://user:pass@host:5432/dbname
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true;";
}

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = string.IsNullOrEmpty(databaseUrl)
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : BuildConnectionString(databaseUrl);

if (string.IsNullOrEmpty(connectionString))
    throw new Exception("Database connection not configured");

builder.Services.AddDbContext<TalentFlowDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(5);
    })
);

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

// ============================
// EVENT STREAM
// ============================
builder.Services.AddScoped<IEventStreamPublisher, EventStreamPublisher>();

// ============================
// SERVICES
// ============================
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<JwtTokenService>();

// ============================
// MEDIATR
// ============================
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly);
});




// ============================
// JWT AUTHENTICATION
// ============================
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"];

if (string.IsNullOrEmpty(jwtSecret))
    throw new Exception("JWT Secret not configured");

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://your-frontend.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
// ============================
// BUILD APP
// ============================
var app = builder.Build();

// ============================
// AUTO MIGRATION
// ============================
/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TalentFlowDbContext>();
    db.Database.Migrate();
}*/




app.UseCors("AllowFrontend");

// ============================
// ERROR HANDLING
// ============================
app.UseExceptionHandler("/error");
app.MapGet("/error", () => Results.Problem("An error occurred"));

// ============================
// SWAGGER
// ============================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentFlow API v1");
    c.RoutePrefix = string.Empty;
});

// ============================
// SECURITY
// ============================
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ============================
// ROUTES
// ============================
app.MapControllers();
app.MapGet("/", () => Results.Ok("TalentFlow API Running"));
app.MapGet("/health", () => Results.Ok("Healthy"));

// ============================
// RENDER PORT BINDING
// ============================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
