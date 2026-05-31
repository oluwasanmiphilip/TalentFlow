using StackExchange.Redis;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Caching
{
    public class RedisOtpStore : IOtpCacheService
    {
        private readonly IDatabase _db;

        public RedisOtpStore(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        private string Key(Guid userId) => $"otp:{userId}";
        private string AttemptKey(Guid userId) => $"otp:attempts:{userId}";

        public async Task SaveOtpAsync(Guid userId, string otp, TimeSpan expiry)
        {
            var payload = new OtpPayload
            {
                Code = otp,
                Attempts = 0
            };

            await _db.StringSetAsync(
                Key(userId),
                JsonSerializer.Serialize(payload),
                expiry
            );
        }

        public async Task<string?> GetOtpAsync(Guid userId)
        {
            var data = await _db.StringGetAsync(Key(userId));

            if (!data.HasValue)
                return null;

            var obj = JsonSerializer.Deserialize<OtpPayload>(data.ToString());
            return obj?.Code;
        }

        public async Task DeleteOtpAsync(Guid userId)
        {
            await _db.KeyDeleteAsync(Key(userId));
            await _db.KeyDeleteAsync(AttemptKey(userId));
        }

        // ✅ FIX 1: missing method
        public async Task SetAttemptsAsync(Guid userId, int attempts)
        {
            await _db.StringSetAsync(AttemptKey(userId), attempts);
        }

        // ✅ FIX 2: missing method
        public async Task<int> GetAttemptsAsync(Guid userId)
        {
            var val = await _db.StringGetAsync(AttemptKey(userId));
            if (!val.HasValue)
                return 0;

            return (int)val;
        }

        public async Task IncrementAttemptsAsync(Guid userId)
        {
            await _db.StringIncrementAsync(AttemptKey(userId));
        }

        private class OtpPayload
        {
            public string Code { get; set; } = "";
            public int Attempts { get; set; }
        }
    }
}