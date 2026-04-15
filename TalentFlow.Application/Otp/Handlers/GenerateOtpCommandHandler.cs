using MediatR;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Otp.Handlers
{
    public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, string>
    {
        private readonly IOtpRepository _otpRepo;

        public GenerateOtpCommandHandler(IOtpRepository otpRepo)
        {
            _otpRepo = otpRepo;
        }

        public async Task<string> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var otpCode = new OtpCode
            {
                UserId = request.UserId,
                Code = otp,
                Channel = request.Channel,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _otpRepo.AddAsync(otpCode);
            return otp;
        }
    }
}
