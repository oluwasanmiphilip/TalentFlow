using TalentFlow.Domain.Common;

namespace TalentFlow.Domain.Entities
{
    public class Notification : EntityBase
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Message { get; private set; }
        public DateTime SentAt { get; private set; }

        // Navigation property (optional, useful for EF Core)
        public User? User { get; private set; }

        private Notification() { } // EF Core

        public Notification(Guid userId, string message)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Message = message;
            SentAt = DateTime.UtcNow;

            // Raise domain-level event
            AddDomainEvent(new TalentFlow.Domain.Events.NotificationSentDomainEvent(this));
        }
    }
}
