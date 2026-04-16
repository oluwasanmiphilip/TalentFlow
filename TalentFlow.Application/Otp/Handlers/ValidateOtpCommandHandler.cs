using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Otp.Commands;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Domain.Entities;

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
        var otp = await _otpRepo.GetByUserIdAndCodeAsync(request.UserId, request.Code);

        // Check validity
        if (otp == null || otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow)
            return null;

        // ✅ Mark OTP as used immediately
        otp.IsUsed = true;
        await _otpRepo.UpdateAsync(otp);

        // Return user info for token issuing
        var user = await _userRepo.GetByIdAsync(request.UserId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }
}
