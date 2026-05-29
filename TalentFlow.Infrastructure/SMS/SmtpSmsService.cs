using Microsoft.Extensions.Logging;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Infrastructure.Email;

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
        await SendAsync(phoneNumber, $"Your OTP code is: {otpCode}");
    }

    public async Task SendAsync(string phoneNumber, string message)
    {
        // your existing logic here
        _logger.LogInformation("Sending SMS to {Phone}", phoneNumber);
        await Task.CompletedTask;
    }
}