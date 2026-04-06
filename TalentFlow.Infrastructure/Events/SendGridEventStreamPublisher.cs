using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Events
{
    public class SendGridEventStreamPublisher : IEventStreamPublisher
    {
        private readonly SendGridClient _client;

        public SendGridEventStreamPublisher(string apiKey)
        {
            _client = new SendGridClient(apiKey);
        }

        public async Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken = default)
        {
            var email = new SendGridMessage
            {
                From = new EmailAddress("no-reply@talentflow.com", "TalentFlow Notifications"),
                Subject = $"Notification Event: {eventName}",
                PlainTextContent = JsonSerializer.Serialize(payload)
            };
            email.AddTo(new EmailAddress("recipient@example.com"));

            await _client.SendEmailAsync(email, cancellationToken);
        }
    }
}
