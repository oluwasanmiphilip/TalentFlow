// File Path: src/TalentFlow.Api/Controllers/AuthController.cs

using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentFlow.Api.Controllers.Requests;
using TalentFlow.Application.Common.Exceptions;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Messages;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Infrastructure.Jobs;
using TalentFlow.Infrastructure.Services;

namespace TalentFlow.Api.Controllers
{
    public class RegisterUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Discipline { get; set; } = string.Empty;
        public int CohortYear { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Bio { get; set; }
        public bool? EmailNotifications { get; set; }

        public IFormFile? ProfilePhoto { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _tokenService;
        private readonly IFileStorageService _fileStorage;
        private readonly IUserRepository _userRepository;

        public AuthController(
            IMediator mediator,
            IJwtTokenService tokenService,
            IFileStorageService fileStorage,
            IUserRepository userRepository)
        {
            _mediator = mediator;
            _tokenService = tokenService;
            _fileStorage = fileStorage;
            _userRepository = userRepository;
        }

        // ============================
        // REGISTER
        // ============================
        [AllowAnonymous]
        [HttpPost("register")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Register([FromForm] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse.Fail<object>("Validation failed", 400, errors));
            }

            var normalizedEmail = (request.Email ?? string.Empty).Trim().ToLowerInvariant();

            if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
            {
                return Conflict(ApiResponse.Fail<object>(
                    "Email already registered",
                    409,
                    new { Email = new[] { "Email already in use" } }
                ));
            }

            string? photoUrl = request.ProfilePhotoUrl;
            string? savedFileUrl = null;
            string? savedThumbUrl = null;

            if (request.ProfilePhoto != null)
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };

                if (!allowed.Contains(request.ProfilePhoto.ContentType?.ToLowerInvariant()))
                    return BadRequest(ApiResponse.Fail<string>("Invalid image type", 400));

                const long maxBytes = 5 * 1024 * 1024;

                if (request.ProfilePhoto.Length > maxBytes)
                    return BadRequest(ApiResponse.Fail<string>("Image too large", 400));

                try
                {
                    var (imageBytes, thumbBytes) =
                        await ImageProcessingHelper.ProcessImageAsync(
                            request.ProfilePhoto,
                            1024,
                            200);

                    savedFileUrl =
                        await _fileStorage.SaveFileAsync(imageBytes, request.ProfilePhoto.FileName, "profile-photos");

                    savedThumbUrl =
                        await _fileStorage.SaveFileAsync(thumbBytes, "thumb_" + request.ProfilePhoto.FileName, "profile-photos");

                    photoUrl = savedFileUrl;
                }
                catch
                {
                    return StatusCode(500,
                        ApiResponse.Fail<string>("Image processing failed", 500));
                }
            }

            var command = new RegisterUserCommand
            {
                Email = normalizedEmail,
                FullName = request.FullName,
                Password = request.Password,
                Role = request.Role,
                Discipline = request.Discipline,
                CohortYear = request.CohortYear,
                PhoneNumber = request.PhoneNumber,
                Bio = request.Bio,
                ProfilePhotoUrl = photoUrl,
                EmailNotifications = request.EmailNotifications
            };

            try
            {
                var userDto = await _mediator.Send(command);

                if (userDto == null)
                {
                    if (savedFileUrl != null)
                        await _fileStorage.DeleteFileAsync(savedFileUrl);

                    if (savedThumbUrl != null)
                        await _fileStorage.DeleteFileAsync(savedThumbUrl);

                    return Conflict(ApiResponse.Fail<object>(
                        "Email already registered",
                        409));
                }

                // ============================
                // OTP via Hangfire
                // ============================
                var otpMessage = new OtpMessage
                {
                    UserId = userDto.Id,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    Channel = "email",
                    Code = Guid.NewGuid().ToString("N")[..6],
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };

                BackgroundJob.Enqueue<OtpJobService>(
                    job => job.SendOtpAsync(otpMessage)
                );

                return Created(string.Empty,
                    ApiResponse.Success<object>(
                        new
                        {
                            userDto.Id,
                            userDto.FullName,
                            userDto.Email,
                            userDto.Role,
                            userDto.ProfilePhotoUrl
                        },
                        "User registered successfully. OTP sent."
                    ));
            }
            catch (DuplicateEmailException)
            {
                if (savedFileUrl != null)
                    await _fileStorage.DeleteFileAsync(savedFileUrl);

                if (savedThumbUrl != null)
                    await _fileStorage.DeleteFileAsync(savedThumbUrl);

                return Conflict(ApiResponse.Fail<object>(
                    "Email already exists",
                    409));
            }
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
                return Unauthorized(ApiResponse.Fail<string>("Invalid credentials", 401));

