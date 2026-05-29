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

        public Task SendOtpAsync(OtpMessage message)
        {
            return _emailService.SendOtpAsync(
                message.Email,
                message.Code
            );
        }
    }
}