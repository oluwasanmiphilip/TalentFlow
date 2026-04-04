using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Domain.Events
{
    public class CourseCreatedDomainEvent : IDomainEvent
    {
        public Course Course { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public CourseCreatedDomainEvent(Course course)
        {
            Course = course;
        }
    }
}