            var accessToken =
                _tokenService.GenerateToken(userDto.Id, userDto.Email, userDto.Role);

            var refreshToken =
                _tokenService.GenerateRefreshToken(userDto.Id, userDto.Email, userDto.Role);

            await _mediator.Send(new SaveLoginTokenCommand
            {
                UserId = userDto.Id,
                Token = accessToken
            });

            return Ok(ApiResponse.Success(new
            {
                accessToken,
                refreshToken
            }, "Login successful"));
        }

        // ============================
        // VERIFY OTP
        // ============================
        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var userDto = await _mediator.Send(new GetUserByEmailCommand
            {
                Email = email
            });

            if (userDto == null)
                return BadRequest(ApiResponse.Fail<string>("User not found", 400));

            var result = await _mediator.Send(new ValidateOtpCommand
            {
                UserId = userDto.Id,
                Code = request.Code
            });

            if (result == null)
                return BadRequest(ApiResponse.Fail<string>("Invalid or expired OTP", 400));

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

            return Ok(ApiResponse.Success(new
            {
                accessToken,
                refreshToken
            }, "OTP verified successfully"));
        }

        // ============================
        // RESEND OTP
        // ============================
        [AllowAnonymous]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _mediator.Send(new GetUserByEmailCommand
            {
                Email = email
            });

            if (user == null)
                return NotFound("User not found");

            BackgroundJob.Enqueue<OtpJobService>(job =>
    job.SendOtpAsync(new OtpMessage
    {
        UserId = user.Id,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Channel = "email",
        Code = Guid.NewGuid().ToString("N").Substring(0, 6),
        ExpiresAt = DateTime.UtcNow.AddMinutes(5)
    })
);

            return Ok("OTP is being sent");
        }

        // ============================
        // FORGOT PASSWORD
        // ============================
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var userDto =
                await _mediator.Send(new GetUserByEmailCommand { Email = command.Email });

            if (userDto == null)
                return NotFound(ApiResponse.Fail<string>("User not found", 404));

            await _mediator.Send(new GenerateOtpCommand
            {
                UserId = userDto.Id,
                Channel = "email"
            });

            return Ok(ApiResponse.Success("OTP sent"));
        }

        // ============================
        // RESET PASSWORD
        // ============================
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
    [FromBody] ResetPasswordCommand command)
        {
            var email =
                command.Email.Trim().ToLowerInvariant();

            // ✅ Find user by email
            var userDto =
                await _mediator.Send(new GetUserByEmailCommand
                {
                    Email = email
                });

            if (userDto == null)
            {
                return NotFound(
                    ApiResponse.Fail<string>(
                        "User not found",
                        404));
            }

            // ✅ Validate OTP
            var otpResult =
                await _mediator.Send(new ValidateOtpCommand
                {
                    UserId = userDto.Id,
                    Code = command.OtpCode
                });

            if (otpResult == null)
            {
                return BadRequest(
                    ApiResponse.Fail<string>(
                        "Invalid or expired OTP",
                        400));
            }

            // ✅ Update password
            await _mediator.Send(new UpdatePasswordCommand
            {
                UserId = userDto.Id,
                NewPassword = command.NewPassword
            });

            return Ok(
                ApiResponse.Success(
                    "Password reset successful"));
        }
    }
}