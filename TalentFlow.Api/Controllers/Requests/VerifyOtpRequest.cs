using System;

namespace TalentFlow.Api.Controllers.Requests
{
    public class VerifyOtpRequest
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}