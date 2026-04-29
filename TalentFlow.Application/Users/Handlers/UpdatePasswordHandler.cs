using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.Handlers
{
    public class UpdatePasswordHandler : IRequestHandler<UpdatePasswordCommand, bool>
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;

        public UpdatePasswordHandler(IUserRepository userRepo, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) return false;

            var newHash = _passwordHasher.Hash(request.NewPassword);
            user.UpdateProfile(user.FullName, user.Email, user.PhoneNumber, "system");
            user.GetType().GetProperty("PasswordHash")?.SetValue(user, newHash);

            await _userRepo.UpdateAsync(user, cancellationToken);
            return true;
        }
    }
}
