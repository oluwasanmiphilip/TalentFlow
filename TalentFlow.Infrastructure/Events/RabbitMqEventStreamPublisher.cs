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
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection(); // ✅ synchronous
        }

        public Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default)
        {
            using var channel = _connection.CreateModel(); // ✅ synchronous

            channel.QueueDeclare(
                queue: "notifications",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = JsonSerializer.Serialize(new { Event = eventName, Payload = payload });
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: "notifications",
                basicProperties: null,
                body: body
            );

            return Task.CompletedTask;
        }
    }
}
