namespace TalentFlow.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }

    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        void Handle(TEvent domainEvent);
    }
}
