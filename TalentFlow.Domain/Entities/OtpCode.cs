using System;

namespace TalentFlow.Domain.Entities
{
    public class OtpCode
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        public OtpCode(Guid userId, string code, DateTime expiresAt)
        {
            UserId = userId;
            Code = code;
            ExpiresAt = expiresAt;
            IsUsed = false;
        }

        public void MarkUsed() => IsUsed = true;
    }
}
