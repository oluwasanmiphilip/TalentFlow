using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Users.Handlers
{
    public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, bool>
    {
        private readonly IUserRepository _repository;

        public UpdateUserProfileHandler(IUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByLearnerIdAsync(request.LearnerId, cancellationToken);
            if (user == null) return false;

            // Ensure UpdateProfile matches your User entity signature
            // Example: UpdateProfile(string fullName, string email, string phoneNumber, string updatedBy)
            user.UpdateProfile(
                request.FullName,
                request.Email,
                request.PhoneNumber,
                request.UpdatedBy ?? "system" // fallback if UpdatedBy is null
            );

            await _repository.UpdateAsync(user, cancellationToken);
            return true;
        }
    }
}
