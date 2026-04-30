using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Dashboard.Instructor.DTOs;
using TalentFlow.Application.Dashboard.Instructor.Queries;

namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/dashboard/instructor")]
    
    public class InstructorDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstructorDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{instructorId}")]
        public async Task<ActionResult<InstructorDashboardDto>> Get(string instructorId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetInstructorDashboardQuery(instructorId), ct);
            return Ok(result);
        }
    }
}
