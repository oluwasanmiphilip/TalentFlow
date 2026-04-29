// File: src/TalentFlow.Api/Controllers/UserController.cs

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Certificates.Commands;
using TalentFlow.Application.Certificates.Queries;
using TalentFlow.Application.Courses.DTOs;
using TalentFlow.Application.Courses.Queries;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Models;
using TalentFlow.Api.Models; // UpdateUserProfileRequest
using TalentFlow.Infrastructure.Services; // ImageProcessingHelper

namespace TalentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileStorageService _fileStorage;
        private readonly IUserRepository _userRepo;

        public UserController(
            IMediator mediator,
            IFileStorageService fileStorage,
            IUserRepository userRepo)
        {
            _mediator = mediator;
            _fileStorage = fileStorage;
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] RegisterUserCommand command)
        {
            var user = await _mediator.Send(command);
            return Ok(user);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            return user is null ? NotFound(ApiResponse.Fail<string>("User not found", 404)) : Ok(ApiResponse.Success<UserDto>(user, "User retrieved"));
        }

        [HttpGet("{learnerId}/courses")]
        public async Task<ActionResult<System.Collections.Generic.List<CourseDto>>> GetCoursesByLearner(string learnerId)
        {
            var courses = await _mediator.Send(new GetCoursesByLearnerQuery(learnerId));
            return Ok(courses);
        }

        [HttpGet("{id:guid}/certificates")]
        public async Task<IActionResult> GetCertificates(Guid id)
        {
            var result = await _mediator.Send(new GetCertificatesByUserIdQuery(id));
            return Ok(result);
        }

        [HttpPost("{id:guid}/certificates")]
        public async Task<IActionResult> CreateCertificate(Guid id, [FromBody] CreateCertificateCommand command)
        {
            command = command with { LearnerId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // PATCH api/user/{learnerId}/profile
        [Authorize]
        [HttpPatch("{learnerId}/profile")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UpdateProfile([FromRoute] string learnerId, [FromForm] UpdateUserProfileRequest request)
        {
            // ModelState / FluentValidation check -> return structured errors
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


            // 1. Load existing user by learnerId
            var user = await _userRepo.GetByLearnerIdAsync(learnerId);
            if (user == null)
                return NotFound(ApiResponse.Fail<string>("User not found", 404));

            // 2. Authorization: allow owner or Admin
            var callerIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "sub" || c.Type == "id" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!User.IsInRole("Admin"))
            {
                if (!Guid.TryParse(callerIdClaim, out var callerGuid) || callerGuid != user.Id)
                    return Forbid();
            }

            // 3. Handle optional photo upload: validate, process, save (image + thumbnail)
            string? photoUrl = request.ProfilePhotoUrl ?? user.ProfilePhotoUrl;
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
                    // Process image (resize + thumbnail)
                    var (imageBytes, thumbBytes) = await ImageProcessingHelper.ProcessImageAsync(request.ProfilePhoto, 1024, 200);

                    // Save main image and thumbnail via IFileStorageService (returns absolute URLs)
                    savedFileUrl = await _fileStorage.SaveFileAsync(imageBytes, request.ProfilePhoto.FileName, "profile-photos");
                    savedThumbUrl = await _fileStorage.SaveFileAsync(thumbBytes, "thumb_" + request.ProfilePhoto.FileName, "profile-photos");

                    photoUrl = savedFileUrl;
                }
                catch (Exception)
                {
                    return StatusCode(500, ApiResponse.Fail<string>("Failed to process or store profile photo", 500));
                }
            }

            // 4. Merge values: if request field is null/empty, keep existing
            var fullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName;
            var email = user.Email; // email change should be a separate flow with verification
            var phone = string.IsNullOrWhiteSpace(request.PhoneNumber) ? user.PhoneNumber : request.PhoneNumber;

            // 5. Determine UpdatedBy (use caller id or learnerId for system)
            var updatedBy = User.Identity?.Name ?? callerIdClaim ?? "system";

            // 6. Build and send the immutable command (positional args)
            var cmd = new UpdateUserProfileCommand(
                user.LearnerId,
                fullName,
                email,
                phone,
                updatedBy,
                string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio,
                string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl,
                request.EmailNotifications
            );

            var cmdResult = await _mediator.Send(cmd);

            if (!cmdResult)
            {
                // cleanup uploaded files if any
                if (!string.IsNullOrWhiteSpace(savedFileUrl))
                    await _fileStorage.DeleteFileAsync(savedFileUrl);
                if (!string.IsNullOrWhiteSpace(savedThumbUrl))
                    await _fileStorage.DeleteFileAsync(savedThumbUrl);

                return StatusCode(500, ApiResponse.Fail<string>("Failed to update profile", 500));
            }

            // 7. Fetch fresh DTO and return (DTO includes absolute ProfilePhotoUrl)
            var updatedDto = await _mediator.Send(new GetUserByIdQuery(user.Id));
            return Ok(ApiResponse.Success<UserDto>(updatedDto!, "Profile updated successfully."));
        }
    }
}
