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
// DATABASE CONFIG (FIXED)
// ============================
string BuildConnectionString(string databaseUrl)
{
    if (string.IsNullOrEmpty(databaseUrl))
        throw new Exception("DATABASE_URL is not set");

    // 🔥 Normalize Render URL
    databaseUrl = databaseUrl.Replace("postgresql://", "postgres://");

    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    var username = userInfo[0];
    var password = Uri.UnescapeDataString(userInfo[1]);
    var database = uri.AbsolutePath.TrimStart('/');

    return $"Host={uri.Host};Port=5432;Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
}

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

var connectionString = !string.IsNullOrEmpty(databaseUrl)
    ? new Npgsql.NpgsqlConnectionStringBuilder(databaseUrl)
    {
        SslMode = Npgsql.SslMode.Require,
        
    }.ToString()
    : builder.Configuration.GetConnectionString("DefaultConnection");

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
// JWT AUTHENTICATION (FIXED)
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
    options.RequireHttpsMetadata = true; // 🔥 FIXED
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
// CORS (DEBUG-FRIENDLY)
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ============================
// BUILD APP
// ============================
var app = builder.Build();

// ============================
// MIDDLEWARE PIPELINE (CLEAN)
// ============================
app.UseCors("AllowFrontend");

app.UseExceptionHandler("/error");
app.Map("/error", () => Results.Problem("An error occurred"));

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentFlow API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseRouting(); // 🔥 IMPORTANT

app.UseAuthentication();
app.UseAuthorization();

// ============================
// ROUTES
// ============================
app.MapControllers();
app.MapGet("/", () => Results.Ok("TalentFlow API Running"));
app.MapGet("/health", () => Results.Ok("Healthy"));


// ============================
// PORT (RENDER)
// ============================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");