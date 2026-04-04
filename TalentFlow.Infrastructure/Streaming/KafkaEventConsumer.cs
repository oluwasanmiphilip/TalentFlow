using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using TalentFlow.Infrastructure.Streaming;

namespace TalentFlow.Infrastructure.Streaming
{
    public class KafkaEventConsumer : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IHubContext<NotificationHub> _hubContext;

        public KafkaEventConsumer(IConsumer<Ignore, string> consumer, IHubContext<NotificationHub> hubContext)
        {
            _consumer = consumer;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("UserProfileUpdated");

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                if (result?.Message?.Value != null)
                {
                    // Push to SignalR clients
                    await _hubContext.Clients.All.SendAsync("ProfileUpdated", result.Message.Value, cancellationToken: stoppingToken);
                }
            }
        }
    }
}
