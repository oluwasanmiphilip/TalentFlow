using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using TalentFlow.Application.Common.Interfaces; // ✅ reference the interface

namespace TalentFlow.Infrastructure.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _apiKey;

        public SendGridEmailService(string apiKey) => _apiKey = apiKey;

        public async Task SendOtpAsync(string toEmail, string otpCode)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("no-reply@talentflow.com", "TalentFlow");
            var subject = "Your OTP Code";
            var to = new EmailAddress(toEmail);
            var plainTextContent = $"Your OTP code is {otpCode}. It expires in 5 minutes.";
            var htmlContent = $"<strong>Your OTP code is {otpCode}</strong>. It expires in 5 minutes.";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
