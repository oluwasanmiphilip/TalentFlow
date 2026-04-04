using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class NotificationSentDomainEvent : IDomainEvent
    {
        public Notification Notification { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public NotificationSentDomainEvent(Notification notification)
        {
            Notification = notification;
        }
    }
}
