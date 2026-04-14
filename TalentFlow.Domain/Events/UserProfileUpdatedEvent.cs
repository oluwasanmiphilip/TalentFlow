using System;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class UserProfileUpdatedEvent : DomainEvent
    {
        public User User { get; }

        public UserProfileUpdatedEvent(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
