using MediatR;
using TalentFlow.Domain.Events;

namespace TalentFlow.Application.Users.Events
{
    public class UserProfileUpdatedNotification : INotification
    {
        public TalentFlow.Domain.Events.UserProfileUpdatedDomainEvent DomainEvent { get; }

        public UserProfileUpdatedNotification(TalentFlow.Domain.Events.UserProfileUpdatedDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
