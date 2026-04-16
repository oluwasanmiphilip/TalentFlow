using System;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Email
{
    public class DummyEmailService : IEmailService
    {
        public Task SendAsync(string toEmail, string subject, string message)
        {
            Console.WriteLine($"[Dummy Email] To: {toEmail}, Subject: {subject}, Message: {message}");
            return Task.CompletedTask;
        }

        public Task SendOtpAsync(string toEmail, string otpCode)
        {
            Console.WriteLine($"[Dummy Email] OTP to: {toEmail}, Code: {otpCode}");
            return Task.CompletedTask;
        }
    }
}
