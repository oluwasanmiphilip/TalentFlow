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
            int retryCount = 0;
            const int maxRetries = 3;

            while (true)
            {
                try
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

                    await client.SendMailAsync(mail);
                    break; // ✅ success, exit loop
                }
                catch (Exception ex)
                {
                    retryCount++;

                    if (retryCount >= maxRetries)
                    {
                        throw new Exception($"Failed to send OTP after {maxRetries} attempts. Last error: {ex.Message}");
                    }

                    // 🔄 wait before retry (exponential backoff)
                    await Task.Delay(500 * retryCount);
                }
            }
        }
    }
}
