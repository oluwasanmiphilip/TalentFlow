using System;

namespace TalentFlow.Api.Controllers.Requests
{
    public class ResendOtpRequest
    {
        public Guid UserId { get; set; }
    }
}