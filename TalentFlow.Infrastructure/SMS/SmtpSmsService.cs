using System.Net;
using System.Net.Mail;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Infrastructure.Email;

namespace TalentFlow.Infrastructure.Sms
{
    public class SmtpSmsService : ISmsService
    {
        private readonly SmtpSettings _settings;

        public SmtpSmsService(SmtpSettings settings)
        {
            _settings = settings;
        }

        // ✅ Implement SendOtpAsync
        public async Task SendOtpAsync(string phoneNumber, string otpCode)
        {
            await SendInternalAsync(phoneNumber, $"Your OTP code is: {otpCode}");
        }

        // ✅ Implement SendAsync (generic SMS)
        public async Task SendAsync(string phoneNumber, string message)
        {
            await SendInternalAsync(phoneNumber, message);
        }

        // 🔄 Shared retry logic
        private async Task SendInternalAsync(string phoneNumber, string message)
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
                        Subject = "SMS Notification",
                        Body = $"To: {phoneNumber}\nMessage: {message}",
                        IsBodyHtml = false
                    };

                    // ⚠️ Replace with a real SMS gateway domain (e.g., Termii/Twilio integration)
                    mail.To.Add($"{phoneNumber}@sms-gateway.example.com");

                    await client.SendMailAsync(mail);
                    break; // ✅ success
                }
                catch (Exception ex)
                {
                    retryCount++;

                    if (retryCount >= maxRetries)
                    {
                        throw new Exception($"Failed to send SMS after {maxRetries} attempts. Last error: {ex.Message}");
                    }

                    // 🔄 exponential backoff
                    await Task.Delay(500 * retryCount);
                }
            }
        }
    }
}
