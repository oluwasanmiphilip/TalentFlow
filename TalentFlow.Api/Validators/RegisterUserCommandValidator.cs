using FluentValidation;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IUserRepository userRepo)
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress()
            .MustAsync(async (email, ct) =>
            {
                if (string.IsNullOrWhiteSpace(email)) return true;
                return !await userRepo.ExistsByEmailAsync(email.Trim().ToLowerInvariant());
            })
            .WithMessage("Email is already registered.");

        RuleFor(x => x.FullName).NotEmpty().Length(2, 100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        // other rules...
    }
}
