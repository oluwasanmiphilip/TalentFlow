using System;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class NotificationReadEvent : DomainEvent
    {
        public Notification Notification { get; }

        public NotificationReadEvent(Notification notification)
        {
            Notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }
    }
}
