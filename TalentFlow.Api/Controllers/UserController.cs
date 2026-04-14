using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Certificates.Commands;
using TalentFlow.Application.Certificates.Queries;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Queries;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Users.DTOs;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Learner")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator) => _mediator = mediator;

    /// <summary>Create a new user</summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(RegisterUserCommand command)
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

    [HttpGet("{learnerId}/courses")]
    public async Task<ActionResult<List<CourseDto>>> GetCoursesByLearner(string learnerId)
    {
        var courses = await _mediator.Send(new GetCoursesByLearnerQuery(learnerId));
        return Ok(courses);
    }


    [HttpGet("{id}/certificates")]
    public async Task<IActionResult> GetCertificates(Guid id)
    {
        var result = await _mediator.Send(new GetCertificatesByUserIdQuery(id));
        return Ok(result);
    }
    [HttpPost("{id}/certificates")]
    public async Task<IActionResult> CreateCertificate(Guid id, [FromBody] CreateCertificateCommand command)
    {
        // Ensure LearnerId is set from route
        command = command with { LearnerId = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

}
