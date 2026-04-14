using MediatR;

namespace TalentFlow.Application.Enrollments.Events
{
    public class CourseEnrollmentEvent : INotification
    {
        public Guid EnrollmentId { get; }
        public Guid CourseId { get; }
        public Guid UserId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public CourseEnrollmentEvent(Guid enrollmentId, Guid courseId, Guid userId)
        {
            EnrollmentId = enrollmentId;
            CourseId = courseId;
            UserId = userId;
        }
    }
}
