using MediatR;
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
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByLearnerIdAsync(request.LearnerId);
            if (user == null) return false;

            user.UpdateProfile(request.Name);
            await _repository.UpdateAsync(user);

            return true;
        }
    }
}
