using System;
using MediatR;
using TalentFlow.Application.Users.DTOs;

namespace TalentFlow.Application.Otp.Commands
{
    public class ValidateOtpCommand : IRequest<UserDto?>
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Channel { get; set; } = "email"; // email or sms
    }
}
