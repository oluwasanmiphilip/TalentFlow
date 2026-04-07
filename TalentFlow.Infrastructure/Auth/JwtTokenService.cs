using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Guid learnerId, string email, string roleName)
        {
            if (learnerId == Guid.Empty)
                throw new ArgumentException("LearnerId cannot be null or empty", nameof(learnerId));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("RoleName cannot be null or empty", nameof(roleName));

            var claims = new[]
            {
                new Claim("learner_id", learnerId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, roleName)
            };

            // ✅ Ensure JWT secret exists
            var secret = _configuration["Jwt:Secret"]
                         ?? throw new InvalidOperationException("JWT secret is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TalentFlow",
                audience: "TalentFlowApi",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}