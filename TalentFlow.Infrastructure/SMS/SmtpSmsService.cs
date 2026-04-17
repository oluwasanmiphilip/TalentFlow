using TalentFlow.Application.Common.Interfaces;

public class SmtpSmsService : ISmsService
{
    private readonly IEmailService _emailService;

    public SmtpSmsService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendAsync(string phoneNumber, string message)
    {
        var smsAddress = $"{phoneNumber}@vtext.com"; // Example: Verizon
        await _emailService.SendOtpAsync(smsAddress, message);
    }

    public async Task SendOtpAsync(string toPhoneNumber, string otpCode)
    {
        var smsAddress = $"{toPhoneNumber}@vtext.com"; // Carrier domain configurable
        await _emailService.SendOtpAsync(smsAddress, otpCode);
    }
}
