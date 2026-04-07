using FluentValidation;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Validation;

namespace TalentFlow.Application.Users.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(u => u.Email).ValidTalentFlowEmail();

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
