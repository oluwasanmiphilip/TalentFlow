// File Path: TalentFlow.API/Controllers/AuthController.cs

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Infrastructure.Auth;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly JwtTokenService _jwt;
        private readonly IRefreshTokenRepository _refreshRepo;

        public AuthController(IMediator mediator, JwtTokenService jwt, IRefreshTokenRepository refreshRepo)
        {
            _mediator = mediator;
            _jwt = jwt;
            _refreshRepo = refreshRepo;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var userDto = await _mediator.Send(command);
            if (userDto == null)
                return BadRequest(ApiResponse<string>.Fail("Invalid registration data", 400));

            var accessToken = _jwt.GenerateToken(userDto.LearnerId, userDto.Email, userDto.Role);
            var refreshToken = _jwt.GenerateRefreshToken(userDto.LearnerId, userDto.Email, userDto.Role);
            _refreshRepo.Save(refreshToken);

            return Created("auth/register", ApiResponse<object>.Success(new
            {
                learner_id = userDto.LearnerId,
                full_name = userDto.FullName,
                email = userDto.Email,
                role = userDto.Role,
                accessToken,
                refreshToken = refreshToken.Token
            }, "User registered successfully", 201));
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var userDto = await _mediator.Send(command);
            if (userDto == null)
                return Unauthorized(ApiResponse<string>.Fail("Invalid email or password", 401));

            var accessToken = _jwt.GenerateToken(userDto.LearnerId, userDto.Email, userDto.Role);
            var refreshToken = _jwt.GenerateRefreshToken(userDto.LearnerId, userDto.Email, userDto.Role);
            _refreshRepo.Save(refreshToken);

            return Ok(ApiResponse<object>.Success(new
            {
                learner_id = userDto.LearnerId,
                full_name = userDto.FullName,
                email = userDto.Email,
                role = userDto.Role,
                accessToken,
                refreshToken = refreshToken.Token
            }, "Login successful"));
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] string refreshToken)
        {
            var storedToken = _refreshRepo.GetByToken(refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized(ApiResponse<string>.Fail("Invalid or expired refresh token", 401));

            var newAccessToken = _jwt.GenerateAToken(storedToken.UserId, storedToken.Email, storedToken.Role);

            return Ok(ApiResponse<object>.Success(new
            {
                token = newAccessToken,
                expiresIn = 3600,
                tokenType = "Bearer"
            }, "Token refreshed successfully"));
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] string refreshToken)
        {
            var storedToken = _refreshRepo.GetByToken(refreshToken);
            if (storedToken == null)
                return NotFound(ApiResponse<string>.Fail("Refresh token not found", 404));

            _refreshRepo.Revoke(refreshToken);
            return Ok(ApiResponse<string>.Success("User logged out successfully"));
        }

        [Authorize(Policy = "RequireLearner")]
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
    }
}
