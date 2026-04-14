using System;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class NotificationSentEvent : DomainEvent
    {
        public Notification Notification { get; }

        public NotificationSentEvent(Notification notification)
        {
            Notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }
    }
}
