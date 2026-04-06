using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Roles.Commands;
using TalentFlow.Application.Roles.Queries;
using TalentFlow.Application.Roles.DTOs;

namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // restrict all endpoints to Admins
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RoleController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleCommand command)
        {
            var role = await _mediator.Send(command);
            return Ok(role);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
        {
            var role = await _mediator.Send(new GetRoleByIdQuery(id));
            return role is null ? NotFound() : Ok(role);
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
        {
            var roles = await _mediator.Send(new GetAllRolesQuery());
            return Ok(roles);
        }

        [HttpPost("{roleId}/assign/{userId}")]
        public async Task<ActionResult> AssignRole(Guid roleId, Guid userId)
        {
            var result = await _mediator.Send(new AssignRoleToUserCommand(roleId, userId));
            return result ? Ok(new { message = "Role assigned successfully" })
                          : BadRequest("Failed to assign role");
        }
    }
}
