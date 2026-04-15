using Azure;
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
                Subject = "Your OTP Code",
                PlainTextContent = message.Message
            };

            // Use the new property
            mail.AddTo(new EmailAddress(message.RecipientEmail));

            var response = await client.SendEmailAsync(mail);
            Console.WriteLine($"SendGrid response: {response.StatusCode}");
        }



    }
}
