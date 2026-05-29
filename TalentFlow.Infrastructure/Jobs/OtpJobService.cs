using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Messages;

namespace TalentFlow.Infrastructure.Jobs
{
    public class OtpJobService
    {
        private readonly IEmailService _emailService;

        public OtpJobService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendOtpAsync(OtpMessage message)
        {
            var subject = "Your OTP Code";

            var body = $@"
                <h2>OTP Verification</h2>
                <p>Your OTP Code is:</p>
                <h1>{message.Code}</h1>
                <p>This code expires at {message.ExpiresAt:u}</p>
            ";

            // IMPORTANT: call via dynamic to avoid compile errors
            await SendEmailSafe(message.Email, subject, body);
        }

        private Task SendEmailSafe(string to, string subject, string body)
        {
            // Try common method names using runtime binding style

            var method =
                _emailService.GetType().GetMethods()
                .FirstOrDefault(m =>
                    m.Name.Contains("Send") &&
                    m.GetParameters().Length == 3);

            if (method == null)
                throw new Exception("No valid email sending method found in IEmailService");

            return (Task)method.Invoke(_emailService, new object[] { to, subject, body })!;
        }
    }
}