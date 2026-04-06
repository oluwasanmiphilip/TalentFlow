using MediatR;
using TalentFlow.Application.Roles.DTOs;

namespace TalentFlow.Application.Roles.Queries
{
    public record GetRoleByIdQuery(Guid Id) : IRequest<RoleDto>;

    public record GetAllRolesQuery() : IRequest<List<RoleDto>>;
}
