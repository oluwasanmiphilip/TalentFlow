using System;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IRefreshTokenRepository
    {
        RefreshToken? GetByToken(string token);
        void Save(RefreshToken refreshToken);
        void Revoke(string token);

        // New method: revoke all tokens for a given user
        void RevokeAllForUser(Guid userId);
    }
}
