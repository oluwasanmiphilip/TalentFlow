using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Enrollments.DTOs;
using EnrollmentDto = TalentFlow.Application.Courses.DTOs.EnrollmentDto;

//[ApiExplorerSettings(GroupName = "v4")]
[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IMediator _mediator;
    public EnrollmentController(IMediator mediator) => _mediator = mediator;

    /// <summary>Enroll a learner into a course</summary>
    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Enroll(EnrollCourseCommand command)
    {
        var result = await _mediator.Send(command);
        return result ? Ok() : BadRequest("Enrollment failed");
    }

    /// <summary>Get enrollment by ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollment(Guid id)
    {
        var enrollment = await _mediator.Send(new GetEnrollmentByIdQuery(id));
        return enrollment is null ? NotFound() : Ok(enrollment);
    }
}
