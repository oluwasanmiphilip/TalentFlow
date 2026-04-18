using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.LeanersProgress.Commands;

namespace TalentFlow.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonProgressController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LessonProgressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // PUT: api/lessonprogress/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVideoPosition(Guid id, [FromBody] UpdateVideoPositionCommand command)
        {
            if (id != command.LessonId) return BadRequest("Lesson ID mismatch");

            var userId = User.FindFirst("learner_id")?.Value ?? "system";
            var enrichedCommand = command with { UserId = Guid.Parse(userId) };

            var result = await _mediator.Send(enrichedCommand);
            return result ? Ok(new { saved = true }) : NotFound();
        }

        // POST: api/lessonprogress/{id}/complete
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> MarkLessonComplete(Guid id)
        {
            var userId = User.FindFirst("learner_id")?.Value ?? "system";
            var command = new MarkLessonCompleteCommand(id, Guid.Parse(userId));

            var result = await _mediator.Send(command);
            if (result == null) return NotFound();

            return Ok(result); // result should include completed_at, next_lesson_id, course_pct
        }
        [HttpPost("update-video-position")]
        public async Task<IActionResult> UpdateVideoPosition(UpdateVideoPositionCommand command)
        {
            var result = await _mediator.Send(command);
            return result ? Ok() : BadRequest();
        }
    }
}
