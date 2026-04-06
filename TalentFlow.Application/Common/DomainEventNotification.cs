using MediatR;
using TalentFlow.Domain.Common;

namespace TalentFlow.Application.Common
{
    /// <summary>
    /// Wraps a domain event so it can be published via MediatR.
    /// </summary>
    public class DomainEventNotification<TDomainEvent> : INotification
        where TDomainEvent : IDomainEvent
    {
        public TDomainEvent DomainEvent { get; }

        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
