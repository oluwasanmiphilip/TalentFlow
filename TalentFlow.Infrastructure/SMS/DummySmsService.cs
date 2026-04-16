using System;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Sms
{
    public class DummySmsService : ISmsService
    {
        public Task SendAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"[Dummy SMS] To: {phoneNumber}, Message: {message}");
            return Task.CompletedTask;
        }

        public Task SendOtpAsync(string toPhoneNumber, string otpCode)
        {
            Console.WriteLine($"[Dummy SMS] OTP to: {toPhoneNumber}, Code: {otpCode}");
            return Task.CompletedTask;
        }
    }
}
