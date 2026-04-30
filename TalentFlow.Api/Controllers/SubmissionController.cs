using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Submissions.Commands;
using TalentFlow.Application.Submissions.Queries;
using TalentFlow.Application.Submissions.DTOs;

namespace TalentFlow.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubmissionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubmissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/submission
        [HttpPost]
        public async Task<IActionResult> SubmitAssignment([FromBody] CreateSubmissionCommand command)
        {
            if (command.AssignmentId == Guid.Empty)
                return BadRequest("AssignmentId is required");

            var result = await _mediator.Send(command);
            if (result == null) return BadRequest("Submission failed");

            return Ok(result); // result should include reference_number, submitted_at, status
        }

        // GET: api/submission/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SubmissionDto>> GetSubmission(Guid id)
        {
            var submission = await _mediator.Send(new GetSubmissionQuery(id));
            if (submission == null) return NotFound();

            return Ok(submission);
        }

        // GET: api/submission/validate-url?url=...
        [HttpGet("validate-url")]
        public async Task<IActionResult> ValidateUrl([FromQuery] string url)
        {
            var result = await _mediator.Send(new ValidateUrlQuery(url));
            return Ok(result);
        }
        // PUT: api/submission/{id}/grade
        [HttpPut("{id}/grade")]
        [Authorize(Policy = "RequireInstructor")]
        public async Task<IActionResult> GradeSubmission(Guid id, [FromBody] GradeSubmissionCommand command)
        {
            if (id != command.SubmissionId) return BadRequest("ID mismatch");

            var gradedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var enrichedCommand = command with { GradedBy = gradedBy };

            var result = await _mediator.Send(enrichedCommand);
            return result ? Ok(new { message = "Submission graded successfully" }) : NotFound();
        }

    }
}
