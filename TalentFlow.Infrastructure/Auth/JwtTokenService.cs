// File Path: TalentFlow.Infrastructure/Auth/JwtTokenService.cs

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Infrastructure.Auth
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Guid learnerId, string email, string roleName)
        {
            var claims = new[]
            {
                new Claim("learner_id", learnerId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, roleName)
            };

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

        // ✅ Wrapper for consistency
        public string GenerateAToken(Guid learnerId, string email, string roleName)
        {
            return GenerateToken(learnerId, email, roleName);
        }

        public RefreshToken GenerateRefreshToken(Guid userId, string email, string role)
        {
            return new RefreshToken
            {
                UserId = userId,
                Email = email,
                Role = role,
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
