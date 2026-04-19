using System.IdentityModel.Tokens.Jwt;
using TalentFlow.Application.Common.Interfaces;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()
            ?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = Guid.Parse(jwtToken.Claims.First(c => c.Type == "userId").Value);

            var user = await userRepository.GetByIdAsync(userId);

            // ✅ Enforce single-session login
            if (user?.LastLoginToken != token)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Session expired or logged in elsewhere");
                return;
            }
        }

        await _next(context);
    }
}
