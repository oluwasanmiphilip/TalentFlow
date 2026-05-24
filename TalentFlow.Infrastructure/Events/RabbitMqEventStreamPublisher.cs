using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Events
{
    public class RabbitMqEventStreamPublisher : IEventStreamPublisher
    {
        private readonly IConnection _connection;

        public RabbitMqEventStreamPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishAsync(
    string eventName,
    object payload,
    CancellationToken cancellationToken = default)
        {
            await using var channel =
                await _connection.CreateChannelAsync(
                    cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: "notifications",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var message = JsonSerializer.Serialize(new
            {
                Event = eventName,
                Payload = payload
            });

            var body = Encoding.UTF8.GetBytes(message); // ✅ FIX HERE

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "notifications",
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}