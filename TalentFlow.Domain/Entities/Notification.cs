using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;

namespace TalentFlow.Domain.Entities
{
    public class Notification : EntityBase
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Message { get; private set; }
        public DateTime SentAt { get; private set; }

        private Notification() { } // EF Core

        public Notification(Guid userId, string message)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Message = message;
            SentAt = DateTime.UtcNow;

            AddDomainEvent(new NotificationSentDomainEvent(this));
        }

        // ✅ New domain method
        public void MarkAsSent()
        {
            SentAt = DateTime.UtcNow;
            AddDomainEvent(new NotificationSentDomainEvent(this));
        }
    }
}
