using MediatR;
using TalentFlow.Domain.Events;

namespace TalentFlow.Application.Users.Events
{
    public class UserProfileUpdatedNotification : INotification
    {
        public TalentFlow.Domain.Events.UserProfileUpdatedEvent DomainEvent { get; }

        public UserProfileUpdatedNotification(TalentFlow.Domain.Events.UserProfileUpdatedEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
