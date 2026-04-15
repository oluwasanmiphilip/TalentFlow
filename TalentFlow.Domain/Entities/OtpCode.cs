using System;

namespace TalentFlow.Domain.Entities
{
    public class OtpCode
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Channel { get; set; } = "email"; // email or sms
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        // ✅ Parameterless constructor for EF Core and object initializers
        public OtpCode() { }

        // Optional convenience constructor
        public OtpCode(Guid userId, string code, string channel, DateTime expiresAt)
        {
            UserId = userId;
            Code = code;
            Channel = channel;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            IsUsed = false;
        }

        public void MarkUsed() => IsUsed = true;

        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    }
}
