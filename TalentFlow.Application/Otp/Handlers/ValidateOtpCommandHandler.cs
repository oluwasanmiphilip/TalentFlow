using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Otp.Handlers
{
    public class ValidateOtpCommandHandler : IRequestHandler<ValidateOtpCommand, UserDto?>
    {
        private readonly IOtpRepository _otpRepo;
        private readonly IUserRepository _userRepo;

        public ValidateOtpCommandHandler(IOtpRepository otpRepo, IUserRepository userRepo)
        {
            _otpRepo = otpRepo;
            _userRepo = userRepo;
        }

        public async Task<UserDto?> Handle(ValidateOtpCommand request, CancellationToken cancellationToken)
        {
            var otp = await _otpRepo.GetByUserIdAsync(request.UserId, cancellationToken);
            if (otp == null || otp.Code != request.Code || otp.ExpiresAt < DateTime.UtcNow)
                return null;

            // ✅ Mark OTP as used once validated
            await _otpRepo.MarkAsUsedAsync(request.UserId, cancellationToken);

            var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Discipline = user.Discipline,
                CohortYear = user.CohortYear,
                PhoneNumber = user.PhoneNumber
            };
        }


    }
}
