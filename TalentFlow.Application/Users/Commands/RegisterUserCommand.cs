using MediatR;
using TalentFlow.Domain.Entities;

using TalentFlow.Application.Users.DTOs;

namespace TalentFlow.Application.Users.Commands
{
    /// <summary>
    /// Command to register a new user in the system.
    /// </summary>
    public record RegisterUserCommand(
        string LearnerId,
        string Email,
        string FullName
    ) : IRequest<UserDto>;
}
