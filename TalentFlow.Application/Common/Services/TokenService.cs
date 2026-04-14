using System;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.DTOs;

using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Services
{
    public class TokenService
    {
        private readonly IJwtTokenService _jwt;
        private readonly IRefreshTokenRepository _refreshRepo;

        public TokenService(IJwtTokenService jwt, IRefreshTokenRepository refreshRepo)
        {
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
            _refreshRepo = refreshRepo ?? throw new ArgumentNullException(nameof(refreshRepo));
        }

        public (string accessToken, string refreshToken) IssueTokens(UserDto user)
        {
            // revoke all existing refresh tokens for this user
            _refreshRepo.RevokeAllForUser(user.Id);

            // generate new access + refresh tokens
            var accessToken = _jwt.GenerateToken(user.Id, user.Email, user.Role);
            var refreshToken = _jwt.GenerateRefreshToken(user.Id, user.Email, user.Role);

            // persist refresh token
            _refreshRepo.Save(refreshToken);

            return (accessToken, refreshToken.Token);
        }
    }
}
