//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using TalentFlow.Application.Common.Interfaces;
//using TalentFlow.Application.LearningTask.Commands;
//using TalentFlow.Application.LearningTask.DTOs;
//using TalentFlow.Application.LearningTask.Repositories;
//using TalentFlow.Domain.Entities;

//namespace TalentFlow.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class LearningTaskController : ControllerBase
//    {
//        private readonly IMediator _mediator;
//        private readonly ILearningTaskRepository _repository;

//        public LearningTaskController(IMediator mediator, ILearningTaskRepository repository)
//        {
//            _mediator = mediator;
//            _repository = repository;
//        }

//        // POST: api/learningtask
//        [HttpPost]
//        public async Task<ActionResult<LearningTaskDto>> Create([FromBody] CreateLearningTaskCommand command, CancellationToken ct)
//        {
//            var result = await _mediator.Send(command, ct);
//            return Ok(result);
//        }

//        // PUT: api/learningtask/{id}
//        [HttpPut("{id}")]
//        public async Task<ActionResult<LearningTaskDto>> Update(Guid id, [FromBody] UpdateLearningTaskCommand command, CancellationToken ct)
//        {
//            if (id != command.Id) return BadRequest("Mismatched task ID.");
//            var result = await _mediator.Send(command, ct);
//            return Ok(result);
//        }

//        // DELETE: api/learningtask/{id}
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
//        {
//            var result = await _mediator.Send(new DeleteLearningTaskCommand(id), ct);
//            if (!result) return NotFound();
//            return NoContent();
//        }

//        // PATCH: api/learningtask/{id}/complete
//        [HttpPatch("{id}/complete")]
//        public async Task<ActionResult<LearningTaskDto>> Complete(Guid id, CancellationToken ct)
//        {
//            var result = await _mediator.Send(new CompleteLearningTaskCommand(id), ct);
//            return Ok(result);
//        }

//        // GET: api/learningtask/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<LearningTaskDto>> GetById(Guid id, CancellationToken ct)
//        {
//            var task = await _repository.GetByIdAsync(id, ct);
//            if (task == null) return NotFound();

//            return Ok(new LearningTaskDto
//            {
//                Id = task.Id,
//                AssignedTo = task.AssignedTo,
//                Title = task.Title,
//                Description = task.Description,
//                //DueDate = task.DueDate,
//                Status = task.Status // now LearningTaskStatus
//            });
//        }

//        // GET: api/learningtask/user/{userId}
//        [HttpGet("user/{userId}")]
//        public async Task<ActionResult<IEnumerable<LearningTaskDto>>> GetByUser(Guid userId, CancellationToken ct)
//        {
//            var tasks = await _repository.GetByUserAsync(userId, ct);
//            var result = tasks.Select(t => new LearningTaskDto
//            {
//                Id = t.Id,
//                AssignedTo = t.AssignedTo,
//                Title = t.Title,
//                Description = t.Description,
//                DueDate = t.DueDate,
//                Status = t.Status // now LearningTaskStatus
//            });

//            return Ok(result);
//        }
//    }
//}
