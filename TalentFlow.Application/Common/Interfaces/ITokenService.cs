using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Guid userId, string email, string role);
        RefreshToken GenerateRefreshToken(Guid userId, string email, string role);
    }
}
