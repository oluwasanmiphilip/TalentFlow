using FluentValidation;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(IUserRepository userRepository)
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MustAsync(async (email, cancellation) =>
                {
                    var existingUser = await userRepository.GetByEmailAsync(email);
                    return existingUser == null;
                }).WithMessage("Email is already registered");

            RuleFor(u => u.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(255).WithMessage("Full name must be less than 255 characters");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(u => u.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => role == "Learner" || role == "Instructor" || role == "Admin")
                .WithMessage("Role must be Learner, Instructor, or Admin");
        }
    }
}
