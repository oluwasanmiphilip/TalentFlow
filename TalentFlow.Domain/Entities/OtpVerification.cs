namespace TalentFlow.Domain.Entities
{
    public class OtpVerification
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string CodeHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsUsed { get; set; } = false;

        public int AttemptCount { get; set; } = 0;
    }
}