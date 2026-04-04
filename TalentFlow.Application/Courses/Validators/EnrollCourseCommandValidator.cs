using FluentValidation;
using TalentFlow.Application.Courses.Commands;

namespace TalentFlow.Application.Courses.Validators
{
    public class EnrollCourseCommandValidator : AbstractValidator<EnrollCourseCommand>
    {
        public EnrollCourseCommandValidator()
        {
            RuleFor(c => c.LearnerId)
                .NotEmpty().WithMessage("LearnerId is required")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("LearnerId must be alphanumeric");

            RuleFor(c => c.CourseSlug)
                .NotEmpty().WithMessage("Course slug is required")
                .MaximumLength(100).WithMessage("Course slug must be less than 100 characters");
        }
    }
}
