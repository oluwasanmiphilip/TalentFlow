namespace TalentFlow.Application.Common.Interfaces
{
    public interface IEventStreamPublisher
    {
        Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default);
    }
}
