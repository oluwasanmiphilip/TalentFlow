using System;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Messaging.MessageContracts
{
    public class OtpMessage : IOtpMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Channel { get; set; } = "email"; // or "sms"
        public DateTime ExpiresAt { get; set; }
    }
}
