using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Questions.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IQuestionRepository _repo;

        public QuestionController(IMediator mediator, IQuestionRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        // GET: api/question/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(Guid id)
        {
            var question = await _repo.GetByIdAsync(id, HttpContext.RequestAborted); // ✅ FIX

            if (question == null) return NotFound();

            return Ok(question);
        }

        // GET: api/question/assessment/{assessmentId}
        [HttpGet("assessment/{assessmentId}")]
        public async Task<ActionResult<List<Question>>> GetQuestionsByAssessment(Guid assessmentId)
        {
            var questions = await _repo.GetByAssessmentIdAsync(
                assessmentId,
                HttpContext.RequestAborted // ✅ FIX
            );

            if (questions == null || questions.Count == 0) return NotFound();

            return Ok(questions);
        }

        // PUT: api/question/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuestionCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");

            var updatedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var enrichedCommand = command with { UpdatedBy = updatedBy };

            var result = await _mediator.Send(enrichedCommand);
            return result ? Ok(new { message = "Question updated" }) : NotFound();
        }

        // DELETE: api/question/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var result = await _mediator.Send(new DeleteQuestionCommand(id));
            return result ? Ok(new { message = "Question deleted" }) : NotFound();
        }
    }
}