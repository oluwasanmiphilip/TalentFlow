// File Path: TalentFlow.Application.Common.Interfaces/IRefreshTokenRepository.cs

using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IRefreshTokenRepository
    {
        RefreshToken? GetByToken(string token);
        void Save(RefreshToken refreshToken);
        void Revoke(string token);
    }
}
