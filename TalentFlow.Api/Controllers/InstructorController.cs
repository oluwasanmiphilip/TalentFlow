using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Instructors.Commands;
using TalentFlow.Application.Instructors.DTOs;
using TalentFlow.Application.Instructors.Queries;

namespace TalentFlow.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Instructor")]

    public class InstructorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InstructorController(IMediator mediator) => _mediator = mediator;

        /// <summary>Create a new instructor</summary>
        [HttpPost]
        public async Task<ActionResult<InstructorDto>> CreateInstructor(CreateInstructorCommand command)
        {
            var instructor = await _mediator.Send(command);
            return Ok(instructor);
        }

        /// <summary>Get instructor by ID</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InstructorDto>> GetInstructorById(Guid id)
        {
            var instructor = await _mediator.Send(new GetInstructorByIdQuery(id));
            return instructor is null ? NotFound() : Ok(instructor);
        }

        /// <summary>Get all instructors</summary>
        [HttpGet]
        public async Task<ActionResult<List<InstructorDto>>> GetAllInstructors()
        {
            var instructors = await _mediator.Send(new GetAllInstructorsQuery());
            return Ok(instructors);
        }
    }
}
