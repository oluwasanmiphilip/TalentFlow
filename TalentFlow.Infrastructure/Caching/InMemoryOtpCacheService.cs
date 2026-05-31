using System.Collections.Concurrent;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Caching
{
    public class InMemoryOtpCacheService : IOtpCacheService
    {
        private class OtpEntry
        {
            public string Code { get; set; } = "";
            public DateTime Expiry { get; set; }
            public int Attempts { get; set; }
        }

        private readonly ConcurrentDictionary<Guid, OtpEntry> _store = new();
        private readonly ConcurrentDictionary<Guid, DateTime> _lastSent = new();

        private static readonly TimeSpan COOLDOWN = TimeSpan.FromSeconds(60);
        public Task<bool> CanSendAsync(Guid userId)
        {
            if (!_lastSent.TryGetValue(userId, out var last))
                return Task.FromResult(true);

            return Task.FromResult(DateTime.UtcNow - last > COOLDOWN);
        }

        public Task MarkSentAsync(Guid userId)
        {
            _lastSent[userId] = DateTime.UtcNow;
            return Task.CompletedTask;
        }
        public Task SaveOtpAsync(Guid userId, string otp, TimeSpan expiry)
        {
            _store[userId] = new OtpEntry
            {
                Code = otp,
                Expiry = DateTime.UtcNow.Add(expiry),
                Attempts = 0
            };

            return Task.CompletedTask;
        }

        public Task<string?> GetOtpAsync(Guid userId)
        {
            if (_store.TryGetValue(userId, out var entry))
            {
                if (entry.Expiry < DateTime.UtcNow)
                    return Task.FromResult<string?>(null);

                return Task.FromResult<string?>(entry.Code);
            }

            return Task.FromResult<string?>(null);
        }

        public Task DeleteOtpAsync(Guid userId)
        {
            _store.TryRemove(userId, out _);
            return Task.CompletedTask;
        }

        public Task SetAttemptsAsync(Guid userId, int attempts)
        {
            if (_store.ContainsKey(userId))
                _store[userId].Attempts = attempts;

            return Task.CompletedTask;
        }

        public Task<int> GetAttemptsAsync(Guid userId)
        {
            return Task.FromResult(_store.TryGetValue(userId, out var entry)
                ? entry.Attempts
                : 0);
        }

        public Task IncrementAttemptsAsync(Guid userId)
        {
            if (_store.ContainsKey(userId))
                _store[userId].Attempts++;

            return Task.CompletedTask;
        }
    }
}