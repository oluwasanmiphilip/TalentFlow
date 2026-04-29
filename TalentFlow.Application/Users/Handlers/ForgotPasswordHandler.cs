using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Otp.Commands;

namespace TalentFlow.Application.Users.Handlers
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IUserRepository _userRepo;
        private readonly IMediator _mediator;

        public ForgotPasswordHandler(IUserRepository userRepo, IMediator mediator)
        {
            _userRepo = userRepo;
            _mediator = mediator;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null) return false;

            await _mediator.Send(new GenerateOtpCommand
            {
                UserId = user.Id,
                Channel = "email"
            }, cancellationToken);

            return true;
        }
    }
}
