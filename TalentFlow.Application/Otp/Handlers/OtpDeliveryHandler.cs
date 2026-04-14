using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Messages;

namespace TalentFlow.Application.Otp.Handlers
{
    public class OtpDeliveryHandler
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public OtpDeliveryHandler(IEmailService emailService, ISmsService smsService)
        {
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task DeliverOtpAsync(OtpMessage message)
        {
            if (message.Channel == "email")
            {
                await _emailService.SendOtpAsync(message.Email, message.Code);
            }
            else if (message.Channel == "sms")
            {
                await _smsService.SendOtpAsync(message.PhoneNumber, message.Code);
            }
        }
    }
}
