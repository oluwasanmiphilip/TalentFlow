using System.Text.Json;
using Confluent.Kafka;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Events
{
    public class KafkaEventStreamPublisher : IEventStreamPublisher
    {
        private readonly IProducer<string, string> _producer;

        public KafkaEventStreamPublisher(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default)
        {
            // Serialize payload to JSON
            var serializedPayload = JsonSerializer.Serialize(payload);

            // Create Kafka message
            var message = new Message<string, string>
            {
                Key = eventName,
                Value = serializedPayload
            };

            // Send to Kafka topic (use eventName as topic or configure separately)
            var deliveryResult = await _producer.ProduceAsync(eventName, message, cancellationToken);

            Console.WriteLine($"Kafka Published: {eventName} => {serializedPayload} (Partition {deliveryResult.Partition}, Offset {deliveryResult.Offset})");
        }
    }
}
