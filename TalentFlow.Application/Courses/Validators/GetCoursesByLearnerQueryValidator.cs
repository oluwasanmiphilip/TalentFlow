using FluentValidation;
using TalentFlow.Application.Courses.Queries;

namespace TalentFlow.Application.Courses.Validators
{
    public class GetCoursesByLearnerQueryValidator : AbstractValidator<GetCoursesByLearnerQuery>
    {
        public GetCoursesByLearnerQueryValidator()
        {
            RuleFor(q => q.LearnerId)
                .NotEmpty().WithMessage("LearnerId is required"); // ✅ Guid check only
        }
    }
}
