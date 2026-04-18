using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Mappings;
using TalentFlow.Application.Enrollments.Commands;
using TalentFlow.Application.Lessons.Commands;
using TalentFlow.Application.Lessons.Queries;



namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdmin")]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICourseRepository _repo;

        public CourseController(IMediator mediator, ICourseRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        // POST: api/course
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Title) ||
                string.IsNullOrWhiteSpace(command.Description) ||
                string.IsNullOrWhiteSpace(command.Slug))
            {
                return BadRequest(ApiResponse<string>.Fail("Title, description, and slug are required", 400));
            }

            var courseId = await _mediator.Send(command);
            return Created($"api/course/{courseId}", ApiResponse<object>.Success(new { id = courseId }, "Course created successfully", 201));
        }

        // PUT: api/course/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Course updated" }) : NotFound();
        }

        // DELETE: api/course/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var deletedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var result = await _mediator.Send(new DeleteCourseCommand(id, deletedBy));
            return result ? Ok(new { message = "Course deleted" }) : NotFound();
        }

        // GET: api/course/{slug}
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetCourseBySlug(string slug)
        {
            var course = await _repo.GetBySlugAsync(slug);
            if (course == null) return NotFound(ApiResponse<string>.Fail("Course not found", 404));

            return Ok(ApiResponse<object>.Success(course.ToDto(), "Course retrieved successfully"));
        }

        // GET: api/course
        [HttpGet]
        // GET: api/course
        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetAllCourses()
        {
            var courses = await _repo.GetAllAsync();
            if (courses == null || courses.Count == 0) return NotFound();

            return Ok(courses.Select(c => c.ToDto()).ToList());
        }

        // GET: api/course/learner/{userId}
        [HttpGet("learner/{userId}")]
        public async Task<ActionResult<List<CourseDto>>> GetCoursesByLearner(Guid userId)
        {
            var courses = await _repo.GetByLearnerIdAsync(userId);
            if (courses == null || courses.Count == 0) return NotFound();

            return Ok(courses.Select(c => c.ToDto()).ToList());
        }
        // POST: api/course/{courseId}/enroll/{userId}
        [HttpPost("{courseId}/enroll/{userId}")]
        public async Task<IActionResult> EnrollLearner(Guid courseId, Guid userId)
        {
            var enrolledBy = User.FindFirst("learner_id")?.Value ?? "system";
            var result = await _mediator.Send(new EnrollLearnerCommand(courseId, userId, enrolledBy));
            return result ? Ok(new { message = "Learner enrolled successfully" }) : NotFound();
        }
        // DELETE: api/course/{courseId}/unenroll/{userId}
        [HttpDelete("{courseId}/unenroll/{userId}")]
        public async Task<IActionResult> UnenrollLearner(Guid courseId, Guid userId)
        {
            var deletedBy = User.FindFirst("learner_id")?.Value ?? "system";
            var result = await _mediator.Send(new UnenrollLearnerCommand(courseId, userId, deletedBy));
            return result ? Ok(new { message = "Learner unenrolled successfully" }) : NotFound();
        }
        [HttpGet("{id}/lessons")]
        public async Task<IActionResult> GetLessons(Guid id, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetLessonsPagedQuery(id, pageNumber, pageSize));
            return Ok(result);
        }

        [HttpPost("{id}/lessons")]
        public async Task<IActionResult> CreateLesson(Guid id, [FromBody] CreateLessonCommand command)
        {
            // ✅ FIXED: assign directly (class style)
            command.CourseId = id;

            var result = await _mediator.Send(command);
            return Ok(result);
        }



    }
}
