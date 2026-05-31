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

        public async Task SendOtpAsync(string recipientEmail, string otpCode)
        {
            using var client = new SmtpClient(_settings.Server, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = "Your OTP Code",
                Body = $"Your OTP code is: {otpCode}",
                IsBodyHtml = false
            };

            mail.To.Add(recipientEmail);

            int maxRetries = 3;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await client.SendMailAsync(mail);
                    return; // success exit
                }
                catch
                {
                    if (i == maxRetries - 1)
                        throw;

                    await Task.Delay(500 * (i + 1));
                }
            }
        }
    }
}