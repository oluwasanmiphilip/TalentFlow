using System;

namespace TalentFlow.Application.Common.Messages
{
    public class OtpMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Channel { get; set; } = "email";
        public DateTime ExpiresAt { get; set; }
    }
}
