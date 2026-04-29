// File: TalentFlow.Application/Users/Handlers/UpdateUserProfileHandler.cs

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.Commands;

namespace TalentFlow.Application.Users.Handlers
{
    public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserProfileHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByLearnerIdAsync(request.LearnerId, cancellationToken);
            if (user == null) return false;

            // Update core profile fields via domain method
            user.UpdateProfile(request.FullName, request.Email, request.PhoneNumber, request.UpdatedBy);

            // Optional: Bio
            if (!string.IsNullOrWhiteSpace(request.Bio))
            {
                typeof(TalentFlow.Domain.Entities.User).GetProperty("Bio")?.SetValue(user, request.Bio);
            }

            // Optional: Profile photo URL
            if (!string.IsNullOrWhiteSpace(request.ProfilePhotoUrl))
            {
                typeof(TalentFlow.Domain.Entities.User).GetProperty("ProfilePhotoUrl")?.SetValue(user, request.ProfilePhotoUrl);
            }

            // Optional: Notification prefs stored as JSON
            if (request.EmailNotifications.HasValue)
            {
                var prefs = JsonSerializer.Serialize(new { email = request.EmailNotifications.Value });
                typeof(TalentFlow.Domain.Entities.User).GetProperty("NotificationPrefs")?.SetValue(user, prefs);
            }

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
