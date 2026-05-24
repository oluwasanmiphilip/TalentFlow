using Microsoft.Extensions.Hosting;

namespace TalentFlow.Workers
{
    public class OtpWorker : BackgroundService
    {
        private readonly OtpConsumer _consumer;

        public OtpWorker(OtpConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
        {
            await _consumer.StartAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}