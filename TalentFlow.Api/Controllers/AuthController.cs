// File Path: src/TalentFlow.Api/Controllers/AuthController.cs

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Exceptions;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Infrastructure.Services; // ImageProcessingHelper

namespace TalentFlow.Api.Controllers
{
    // Request model used to accept both JSON-less multipart/form-data and simple form posts
    public class RegisterUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Discipline { get; set; } = string.Empty;
        public int CohortYear { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        // Optional profile fields
        public string? Bio { get; set; }
        public bool? EmailNotifications { get; set; }

        // Optional file upload (multipart/form-data)
        public IFormFile? ProfilePhoto { get; set; }

        // Optional direct URL (client may provide a hosted image URL instead of uploading)
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

            // Normalize email
            var normalizedEmail = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return BadRequest(ApiResponse.Fail<string>("Email is required", 400));

            // Early check for friendly UX
            if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
            {
                var fieldErrors = new { Email = new[] { "Email is already in use. Try logging in or reset your password." } };
                return Conflict(ApiResponse.Fail<object>("Email already registered", 409, fieldErrors));
            }

            // File processing
            string? photoUrl = request.ProfilePhotoUrl;
            string? savedFileUrl = null;
            string? savedThumbUrl = null;

            if (request.ProfilePhoto != null)
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowed.Contains(request.ProfilePhoto.ContentType?.ToLowerInvariant()))
                    return BadRequest(ApiResponse.Fail<string>("Unsupported image type. Allowed: jpeg, png, webp", 400));

                const long maxBytes = 5 * 1024 * 1024;
                if (request.ProfilePhoto.Length > maxBytes)
                    return BadRequest(ApiResponse.Fail<string>("Profile photo exceeds 5 MB limit", 400));

                try
                {
                    var (imageBytes, thumbBytes) = await ImageProcessingHelper.ProcessImageAsync(request.ProfilePhoto, maxDimension: 1024, thumbSize: 200);
                    savedFileUrl = await _fileStorage.SaveFileAsync(imageBytes, request.ProfilePhoto.FileName, "profile-photos");
                    savedThumbUrl = await _fileStorage.SaveFileAsync(thumbBytes, "thumb_" + request.ProfilePhoto.FileName, "profile-photos");
                    photoUrl = savedFileUrl;
                }
                catch (Exception)
                {
                    return StatusCode(500, ApiResponse.Fail<string>("Failed to process or store profile photo", 500));
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
                var userDto = await _mediator.Send(command, HttpContext.RequestAborted);

                if (userDto == null)
                {
                    // Handler indicated failure (duplicate or validation) — cleanup uploaded files
                    if (!string.IsNullOrWhiteSpace(savedFileUrl))
                        await _fileStorage.DeleteFileAsync(savedFileUrl);
                    if (!string.IsNullOrWhiteSpace(savedThumbUrl))
                        await _fileStorage.DeleteFileAsync(savedThumbUrl);

                    var fieldErrors = new { Email = new[] { "Email is already in use. Try logging in or reset your password." } };
                    return Conflict(ApiResponse.Fail<object>("Email already registered", 409, fieldErrors));
                }

                // Send OTP if opted in
                var shouldSendEmail = command.EmailNotifications ?? true;
                if (shouldSendEmail)
                {
                    await _mediator.Send(new GenerateOtpCommand { UserId = userDto.Id, Channel = "email" }, HttpContext.RequestAborted);
                }

                var responsePayload = new
                {
                    id = userDto.Id,
                    full_name = userDto.FullName,
                    email = userDto.Email,
                    role = userDto.Role,
                    profile_photo_url = userDto.ProfilePhotoUrl,
                    bio = userDto.Bio,
                    email_notifications = userDto.EmailNotifications
                };

                // Use Created with payload (avoids referencing a non-existent route)
                return Created(string.Empty, ApiResponse.Success<object>(responsePayload, "User registered successfully. OTP sent to your email.", 201));
            }
            catch (DuplicateEmailException)
            {
                // cleanup uploaded files if registration failed
                if (!string.IsNullOrWhiteSpace(savedFileUrl))
                    await _fileStorage.DeleteFileAsync(savedFileUrl);
                if (!string.IsNullOrWhiteSpace(savedThumbUrl))
                    await _fileStorage.DeleteFileAsync(savedThumbUrl);

                var fieldErrors = new { Email = new[] { "Email is already in use. Try logging in or reset your password." } };
                return Conflict(ApiResponse.Fail<object>("Email already registered", 409, fieldErrors));
            }
        }

        // ============================
        // LOGIN (direct token issuance)
        // ============================
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var userDto = await _mediator.Send(command);

            if (userDto == null)
                return Unauthorized(ApiResponse.Fail<string>("Invalid email or password", 401));

            var accessToken = _tokenService.GenerateToken(userDto.Id, userDto.Email, userDto.Role);
            var refreshToken = _tokenService.GenerateRefreshToken(userDto.Id, userDto.Email, userDto.Role);

            await _mediator.Send(new SaveLoginTokenCommand
            {
                UserId = userDto.Id,
                Token = accessToken
            });

            return Ok(ApiResponse.Success<object>(new
            {
                accessToken,
                refreshToken
            }, "Login successful."));
        }

        // ============================
        // VERIFY OTP (for registration or password reset)
        // ============================
        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] ValidateOtpCommand command)
        {
            var userDto = await _mediator.Send(command);

            if (userDto == null)
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

            await _mediator.Send(new SaveLoginTokenCommand
            {
                UserId = userDto.Id,
                Token = accessToken
            });

            return Ok(ApiResponse.Success<object>(new
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
                return NotFound(ApiResponse.Fail<string>("User not found", 404));

            await _mediator.Send(new GenerateOtpCommand
            {
                UserId = userDto.Id,
                Channel = "email"
            });

            return Ok(ApiResponse.Success<string>("New OTP sent to your email."));
        }

        // ============================
        // FORGOT PASSWORD
        // ============================
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var userDto = await _mediator.Send(new GetUserByEmailCommand { Email = command.Email });

            if (userDto == null)
                return NotFound(ApiResponse.Fail<string>("User not found", 404));

            await _mediator.Send(new GenerateOtpCommand
            {
                UserId = userDto.Id,
                Channel = "email"
            });

            return Ok(ApiResponse.Success<string>("OTP sent to your email for password reset."));
        }

        // ============================
        // RESET PASSWORD
        // ============================
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var userDto = await _mediator.Send(new ValidateOtpCommand
            {
                UserId = command.UserId,
                Code = command.OtpCode
            });

            if (userDto == null)
                return BadRequest(ApiResponse.Fail<string>("Invalid or expired OTP", 400));

            await _mediator.Send(new UpdatePasswordCommand
            {
                UserId = userDto.Id,
                NewPassword = command.NewPassword
            });

            return Ok(ApiResponse.Success<string>("Password reset successfully."));
        }
    }
}
