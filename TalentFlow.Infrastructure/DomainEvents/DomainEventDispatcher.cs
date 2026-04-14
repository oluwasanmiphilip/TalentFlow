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
                    case TalentFlow.Domain.Events.UserCreatedEvent userCreated:
                        {
                            var userCreatedEvent = new TalentFlow.Application.Users.Events.UserCreatedEvent(userCreated.User.Id);
                            await _mediator.Publish(userCreatedEvent, ct);
                            break;
                        }

                    case TalentFlow.Domain.Events.UserProfileUpdatedEvent profileUpdated:
                        {
                            var userProfileUpdatedEvent = new TalentFlow.Application.Users.Events.UserProfileUpdatedEvent(profileUpdated.User.Id);
                            await _mediator.Publish(userProfileUpdatedEvent, ct);
                            break;
                        }

                    case TalentFlow.Domain.Events.CourseCreatedEvent courseCreated:
                        {
                            var courseCreatedEvent = new TalentFlow.Application.Courses.Events.CourseCreatedEvent(courseCreated.Course.Id);
                            await _mediator.Publish(courseCreatedEvent, ct);
                            break;
                        }

                    case TalentFlow.Domain.Events.CourseEnrollmentDomainEvent enrollmentEvent:
                        {
                            var courseEnrollmentEvent = new TalentFlow.Application.Enrollments.Events.CourseEnrollmentEvent(
                                enrollmentEvent.Enrollment.Id,
                                enrollmentEvent.Course.Id,
                                enrollmentEvent.User.Id
                            );
                            await _mediator.Publish(courseEnrollmentEvent, ct);
                            break;
                        }

                    case TalentFlow.Domain.Events.AssessmentCreatedDomainEvent assessmentCreated:
                        {
                            var assessmentCreatedEvent = new TalentFlow.Application.Assessments.Events.AssessmentCreatedEvent(assessmentCreated.Assessment.Id);
                            await _mediator.Publish(assessmentCreatedEvent, ct);
                            break;
                        }



                    case TalentFlow.Domain.Events.NotificationSentEvent notificationSent:
                        {
                            var notificationSentEvent = new TalentFlow.Application.Notifications.Events.NotificationSentEvent(notificationSent.Notification.Id);
                            await _mediator.Publish(notificationSentEvent, ct);
                            break;
                        }



                    default:
                        break;
                }
            }
        }
    }
}
