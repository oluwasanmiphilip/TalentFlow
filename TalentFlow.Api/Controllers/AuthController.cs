// File Path: TalentFlow.Api/Controllers/AuthController.cs

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Users.Commands;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenService _tokenService;

    public AuthController(IMediator mediator, IJwtTokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    // ============================
    // REGISTER
    // ============================
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var userDto = await _mediator.Send(command);

        if (userDto == null)
            return BadRequest(ApiResponse<string>.Fail("Invalid registration data", 400));

        await _mediator.Send(new GenerateOtpCommand
        {
            UserId = userDto.Id
        });

        return Created("auth/register", ApiResponse<object>.Success(new
        {
            id = userDto.Id,
            full_name = userDto.FullName,
            email = userDto.Email,
            role = userDto.Role
        }, "User registered successfully. OTP sent to your email.", 201));
    }

    // ============================
    // LOGIN
    // ============================
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var userDto = await _mediator.Send(command);

        if (userDto == null)
            return Unauthorized(ApiResponse<string>.Fail("Invalid email or password", 401));

        var accessToken = _tokenService.GenerateToken(userDto.Id, userDto.Email, userDto.Role);
        var refreshToken = _tokenService.GenerateRefreshToken(userDto.Id, userDto.Email, userDto.Role);

        await _mediator.Send(new SaveLoginTokenCommand
        {
            UserId = userDto.Id,
            Token = accessToken
        });

        return Ok(ApiResponse<object>.Success(new
        {
            accessToken,
            refreshToken
        }, "Login successful."));


        return Ok(ApiResponse<object>.Success(new
        {
            userId = userDto.Id,
            message = "OTP sent to your email. Please verify to complete login."
        }));
    }

    // ============================
    // VERIFY OTP
    // ============================
    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] ValidateOtpCommand command)
    {
        var userDto = await _mediator.Send(command);

        if (userDto == null)
            return BadRequest(ApiResponse<string>.Fail("Invalid or expired OTP", 400));

        var accessToken = _tokenService.GenerateToken(
            userDto.Id,
            userDto.Email,
            userDto.Role
        );

        var refreshToken = _tokenService.GenerateRefreshToken(
            userDto.Id,
            userDto.Email,
            userDto.Role
        );

        // ✅ FIXED: SAVE TOKEN USING MEDIATOR (CLEAN CQRS)
        await _mediator.Send(new SaveLoginTokenCommand
        {
            UserId = userDto.Id,
            Token = accessToken
        });

        return Ok(ApiResponse<object>.Success(new
        {
            accessToken,
            refreshToken
        }, "OTP verified successfully. Tokens issued."));
    }

    // ============================
    // RESEND OTP
    // ============================
    [AllowAnonymous]
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] Guid userId)
    {
        var userDto = await _mediator.Send(new GetUserByIdCommand { UserId = userId });

        if (userDto == null)
            return NotFound(ApiResponse<string>.Fail("User not found", 404));

        await _mediator.Send(new GenerateOtpCommand
        {
            UserId = userDto.Id
        });

        return Ok(ApiResponse<string>.Success("New OTP sent to your email."));
    }
}