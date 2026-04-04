using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Queries;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Users.DTOs;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator) => _mediator = mediator;

    /// <summary>Create a new user</summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserCommand command)
    {
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    /// <summary>Get user by ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>Get courses enrolled by a learner</summary>
    [HttpGet("{learnerId}/courses")]
    public async Task<ActionResult<List<CourseDto>>> GetCoursesByLearner(string learnerId)
    {
        var courses = await _mediator.Send(new GetCoursesByLearnerQuery(learnerId));
        return Ok(courses);
    }
}
