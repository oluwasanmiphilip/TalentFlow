using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class UpdatePasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
