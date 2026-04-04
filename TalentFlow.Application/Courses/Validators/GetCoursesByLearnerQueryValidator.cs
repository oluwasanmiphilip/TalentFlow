using FluentValidation;
using TalentFlow.Application.Courses.Queries;

namespace TalentFlow.Application.Courses.Validators
{
    public class GetCoursesByLearnerQueryValidator : AbstractValidator<GetCoursesByLearnerQuery>
    {
        public GetCoursesByLearnerQueryValidator()
        {
            RuleFor(q => q.LearnerId)
                .NotEmpty().WithMessage("LearnerId is required")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("LearnerId must be alphanumeric");
        }
    }
}
