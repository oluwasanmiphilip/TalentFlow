using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.CourseProgress.Queries;
using TalentFlow.Application.CourseProgress.DTOs;



namespace TalentFlow.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CourseProgressController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourseProgressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/courseprogress/{userId}/{courseId}
        [HttpGet("{userId}/{courseId}")]
        public async Task<ActionResult<CourseProgressDto>> GetCourseProgress(Guid userId, Guid courseId)
        {
            var progress = await _mediator.Send(new GetCourseProgressQuery(userId, courseId));
            if (progress == null) return NotFound();

            return Ok(progress);
        }
    }
}
