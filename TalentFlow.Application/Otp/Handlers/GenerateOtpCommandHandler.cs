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
            // Expire any existing OTPs for this user
            var existingOtps = await _otpRepo.GetActiveOtpsByUserIdAsync(request.UserId);
            foreach (var otp in existingOtps)
            {
                otp.IsUsed = true; // mark as used/expired
                otp.ExpiresAt = DateTime.UtcNow; // force immediate expiry
                await _otpRepo.UpdateAsync(otp);
            }

            // Generate a fresh OTP
            var newOtp = new Random().Next(100000, 999999).ToString();

            var otpCode = new OtpCode
            {
                UserId = request.UserId,
                Code = newOtp,
                Channel = request.Channel,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _otpRepo.AddAsync(otpCode);

            return newOtp;
        }

    }
}
