using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Dashboard.Admin.DTOs;
using TalentFlow.Application.Dashboard.Admin.Queries;

namespace TalentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Policy = "RequireAdmin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{adminId}")]
        public async Task<ActionResult<AdminDashboardDto>> Get(string adminId)
        {
            var result = await _mediator.Send(new GetAdminDashboardQuery(adminId));
            return Ok(result);
        }
    }
}
