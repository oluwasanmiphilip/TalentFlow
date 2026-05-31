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

        public ResetPasswordHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Normalize email
            var email = request.Email.Trim().ToLowerInvariant();

            // 2. Get user by email
            var userDto = await _mediator.Send(
                new GetUserByEmailCommand
                {
                    Email = email
                },
                cancellationToken);

            if (userDto == null)
                return false;

            // 3. Validate OTP using UserId from DB
            var otpResult = await _mediator.Send(
                new ValidateOtpCommand
                {
                    UserId = userDto.Id,
                    Code = request.OtpCode
                },
                cancellationToken);

            if (otpResult == null)
                return false;

            // 4. Update password using real UserId
            await _mediator.Send(
                new UpdatePasswordCommand
                {
                    UserId = userDto.Id,
                    NewPassword = request.NewPassword
                },
                cancellationToken);

            return true;
        }
    }
}