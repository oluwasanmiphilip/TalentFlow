using StackExchange.Redis;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Security
{
    public class OtpRateLimiter : IOtpRateLimiter
    {
        private readonly IDatabase _db;

        private const int RESEND_COOLDOWN_SECONDS = 60;

        public OtpRateLimiter(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        private string Key(Guid userId) => $"otp:cooldown:{userId}";

        public async Task<bool> CanSendAsync(Guid userId)
        {
            return !await _db.KeyExistsAsync(Key(userId));
        }

        public async Task MarkSentAsync(Guid userId)
        {
            await _db.StringSetAsync(
                Key(userId),
                "1",
                TimeSpan.FromSeconds(RESEND_COOLDOWN_SECONDS)
            );
        }
    }
}