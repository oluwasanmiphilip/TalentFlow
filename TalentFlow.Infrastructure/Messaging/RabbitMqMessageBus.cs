using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Messaging
{
    public class RabbitMqMessageBus : IMessageBus
    {
        private readonly IConnection _connection;

        public RabbitMqMessageBus(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishAsync<T>(T message)
            where T : class
        {
            await using var channel =
                await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: typeof(T).Name,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: typeof(T).Name,
                mandatory: false,
                basicProperties: properties,
                body: body);
        }
    }
}