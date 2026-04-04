using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class UserProfileUpdatedDomainEvent : IDomainEvent
    {
        public User User { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserProfileUpdatedDomainEvent(User user)
        {
            User = user;
        }
    }
}
