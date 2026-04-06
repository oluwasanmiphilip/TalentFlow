using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Events
{
    public class RabbitMqEventStreamPublisher : IEventStreamPublisher
    {
        private readonly IConnection _connection;

        public RabbitMqEventStreamPublisher(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };

            // NEW in v7
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        public async Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default)
        {
            // NEW in v7
            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "notifications",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = JsonSerializer.Serialize(new { Event = eventName, Payload = payload });
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "notifications",
                body: body
            );
        }
    }
}