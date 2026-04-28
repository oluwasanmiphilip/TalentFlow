using System;
using MediatR;

namespace TalentFlow.Application.Otp.Commands
{
    public class GenerateOtpCommand : IRequest<string>
    {
        public Guid UserId { get; set; }

        // ✅ New property to choose between delivery channel
        // Accepts "email" or "sms"
        public string Channel { get; set; } = "email";
    }
}
