namespace TalentFlow.Application.Common.Interfaces
{
    public interface IOtpRateLimiter
    {
        Task<bool> CanSendAsync(Guid userId);
        Task MarkSentAsync(Guid userId);
    }
}