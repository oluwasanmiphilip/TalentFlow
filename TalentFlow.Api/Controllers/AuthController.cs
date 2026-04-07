using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Infrastructure.Auth;

namespace TalentFlow.API.Controllers
{
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
            var userDto = await _mediator.Send(command);

            if (userDto == null)
                return BadRequest(ApiResponse<string>.Fail("Invalid registration data", 400));

            var token = _jwt.GenerateToken(userDto.LearnerId, userDto.Email, userDto.Role);

            return Created("auth/register", ApiResponse<object>.Success(new
            {
                learner_id = userDto.LearnerId,
                full_name = userDto.FullName,
                email = userDto.Email,
                role = userDto.Role,
                token
            }, "User registered successfully", 201));
        }



        [HttpGet("me/dashboard")]
        public IActionResult GetDashboard()
        {
            var learnerId = User.FindFirst("learner_id")?.Value;
            if (learnerId == null)
                return Unauthorized(ApiResponse<string>.Fail("Unauthorized access", 401));

            return Ok(ApiResponse<object>.Success(new
            {
                learner_id = learnerId,
                dashboard = new { courses = new[] { "Course A", "Course B" } }
            }, "Dashboard retrieved successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var userDto = await _mediator.Send(command);

            if (userDto == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid email or password", 401));

            var token = _jwt.GenerateToken(userDto.LearnerId, userDto.Email, userDto.Role);

            return Ok(ApiResponse<object>.Success(new
            {
                learner_id = userDto.LearnerId,
                full_name = userDto.FullName,
                email = userDto.Email,
                role = userDto.Role,
                token
            }, "Login successful"));
        }



    }
}
