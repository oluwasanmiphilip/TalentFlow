using System.Security.Cryptography;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Otp.Services
{
    public class OtpService
    {
        private readonly IOtpCacheService _cache;

        private const int ExpiryMinutes = 5;
        private const int MaxAttempts = 3;

        public OtpService(IOtpCacheService cache)
        {
            _cache = cache;
        }

        public async Task<string> GenerateOtpAsync(Guid userId)
        {
            var otp =
                RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

            await _cache.SaveOtpAsync(
                userId,
                otp,
                TimeSpan.FromMinutes(ExpiryMinutes));

            await _cache.SetAttemptsAsync(userId, 0);

            return otp;
        }

        public async Task<bool> ValidateAsync(
            Guid userId,
            string code)
        {
            var stored =
                await _cache.GetOtpAsync(userId);

            if (stored == null)
                return false;

            var attempts =
                await _cache.GetAttemptsAsync(userId);

            if (attempts >= MaxAttempts)
            {
                await _cache.DeleteOtpAsync(userId);
                return false;
            }

            if (stored != code)
            {
                await _cache.IncrementAttemptsAsync(userId);
                return false;
            }

            await _cache.DeleteOtpAsync(userId);

            return true;
        }
    }
}