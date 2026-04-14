using System;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IOtpMessage
    {
        Guid UserId { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        string Code { get; set; }
        string Channel { get; set; } // "email" or "sms"
        DateTime ExpiresAt { get; set; }
    }
}
