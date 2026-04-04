using MediatR;

namespace TalentFlow.Application.Users.Events
{
    public class UserCreatedEvent : INotification
    {
        public Guid UserId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserCreatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
