using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class ForgotPasswordCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
    }
}
