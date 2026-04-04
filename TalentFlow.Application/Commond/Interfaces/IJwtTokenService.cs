namespace TalentFlow.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(string learnerId, string email);
    }
}
