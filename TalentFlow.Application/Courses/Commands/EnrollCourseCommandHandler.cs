using MediatR;

namespace TalentFlow.Application.Courses.Commands
{
    public class EnrollCourseCommand : IRequest<bool>
    {
        public string LearnerId { get; set; } = string.Empty;   // ✅ string, matches User entity
        public string CourseSlug { get; set; } = string.Empty;
    }
}
