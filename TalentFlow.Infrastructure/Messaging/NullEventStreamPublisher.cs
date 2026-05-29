using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Messaging
{
    public class NullEventStreamPublisher : IEventStreamPublisher
    {
        public Task PublishAsync(
            string eventName,
            object payload,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine(
                $"⚠️ Event ignored: {eventName}");

            return Task.CompletedTask;
        }
    }
}