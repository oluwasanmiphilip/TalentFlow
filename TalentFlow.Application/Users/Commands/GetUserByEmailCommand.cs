// File Path: TalentFlow.Application/Users/Commands/GetUserByEmailCommand.cs

using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class GetUserByEmailCommand : IRequest<UserDto?>
    {
        public string Email { get; set; } = string.Empty;
    }
}
