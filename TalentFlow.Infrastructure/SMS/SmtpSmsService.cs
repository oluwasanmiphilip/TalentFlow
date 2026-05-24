using System.Net;
using System.Net.Mail;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Infrastructure.Email;
using Microsoft.Extensions.Logging;

namespace TalentFlow.Infrastructure.Sms
{
    public class SmtpSmsService : ISmsService
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<SmtpSmsService> _logger;

        public SmtpSmsService(SmtpSettings settings, ILogger<SmtpSmsService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendOtpAsync(string phoneNumber, string otpCode)
        {
            await SendInternalAsync(phoneNumber, $"Your OTP code is: {otpCode}");
        }

        public async Task SendAsync(string phoneNumber, string message)
        {
            await SendInternalAsync(phoneNumber, message);
        }

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

                    // ⚠️ Replace with a real SMS gateway domain
                    mail.To.Add($"{phoneNumber}@sms-gateway.example.com");

                    _logger.LogInformation("Attempting to send SMS to {Phone}", phoneNumber);
                    await client.SendMailAsync(mail);
                    _logger.LogInformation("SMS sent successfully to {Phone}", phoneNumber);
                    break; // ✅ success
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogError(ex, "Failed to send SMS (attempt {Attempt})", retryCount);

                    if (retryCount >= maxRetries)
                    {
                        // ✅ Fail gracefully: log and exit without crashing registration
                        _logger.LogCritical("SMS sending failed after {MaxRetries} attempts. Giving up.", maxRetries);
                        return; // don’t throw, just return
                    }

                    // 🔄 exponential backoff
                    await Task.Delay(500 * retryCount);
                }
            }
        }
    }
}
