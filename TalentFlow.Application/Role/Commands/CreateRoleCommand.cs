using MediatR;
using TalentFlow.Application.Roles.DTOs;

namespace TalentFlow.Application.Roles.Commands
{
    public record CreateRoleCommand(string Name) : IRequest<RoleDto>;

    public record AssignRoleToUserCommand(Guid RoleId, Guid UserId) : IRequest<bool>;
}
