using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Assessments.Commands;
using TalentFlow.Application.Assessments.Queries;
using TalentFlow.Application.Assessments.DTOs;

namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssessmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssessmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/assignment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentDto>> GetAssessment(Guid id)
        {
            var assessment = await _mediator.Send(new GetAssessmentQuery(id));
            if (assessment == null) return NotFound();

            return Ok(assessment);
        }

        // GET: api/assessment/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<AssessmentDto>>> GetAssessmentsByCourse(Guid courseId)
        {
            var assessments = await _mediator.Send(new GetAssessmentsByCourseQuery(courseId));
            if (assessments == null || assessments.Count == 0) return NotFound();

            return Ok(assessments);
        }

        // PUT: api/assessment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssessment(Guid id, [FromBody] UpdateAssessmentCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");

            var updatedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var enrichedCommand = command with { UpdatedBy = updatedBy };

            var result = await _mediator.Send(enrichedCommand);
            return result ? Ok(new { message = "Assessment updated" }) : NotFound();
        }

        // DELETE: api/assessment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(Guid id)
        {
            var deletedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var command = new DeleteAssessmentCommand(id, deletedBy);

            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Assessment deleted" }) : NotFound();
        }
    }
}
