using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class CourseEnrollmentDomainEvent : IDomainEvent
    {
        public Enrollment Enrollment { get; }
        public Course Course { get; }
        public User User { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public CourseEnrollmentDomainEvent(Enrollment enrollment, Course course, User user)
        {
            Enrollment = enrollment;
            Course = course;
            User = user;
        }
    }
}
