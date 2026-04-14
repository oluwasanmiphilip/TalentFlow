using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        RefreshToken GenerateRefreshToken(Guid id, string email, string role);
        string GenerateToken(Guid learnerId, string email, string role);
    }
}
