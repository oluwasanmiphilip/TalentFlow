using MediatR;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Domain.Entities;
using TalentFlow.Application.Common.Exceptions; // ✅ Import your custom exceptions
using System.Net.Mail; // ✅ For email format validation

namespace TalentFlow.Application.Users.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Normalize and validate email early
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (!IsValidEmail(email))
                throw new ArgumentException("Email format is invalid");

            // 1) Check uniqueness at application level
            if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            {
                // ✅ Throw custom exception instead of returning null
                throw new DuplicateEmailException(email);
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

            // Build notification prefs JSON; default to true if not provided
            var emailPref = request.EmailNotifications ?? true;
            var notificationPrefs = JsonSerializer.Serialize(new { email = emailPref });

            var user = new User(
                email,
                request.FullName,
                passwordHash,
                request.Role,
                request.Discipline,
                request.CohortYear,
                request.PhoneNumber
            );

            // Set optional profile fields directly
            if (!string.IsNullOrWhiteSpace(request.Bio))
                user.GetType().GetProperty("Bio")?.SetValue(user, request.Bio);

            if (!string.IsNullOrWhiteSpace(request.ProfilePhotoUrl))
                user.GetType().GetProperty("ProfilePhotoUrl")?.SetValue(user, request.ProfilePhotoUrl);

            user.GetType().GetProperty("NotificationPrefs")?.SetValue(user, notificationPrefs);

            // Attach ProfileUser if provided
            if (request.ProfileUser != null)
            {
                var profile = new ProfileUser(
                    user.Id,
                    request.ProfileUser.Bio ?? string.Empty,
                    request.ProfileUser.ProfilePhotoUrl,
                    request.ProfileUser.ProgressVisibility,
                    request.ProfileUser.NotificationPrefs
                );

                user.AttachProfile(profile);
            }

            try
            {
                await _userRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException dbEx)
            {
                // ✅ Still catch DB-level uniqueness violations (race conditions)
                throw new DuplicateEmailException(email, dbEx);
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Discipline = user.Discipline,
                CohortYear = user.CohortYear,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                EmailNotifications = emailPref,
                LearnerId = user.LearnerId
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
