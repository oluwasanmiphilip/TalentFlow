using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.Commands;

namespace TalentFlow.Application.Users.Handlers
{
    public class SaveLoginTokenHandler : IRequestHandler<SaveLoginTokenCommand>
    {
        private readonly IUserRepository _userRepository;

        public SaveLoginTokenHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(SaveLoginTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user != null)
            {
                user.LastLoginToken = request.Token;
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
        }
    }
}