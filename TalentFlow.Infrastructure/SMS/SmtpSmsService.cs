using Microsoft.Extensions.Logging;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Sms
{
    public class SmtpSmsService : ISmsService
    {
        private readonly ILogger<SmtpSmsService> _logger;

        public SmtpSmsService(ILogger<SmtpSmsService> logger)
        {
            _logger = logger;
        }

        public Task SendOtpAsync(string phoneNumber, string otpCode)
        {
            return SendAsync(phoneNumber, $"Your OTP code is: {otpCode}");
        }

        public async Task SendAsync(string phoneNumber, string message)
        {
            const int maxRetries = 3;
            int retry = 0;

            while (retry < maxRetries)
            {
                try
                {
                    // ✅ TEMP IMPLEMENTATION (safe for Render)
                    // Replace later with Termii / Twilio / Africa's Talking

                    _logger.LogInformation(
                        "SMS to {Phone}: {Message}",
                        phoneNumber,
                        message
                    );

                    await Task.CompletedTask;
                    return;
                }
                catch (Exception ex)
                {
                    retry++;
                    _logger.LogError(ex,
                        "SMS failed attempt {Attempt}",
                        retry);

                    await Task.Delay(500 * retry);
                }
            }

            _logger.LogCritical(
                "SMS failed permanently for {Phone}",
                phoneNumber);
        }
    }
}