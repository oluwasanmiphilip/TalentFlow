using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Messages;

namespace TalentFlow.Workers
{
    public class OtpConsumer
    {
        private readonly IConnection _connection;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public OtpConsumer(
            IConnection connection,
            IEmailService emailService,
            ISmsService smsService)
        {
            _connection = connection;
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task StartAsync(
            CancellationToken cancellationToken,
            string queueName = nameof(OtpMessage))
        {
            var channel = await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<OtpMessage>(json);

                if (message != null)
                {
                    try
                    {
                        if (message.Channel == "email")
                        {
                            await _emailService.SendOtpAsync(
                                message.Email,
                                message.Code);
                        }
                        else if (message.Channel == "sms")
                        {
                            await _smsService.SendOtpAsync(
                                message.PhoneNumber,
                                message.Code);
                        }

                        await channel.BasicAckAsync(
                            ea.DeliveryTag,
                            false,
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Failed to send OTP: {ex.Message}");

                        await channel.BasicNackAsync(
                            ea.DeliveryTag,
                            false,
                            true,
                            cancellationToken);
                    }
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);
            await channel.BasicQosAsync(
    prefetchSize: 0,
    prefetchCount: 1,
    global: false,
    cancellationToken);
        }
    }
}