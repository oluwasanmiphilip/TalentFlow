using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Dashboard.Instructor.DTOs;
using TalentFlow.Application.Dashboard.Instructor.Queries;

namespace TalentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/instructor/dashboard")]
    [Authorize(Policy = "RequireInstructor")]
    public class InstructorDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstructorDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{instructorId}")]
        public async Task<ActionResult<InstructorDashboardDto>> Get(string instructorId)
        {
            var result = await _mediator.Send(new GetInstructorDashboardQuery(instructorId));
            return Ok(result);
        }
    }
}
