using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.Handlers
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepo;

        public ResetPasswordHandler(IMediator mediator, IUserRepository userRepo)
        {
            _mediator = mediator;
            _userRepo = userRepo;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var userDto = await _mediator.Send(new ValidateOtpCommand
            {
                UserId = request.UserId,
                Code = request.OtpCode
            }, cancellationToken);

            if (userDto == null) return false;

            await _mediator.Send(new UpdatePasswordCommand
            {
                UserId = request.UserId,
                NewPassword = request.NewPassword
            }, cancellationToken);

            return true;
        }
    }
}
