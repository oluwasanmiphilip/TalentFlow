using MediatR;

namespace TalentFlow.Application.Courses.Events
{
    public class CourseCreatedEvent : INotification
    {
        public Guid CourseId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public CourseCreatedEvent(Guid courseId)
        {
            CourseId = courseId;
        }
    }
}
