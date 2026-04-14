using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Common.Services;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Users.Commands;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly TokenService _tokenService;

    public AuthController(IMediator mediator, TokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var userDto = await _mediator.Send(command);
        if (userDto == null)
            return BadRequest(ApiResponse<string>.Fail("Invalid registration data", 400));

        var otpCode = await _mediator.Send(new GenerateOtpCommand
        {
            UserId = userDto.Id,
            Channel = "email"
        });

        return Created("auth/register", ApiResponse<object>.Success(new
        {
            id = userDto.Id,
            full_name = userDto.FullName,
            email = userDto.Email,
            role = userDto.Role,
            otp = otpCode
        }, "User registered successfully. Please verify OTP.", 201));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var userDto = await _mediator.Send(command);
        if (userDto == null)
            return Unauthorized(ApiResponse<string>.Fail("Invalid email or password", 401));

        await _mediator.Send(new GenerateOtpCommand
        {
            UserId = userDto.Id,
            Channel = "email"
        });

        return Ok(ApiResponse<string>.Success("Login successful. OTP sent to your email."));
    }

    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] ValidateOtpCommand command)
    {
        var userDto = await _mediator.Send(command);
        if (userDto == null)
            return BadRequest(ApiResponse<string>.Fail("Invalid or expired OTP", 400));

        var tokens = _tokenService.IssueTokens(userDto);

        return Ok(ApiResponse<object>.Success(new
        {
            accessToken = tokens.accessToken,
            refreshToken = tokens.refreshToken
        }, "OTP verified successfully. Tokens issued."));
    }
}
