using Microsoft.AspNetCore.Mvc;
using MediatR;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Infrastructure.Auth;

namespace TalentFlow.API.Controllers
{
    //[ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly JwtTokenService _jwt;

        public AuthController(IMediator mediator, JwtTokenService jwt)
        {
            _mediator = mediator;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            // Mediator returns a UserDto
            var userDto = await _mediator.Send(command);

            // Generate token using learnerId + email
            var token = _jwt.GenerateToken(userDto.LearnerId, userDto.Email);

            return Ok(new
            {
                learner_id = userDto.LearnerId,
                full_name = userDto.FullName,
                email = userDto.Email,
                token
            });
        }

        [HttpGet("me/dashboard")]
        public IActionResult GetDashboard()
        {
            var learnerId = User.FindFirst("learner_id")?.Value;
            if (learnerId == null) return Unauthorized();

            // Example response
            return Ok(new
            {
                learner_id = learnerId,
                dashboard = new { courses = new[] { "Course A", "Course B" } }
            });
        }
    }
}
