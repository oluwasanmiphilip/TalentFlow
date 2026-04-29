// File: src/TalentFlow.Api/Controllers/CourseController.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Mappings;
using TalentFlow.Application.Enrollments.Commands;
using TalentFlow.Application.Lessons.Commands;
using TalentFlow.Application.Lessons.Queries;

namespace TalentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (command == null)
                return BadRequest(ApiResponse.Fail<string>("Request body is required", 400));

            if (string.IsNullOrWhiteSpace(command.Title) ||
                string.IsNullOrWhiteSpace(command.Description) ||
                string.IsNullOrWhiteSpace(command.Slug) ||
                string.IsNullOrWhiteSpace(command.ThumbnailUrl) ||
                command.InstructorId == Guid.Empty)
            {
                return BadRequest(ApiResponse.Fail<string>("Title, description, slug, thumbnail, and instructor are required", 400));
            }

            var courseId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetCourseBySlug),
                new { slug = command.Slug },
                ApiResponse.Success<object>(new { id = courseId }, "Course created successfully.", 201)
            );
        }


        /// PUT: api/course/{courseId}/enrollments/{enrollmentId}
        [HttpPut("{courseId:guid}/enrollments/{enrollmentId:guid}")]
        public async Task<IActionResult> UpdateEnrollment(Guid courseId, Guid enrollmentId, [FromBody] UpdateEnrollmentCommand command)
        {
            if (command == null || enrollmentId != command.Id)
                return BadRequest(ApiResponse.Fail<string>("Invalid request", 400));

            var result = await _mediator.Send(command);
            return result
                ? Ok(ApiResponse.Success<string>("Enrollment updated successfully."))
                : NotFound(ApiResponse.Fail<string>("Enrollment not found", 404));
        }


        // DELETE: api/course/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _repo.GetByIdAsync(id);
            if (course == null) return NotFound(ApiResponse.Fail<string>("Course not found", 404));

            // Use authenticated user if available
            var deletedBy = User?.Identity?.Name ?? "system";

            await _repo.SoftDeleteAsync(course, deletedBy);

            return Ok(ApiResponse.Success<string>("Course deleted successfully."));
        }


        // GET: api/course/{slug}
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetCourseBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return BadRequest(ApiResponse.Fail<string>("Slug is required", 400));

            var course = await _repo.GetBySlugAsync(slug);
            if (course == null) return NotFound(ApiResponse.Fail<string>("Course not found", 404));

            var dto = course.ToDto(); // mapping now includes ThumbnailUrl, InstructorId, DurationMinutes, Level, Price, Tags, Rating
            return Ok(ApiResponse.Success<CourseDto>(dto, "Course retrieved successfully."));
        }


        // GET: api/course
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _repo.GetAllAsync();
            var dtos = courses?.Select(c => c.ToDto()).ToList() ?? new List<CourseDto>();
            return Ok(ApiResponse.Success<List<CourseDto>>(dtos, "Courses retrieved successfully."));
        }




        // GET: api/course/learner/{userId}
        [HttpGet("learner/{userId:guid}")]
        public async Task<IActionResult> GetCoursesByLearner(Guid userId)
        {
            var courses = await _repo.GetByLearnerIdAsync(userId);
            var dtos = courses?.Select(c => c.ToDto()).ToList() ?? new List<CourseDto>();
            return Ok(ApiResponse.Success<List<CourseDto>>(dtos, "Learner courses retrieved successfully."));
        }

        // POST: api/course/{courseId}/enroll/{userId}
        [HttpPost("{courseId:guid}/enroll/{userId:guid}")]
        public async Task<IActionResult> EnrollLearner(Guid courseId, Guid userId)
        {
            var enrolledBy = User.FindFirst("userId")?.Value ?? "system";
            var result = await _mediator.Send(new EnrollLearnerCommand(courseId, userId, enrolledBy));
            return result
                ? Ok(ApiResponse.Success<string>("Learner enrolled successfully."))
                : NotFound(ApiResponse.Fail<string>("Course or learner not found", 404));
        }

        // DELETE: api/course/{courseId}/unenroll/{userId}
        [HttpDelete("{courseId:guid}/unenroll/{userId:guid}")]
        public async Task<IActionResult> UnenrollLearner(Guid courseId, Guid userId)
        {
            var deletedBy = User.FindFirst("userId")?.Value ?? "system";
            var result = await _mediator.Send(new UnenrollLearnerCommand(courseId, userId, deletedBy));
            return result
                ? Ok(ApiResponse.Success<string>("Learner unenrolled successfully."))
                : NotFound(ApiResponse.Fail<string>("Enrollment not found", 404));
        }

        // GET: api/course/{id}/lessons?pageNumber=1&pageSize=10
        [HttpGet("{id:guid}/lessons")]
        public async Task<IActionResult> GetLessons(Guid id, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest(ApiResponse.Fail<string>("Invalid paging parameters", 400));

            var result = await _mediator.Send(new GetLessonsPagedQuery(id, pageNumber, pageSize));
            return Ok(ApiResponse.Success<object>(result, "Lessons retrieved successfully."));
        }

        // POST: api/course/{id}/lessons
        [HttpPost("{id:guid}/lessons")]
        public async Task<IActionResult> CreateLesson(Guid id, [FromBody] CreateLessonCommand command)
        {
            if (command == null)
                return BadRequest(ApiResponse.Fail<string>("Request body is required", 400));

            // Ensure the command references the correct course
            command.CourseId = id;

            var lessonId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetLessons), new { id = id }, ApiResponse.Success<object>(new { id = lessonId }, "Lesson created successfully.", 201));
        }
        // PUT: api/course/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
        {
            if (command == null)
                return BadRequest(ApiResponse.Fail<string>("Request body is required", 400));

            if (id != command.Id)
                return BadRequest(ApiResponse.Fail<string>("ID mismatch", 400));

            var result = await _mediator.Send(command);
            return result
                ? Ok(ApiResponse.Success<string>("Course updated successfully."))
                : NotFound(ApiResponse.Fail<string>("Course not found", 404));
        }

    }
}
