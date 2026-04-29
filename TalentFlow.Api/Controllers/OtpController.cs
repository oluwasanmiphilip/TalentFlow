// File Path: src/TalentFlow.Api/Controllers/OtpController.cs

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Common.Services;
using TalentFlow.Application.Otp.Commands;

namespace TalentFlow.Api.Controllers
{
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
            if (userId == Guid.Empty)
                return BadRequest(ApiResponse.Fail<string>("UserId is required", 400));

            var code = await _mediator.Send(new GenerateOtpCommand
            {
                UserId = userId,
                Channel = "email"
            });

            return Ok(ApiResponse.Success<string>(code, "OTP generated successfully"));
        }

        [HttpPost("resend")]
        public async Task<IActionResult> Resend([FromBody] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(ApiResponse.Fail<string>("UserId is required", 400));

            var code = await _mediator.Send(new GenerateOtpCommand
            {
                UserId = userId,
                Channel = "email"
            });

            return Ok(ApiResponse.Success<string>(code, "OTP resent successfully"));
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateOtpCommand command)
        {
            if (command == null || command.UserId == Guid.Empty || string.IsNullOrWhiteSpace(command.Code))
                return BadRequest(ApiResponse.Fail<string>("UserId and OTP code are required", 400));

            var userDto = await _mediator.Send(command);
            if (userDto == null)
                return BadRequest(ApiResponse.Fail<string>("Invalid or expired OTP", 400));

            var tokens = _tokenService.IssueTokens(userDto);

            return Ok(ApiResponse.Success<object>(new
            {
                accessToken = tokens.accessToken,
                refreshToken = tokens.refreshToken
            }, "OTP verified successfully. Tokens issued."));
        }
    }
}
