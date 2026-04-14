using System.Collections.Generic;

namespace TalentFlow.Domain.Common
{
    public abstract class EntityBase
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent @event)
        {
            _domainEvents.Add(@event);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
