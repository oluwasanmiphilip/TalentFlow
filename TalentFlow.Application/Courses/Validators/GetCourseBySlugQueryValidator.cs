using FluentValidation;
using TalentFlow.Application.Courses.Queries;

namespace TalentFlow.Application.Courses.Validators
{
    public class GetCourseBySlugQueryValidator : AbstractValidator<GetCourseBySlugQuery>
    {
        public GetCourseBySlugQueryValidator()
        {
            RuleFor(q => q.Slug)
                .NotEmpty().WithMessage("Course slug is required")
                .MaximumLength(100).WithMessage("Course slug must be less than 100 characters");
        }
    }
}
