using SendGrid;
using SendGrid.Helpers.Mail;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Infrastructure.Resilience;

namespace TalentFlow.Infrastructure.Notifications
{
    public class SendGridNotificationService 
    {
        private readonly string _apiKey;

        public SendGridNotificationService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task SendAsync(NotificationMessage message)
        {
            var client = new SendGridClient(_apiKey);
            var mail = new SendGridMessage
            {
                From = new EmailAddress("noreply@talentflow.com", "TalentFlow"),
                Subject = "Notification",
                PlainTextContent = message.Message
            };
            mail.AddTo(new EmailAddress(message.LearnerId + "@example.com"));

            await PollyPolicies.RetryPolicy
                .WrapAsync(PollyPolicies.CircuitBreakerPolicy)
                .ExecuteAsync(async () =>
                {
                    await client.SendEmailAsync(mail);
                });
        }
    }
}
