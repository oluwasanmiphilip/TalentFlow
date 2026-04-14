using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Messaging
{
    public class RabbitMqMessageBus : IMessageBus, IDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqMessageBus(string host, int port, string user, string pass)
        {
            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                UserName = user,
                Password = pass
            };
            // RabbitMQ client only supports synchronous connection creation
            _connection = factory.CreateConnection();
        }

        public Task PublishAsync<T>(T message) where T : class
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(
                queue: typeof(T).Name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // ensure message survives broker restart

            channel.BasicPublish(
                exchange: "",
                routingKey: typeof(T).Name,
                basicProperties: properties,
                body: body
            );

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
