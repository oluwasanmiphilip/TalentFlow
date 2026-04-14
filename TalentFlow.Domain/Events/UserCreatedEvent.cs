using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class UserCreatedEvent : DomainEvent
    {
        public User User { get; }

        public UserCreatedEvent(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
