using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Otp.Commands;

public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, string>
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly IOtpCacheService _cache;

    public GenerateOtpCommandHandler(
        IUserRepository userRepo,
        IEmailService emailService,
        ISmsService smsService,
        IOtpCacheService cache)
    {
        _userRepo = userRepo;
        _emailService = emailService;
        _smsService = smsService;
        _cache = cache;
    }

    public async Task<string> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId);

        if (user == null)
            throw new Exception("User not found");

        if (string.IsNullOrWhiteSpace(user.Email) &&
            string.IsNullOrWhiteSpace(user.PhoneNumber))
            throw new Exception("User has no valid contact channel");

        // 1. Generate OTP (no external generator needed)
        var otp = new Random().Next(100000, 999999).ToString();

        // 2. Save OTP in Redis
        await _cache.SaveOtpAsync(user.Id, otp, TimeSpan.FromMinutes(5));

        // 3. Send OTP based on channel
        var channel = request.Channel?.ToLowerInvariant();

        if (channel == "email")
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new Exception("User email is missing");

            await _emailService.SendOtpAsync(user.Email, otp);
        }
        else if (channel == "sms")
        {
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                throw new Exception("User phone number is missing");

            await _smsService.SendOtpAsync(user.PhoneNumber, otp);
        }
        else
        {
            throw new Exception("Invalid OTP channel");
        }

        return otp;
    }
}