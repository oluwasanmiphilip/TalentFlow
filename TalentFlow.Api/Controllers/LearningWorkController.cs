using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Interfaces;
using TalentFlow.Application.LearningWorks.Commands;
using TalentFlow.Application.LearningWorks.DTOs;

namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class LearningWorksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILearningWorkRepository _repository;

        public LearningWorksController(IMediator mediator, ILearningWorkRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        // POST: api/learningworks
        [HttpPost]
        public async Task<ActionResult<LearningWorkDto>> Create([FromBody] CreateLearningWorkCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        // PUT: api/learningworks/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<LearningWorkDto>> Update(Guid id, [FromBody] Application.LearningWorks.Commands.UpdateLearningWorkCommand command, CancellationToken ct)
        {
            if (id != command.Id) return BadRequest("Mismatched work ID.");
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        // DELETE: api/learningworks/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteLearningWorkCommand(id), ct);
            if (!result) return NotFound();
            return NoContent();
        }

        // PATCH: api/learningworks/{id}/complete
        [HttpPatch("{id}/complete")]
        public async Task<ActionResult<LearningWorkDto>> Complete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new CompleteLearningWorkCommand(id), ct);
            return Ok(result);
        }

        // GET: api/learningworks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LearningWorkDto>> GetById(Guid id, CancellationToken ct)
        {
            var work = await _repository.GetByIdAsync(id, ct);
            if (work == null) return NotFound();

            return Ok(new LearningWorkDto
            {
                Id = work.Id,
                AssignedTo = work.AssignedTo,
                Title = work.Title,
                Details = work.Details,
                DueDate = work.DueDate,
                State = work.State
            });
        }

        // GET: api/learningworks/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<LearningWorkDto>>> GetByUser(Guid userId, CancellationToken ct)
        {
            var works = await _repository.GetByUserAsync(userId, ct);
            var result = works.Select(w => new LearningWorkDto
            {
                Id = w.Id,
                AssignedTo = w.AssignedTo,
                Title = w.Title,
                Details = w.Details,
                DueDate = w.DueDate,
                State = w.State
            });

            return Ok(result);
        }
    }
}
