using FluentValidation;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Validation;

namespace TalentFlow.Application.Users.Validators
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(u => u.Email).ValidTalentFlowEmail();

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
