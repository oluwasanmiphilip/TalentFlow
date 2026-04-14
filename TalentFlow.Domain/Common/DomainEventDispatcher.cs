namespace TalentFlow.Domain.Common
{
    public class DomainEventDispatcher
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();

        public void Register<TEvent>(IDomainEventHandler<TEvent> handler) where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<object>();
            }
            _handlers[eventType].Add(handler);
        }

        public void Dispatch(IDomainEvent domainEvent)
        {
            var eventType = domainEvent.GetType();
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((dynamic)handler).Handle((dynamic)domainEvent);
                }
            }
        }
    }
}
