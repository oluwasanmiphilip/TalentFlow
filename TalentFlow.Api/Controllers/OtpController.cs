// File Path: src/TalentFlow.Api/Controllers/OtpController.cs

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Common.Services;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Users.Commands;

namespace TalentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _tokenService;

        public OtpController(IMediator mediator, IJwtTokenService tokenService)
        {
            _mediator = mediator;
            _tokenService = tokenService;
        }

        // ============================
        // GENERATE OTP
        // ============================
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateOtpRequest request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(ApiResponse.Fail<string>("UserId is required", 400));

            var code = await _mediator.Send(new GenerateOtpCommand
            {
                UserId = request.UserId,
                Channel = "email"
            });

            return Ok(ApiResponse.Success(code, "OTP generated successfully"));
        }

        // ============================
        // RESEND OTP
        // ============================
        [HttpPost("resend")]
        public async Task<IActionResult> Resend([FromBody] GenerateOtpRequest request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(ApiResponse.Fail<string>("UserId is required", 400));

            var code = await _mediator.Send(new GenerateOtpCommand
            {
                UserId = request.UserId,
                Channel = "email"
            });

            return Ok(ApiResponse.Success(code, "OTP resent successfully"));
        }

        // ============================
        // VALIDATE OTP
        // ============================
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateOtpCommand command)
        {
            if (command == null ||
                command.UserId == Guid.Empty ||
                string.IsNullOrWhiteSpace(command.Code))
            {
                return BadRequest(ApiResponse.Fail<string>(
                    "UserId and OTP code are required", 400));
            }

            var result = await _mediator.Send(command);

            if (result == null)
                return BadRequest(ApiResponse.Fail<string>("Invalid or expired OTP", 400));

            var accessToken = _tokenService.GenerateToken(
                result.Id,
                result.Email,
                result.Role
            );

            var refreshToken = _tokenService.GenerateRefreshToken(
                result.Id,
                result.Email,
                result.Role
            );

            return Ok(ApiResponse.Success(new
            {
                accessToken,
                refreshToken
            }, "OTP verified successfully. Tokens issued."));
        }
    }

    // ============================
    // REQUEST DTO (IMPORTANT FIX)
    // ============================
    public class GenerateOtpRequest
    {
        public Guid UserId { get; set; }
    }
}