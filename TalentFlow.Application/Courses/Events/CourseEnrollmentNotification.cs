using MediatR;
using TalentFlow.Application.Enrollments.Events;

namespace TalentFlow.Application.Courses.Events
{
    public class CourseEnrollmentNotification : INotification
    {
        public CourseEnrollmentEvent DomainEvent { get; }

        public CourseEnrollmentNotification(CourseEnrollmentEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
