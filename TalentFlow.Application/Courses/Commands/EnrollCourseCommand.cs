using MediatR;

namespace TalentFlow.Application.Courses.Commands
{
    // Command to enroll a learner in a course by slug
    public record EnrollCourseCommand(string LearnerId, string CourseSlug) : IRequest<bool>;
}
