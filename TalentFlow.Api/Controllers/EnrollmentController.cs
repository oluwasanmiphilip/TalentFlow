using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.CourseProgress.Commands;
using TalentFlow.Application.Courses.DTOs; // ✅ Correct import
using TalentFlow.Application.Enrollments.Commands;
using TalentFlow.Application.Enrollments.DTOs;
using TalentFlow.Application.Enrollments.Queries;

namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnrollmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/enrollment/{id}
        [HttpGet("{id}")]
        [HttpGet("{learnerId}/{courseId}")]
        public async Task<IActionResult> GetEnrollment(Guid learnerId, Guid courseId)
        {
            var enrollment = await _mediator.Send(new GetEnrollmentQuery(learnerId, courseId));
            if (enrollment == null)
                return NotFound(ApiResponse.Fail<string>("Enrollment not found", 404));

            return Ok(ApiResponse.Success<object>(enrollment, "Enrollment retrieved successfully"));
        }

        // GET: api/enrollment/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetEnrollmentsByCourse(Guid courseId)
        {
            var enrollments = await _mediator.Send(new GetEnrollmentsByCourseQuery(courseId));
            if (enrollments == null || enrollments.Count == 0) return NotFound();

            return Ok(enrollments);
        }

        // PUT: api/enrollment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(Guid id, [FromBody] UpdateEnrollmentCommand command)
        {
            if (id != command.Id) return BadRequest(ApiResponse.Fail<string>("ID mismatch", 400));

            var updatedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var enrichedCommand = command with { UpdatedBy = updatedBy };

            var result = await _mediator.Send(enrichedCommand);
            return result
                ? Ok(ApiResponse.Success<string>("Enrollment updated"))
                : NotFound(ApiResponse.Fail<string>("Enrollment not found", 404));
        }

        // DELETE: api/enrollment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(Guid id)
        {
            var deletedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var command = new DeleteEnrollmentCommand(id, deletedBy);

            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Enrollment deleted" }) : NotFound();
        }

        [HttpGet("course-enrollment/{courseId}")]
        public async Task<ActionResult<CourseEnrollmentDto>> GetCourseEnrollment(Guid courseId)
        {
            var enrollment = await _mediator.Send(new GetCourseEnrollmentQuery(courseId));
            if (enrollment == null) return NotFound();

            return Ok(enrollment);
        }

        // PUT: api/enrollment/course/{courseId}/complete
        [HttpPut("course/{courseId}/complete")]
        public async Task<IActionResult> CompleteCourse(Guid courseId)
        {
            var learnerIdClaim = User.FindFirst("learner_id")?.Value;
            if (string.IsNullOrEmpty(learnerIdClaim))
                return Unauthorized(ApiResponse.Fail<string>("Learner ID not found in token", 401));

            var learnerId = Guid.Parse(learnerIdClaim);

            var command = new CompleteCourseCommand(learnerId, courseId);
            var result = await _mediator.Send(command);

            if (result == null)
                return BadRequest(ApiResponse.Fail<string>("Course cannot be marked complete until all lessons are finished", 400));

            return Ok(ApiResponse.Success<object>(result, "Course marked as completed"));
        }
    }
}
