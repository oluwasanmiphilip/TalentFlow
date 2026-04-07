using MediatR;

namespace TalentFlow.Application.Courses.Commands
{
    public class EnrollCourseCommand : IRequest<bool>
    {
        public Guid LearnerId { get; set; }   // ✅ Guid
        public string CourseSlug { get; set; } = string.Empty;
    }
}
