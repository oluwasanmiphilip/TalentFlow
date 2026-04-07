namespace TalentFlow.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid learnerId, string email, string role);
    }
}
