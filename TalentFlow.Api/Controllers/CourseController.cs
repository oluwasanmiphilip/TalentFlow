using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Courses.Queries;

namespace TalentFlow.API.Controllers
{
    //[ApiExplorerSettings(GroupName = "v2")]
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
        {
            var slug = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCourseBySlug), new { slug }, new { slug });
        }


        [HttpPost("{slug}/enroll")]
        public async Task<IActionResult> EnrollCourse(string slug, [FromBody] string learnerId)
        {
            var result = await _mediator.Send(new EnrollCourseCommand(learnerId, slug));
            if (!result) return NotFound("User or course not found");

            return Ok(new { message = $"Learner {learnerId} enrolled in course {slug}" });
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetCourseBySlug(string slug)
        {
            var courseDto = await _mediator.Send(new GetCourseBySlugQuery(slug));
            if (courseDto is null) return NotFound();

            return Ok(courseDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _mediator.Send(new GetAllCoursesQuery());
            return Ok(courses);
        }
        [HttpGet("learner/{learnerId}")]
        public async Task<IActionResult> GetCoursesByLearner(string learnerId)
        {
            var courses = await _mediator.Send(new GetCoursesByLearnerQuery(learnerId));
            if (!courses.Any()) return NotFound("No courses found for this learner");

            return Ok(courses);
        }




    }
}
