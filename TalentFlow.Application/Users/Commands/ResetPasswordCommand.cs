using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
