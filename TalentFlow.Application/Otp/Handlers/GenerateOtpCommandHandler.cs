using MediatR;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Otp.Handlers
{
    public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, string>
    {
        private readonly IOtpRepository _otpRepo;
        private readonly INotificationService _emailService;
        private readonly ISmsService _smsService;
        private readonly IUserRepository _userRepo;

        public GenerateOtpCommandHandler(
            IOtpRepository otpRepo,
            INotificationService emailService,
            ISmsService smsService,
            IUserRepository userRepo)
        {
            _otpRepo = otpRepo;
            _emailService = emailService;
            _smsService = smsService;
            _userRepo = userRepo;
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

            // Fetch user details (email/phone) from repository
            var user = await _userRepo.GetByIdAsync(request.UserId);

            if (request.Channel == "email")
            {
                await _emailService.SendAsync(new NotificationMessage
                {
                    UserId = request.UserId,
                    Channel = "email",
                    Message = $"Your TalentFlow OTP code is {otp}",
                    RecipientEmail = user.Email
                });
            }
            else if (request.Channel == "sms")
            {
                await _smsService.SendOtpAsync(user.PhoneNumber, otp);
            }

            return otp;
        }
    }
}
