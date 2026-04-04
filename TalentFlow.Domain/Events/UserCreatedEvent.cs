using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class UserCreatedDomainEvent : IDomainEvent
    {
        public User User { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserCreatedDomainEvent(User user)
        {
            User = user;
        }
    }
}
