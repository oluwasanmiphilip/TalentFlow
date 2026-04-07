using MediatR;
using TalentFlow.Application.Users.DTOs;

namespace TalentFlow.Application.Users.Commands
{
    public class LoginUserCommand : IRequest<UserDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
