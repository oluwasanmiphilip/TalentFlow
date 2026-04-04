using FluentValidation;
using TalentFlow.Application.Courses.Commands;

namespace TalentFlow.Application.Courses.Validators
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Course title is required")
                .MaximumLength(200).WithMessage("Course title must be less than 200 characters");

            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("Course description is required")
                .MaximumLength(1000).WithMessage("Course description must be less than 1000 characters");
        }
    }
}
