using MediatR;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Otp.Handlers
{
    public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, string>
    {
        private readonly IOtpRepository _otpRepo;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public GenerateOtpCommandHandler(
            IOtpRepository otpRepo,
            IUserRepository userRepo,
            IEmailService emailService,
            ISmsService smsService)
        {
            _otpRepo = otpRepo;
            _userRepo = userRepo;
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task<string> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. Get user
            var user = await _userRepo.GetByIdAsync(request.UserId);
            if (user == null)
                throw new Exception("User not found");

            if (string.IsNullOrWhiteSpace(user.Email) && string.IsNullOrWhiteSpace(user.PhoneNumber))
                throw new Exception("User does not have valid contact info");

            // 2. Expire old OTPs
            var existingOtps = await _otpRepo.GetActiveOtpsByUserIdAsync(request.UserId);
            foreach (var otp in existingOtps)
            {
                otp.IsUsed = true;
                otp.ExpiresAt = DateTime.UtcNow;
                await _otpRepo.UpdateAsync(otp);
            }

            // 3. Generate new OTP
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

            // 4. Send via chosen channel
            if (request.Channel == "email")
            {
                await _emailService.SendOtpAsync(user.Email, newOtp);
            }
            else if (request.Channel == "sms")
            {
                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                    throw new Exception("User does not have a valid phone number");

                await _smsService.SendOtpAsync(user.PhoneNumber, newOtp);
            }
            else
            {
                throw new Exception("Unsupported channel");
            }

            return newOtp; // ⚠️ keep only for development/logging
        }
    }
}
