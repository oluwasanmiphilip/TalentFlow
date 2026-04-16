using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Common.Services;
using TalentFlow.Application.Otp.Commands;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly TokenService _tokenService;

    public OtpController(IMediator mediator, TokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] Guid userId)
    {
        var code = await _mediator.Send(new GenerateOtpCommand
        {
            UserId = userId,
            Channel = "email"
        });

        return Ok(ApiResponse<string>.Success(code, "OTP generated successfully"));
    }

    [HttpPost("resend")]
    public async Task<IActionResult> Resend([FromBody] Guid userId)
    {
        var code = await _mediator.Send(new GenerateOtpCommand
        {
            UserId = userId,
            Channel = "email"
        });

        return Ok(ApiResponse<string>.Success(code, "OTP resent successfully"));
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ValidateOtpCommand command)
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
