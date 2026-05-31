using System;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IOtpCacheService
    {
        Task SaveOtpAsync(Guid userId, string otp, TimeSpan expiry);
        Task<string?> GetOtpAsync(Guid userId);
        Task DeleteOtpAsync(Guid userId);

        Task SetAttemptsAsync(Guid userId, int attempts);
        Task<int> GetAttemptsAsync(Guid userId);
        Task IncrementAttemptsAsync(Guid userId);
    }
}