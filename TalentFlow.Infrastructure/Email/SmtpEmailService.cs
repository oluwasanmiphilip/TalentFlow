using System.Net;
using System.Net.Mail;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailService(SmtpSettings settings)
        {
            _settings = settings;
        }

        public async Task SendOtpAsync(string toEmail, string otpCode)
        {
            using var client = new SmtpClient(_settings.Server, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = "Your OTP Code",
                Body = $"Your OTP is {otpCode}. Expires in 5 minutes.",
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
