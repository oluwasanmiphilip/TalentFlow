using MediatR;

namespace TalentFlow.Application.Users.Events
{
    public class UserProfileUpdatedEvent : INotification
    {
        public Guid UserId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserProfileUpdatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
