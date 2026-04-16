using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;

public class SendGridNotificationService : INotificationService
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

        mail.AddTo(new EmailAddress(message.RecipientEmail));

        var response = await client.SendEmailAsync(mail);

        Console.WriteLine($"SendGrid response: {response.StatusCode}");
    }

    // If you don’t need this, you can leave it empty or throw NotImplementedException
    public Task SendNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
