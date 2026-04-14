using MediatR;

namespace TalentFlow.Application.Users.Events
{
    /// <summary>
    /// Wraps the UserCreatedEvent so it can be published via MediatR.
    /// </summary>
    public class UserCreatedNotification : INotification
    {
        public UserCreatedEvent DomainEvent { get; }

        public UserCreatedNotification(UserCreatedEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
