using MediatR;


namespace TalentFlow.Application.Courses.Events
{
    public class CourseCreatedNotification : INotification
    {
        public CourseCreatedEvent DomainEvent { get; }

        public CourseCreatedNotification(CourseCreatedEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
