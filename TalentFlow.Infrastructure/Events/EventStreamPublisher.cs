using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Events
{
    public class EventStreamPublisher : IEventStreamPublisher
    {
        public async Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default)
        {
            // For now, just log to console. Replace with Kafka, RabbitMQ, etc.
            var serializedPayload = JsonSerializer.Serialize(payload);
            Console.WriteLine($"Event Published: {eventName} => {serializedPayload}");

            // Simulate async work
            await Task.CompletedTask;
        }
    }
}
