using MediatR;
using TalentFlow.Domain.Events;

namespace TalentFlow.Application.Users.Events
{
    public class UserCreatedNotification : INotification
    {
        public TalentFlow.Domain.Events.UserCreatedDomainEvent DomainEvent { get; }

        public UserCreatedNotification(TalentFlow.Domain.Events.UserCreatedDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
