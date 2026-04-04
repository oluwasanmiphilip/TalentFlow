using MediatR;
using TalentFlow.Domain.Common;
using TalentFlow.Application.Users.Events;
using TalentFlow.Application.Enrollments.Events;
using TalentFlow.Application.Courses.Events;

namespace TalentFlow.Infrastructure.Events
{
    public class DomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchEventsAsync(IEnumerable<object> domainEvents, CancellationToken ct = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case TalentFlow.Domain.Events.UserCreatedDomainEvent userCreated:
                        await _mediator.Publish(new UserCreatedNotification(userCreated), ct);
                        break;

                    case TalentFlow.Domain.Events.UserProfileUpdatedDomainEvent profileUpdated:
                        await _mediator.Publish(new UserProfileUpdatedNotification(profileUpdated), ct);
                        break;

                    case TalentFlow.Domain.Events.CourseCreatedDomainEvent courseCreated:
                        await _mediator.Publish(new CourseCreatedNotification(new CourseCreatedEvent(courseCreated.Course.Id)), ct);
                        break;

                    case TalentFlow.Domain.Events.CourseEnrollmentDomainEvent enrollmentEvent:
                        await _mediator.Publish(new CourseEnrollmentNotification(new TalentFlow.Application.Enrollments.Events.CourseEnrollmentEvent(enrollmentEvent.Enrollment.Id, enrollmentEvent.Course.Id, enrollmentEvent.User.Id)), ct);
                        break;

                    case TalentFlow.Domain.Events.NotificationSentDomainEvent notificationSent:
                        await _mediator.Publish(new TalentFlow.Application.Notifications.Events.NotificationSentEvent(notificationSent.Notification.Id), ct);
                        break;

                    case INotification notification:
                        await _mediator.Publish(notification, ct);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
