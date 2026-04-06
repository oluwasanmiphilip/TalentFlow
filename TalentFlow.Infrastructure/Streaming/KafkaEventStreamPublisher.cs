using Confluent.Kafka;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

public class KafkaEventStreamPublisher : IEventStreamPublisher
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventStreamPublisher(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default)
    {
        var message = JsonSerializer.Serialize(new { Event = eventName, Payload = payload });

        await _producer.ProduceAsync(
            "notifications-topic",
            new Message<string, string>
            {
                Key = eventName,
                Value = message
            },
            cancellationToken);
    }
}