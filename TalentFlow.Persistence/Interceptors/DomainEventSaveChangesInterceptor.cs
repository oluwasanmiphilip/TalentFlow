using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TalentFlow.Domain.Common;
using TalentFlow.Application.Courses.Events;
using TalentFlow.Application.Enrollments.Events;
using TalentFlow.Application.Notifications;
using TalentFlow.Application.Notifications.Events;

namespace TalentFlow.Persistence.Interceptors
{
    public class DomainEventSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator _mediator;

        public DomainEventSaveChangesInterceptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context == null) return result;

            // Collect all entities with domain events
            var entitiesWithEvents = context.ChangeTracker
                .Entries<EntityBase>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear events from entities
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            // Publish wrapped notifications
            foreach (var domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case CourseCreatedEvent courseCreated:
                        await _mediator.Publish(new CourseCreatedNotification(courseCreated), cancellationToken);
                        break;

                    case CourseEnrollmentEvent courseEnrollment:
                        await _mediator.Publish(new CourseEnrollmentNotification(courseEnrollment), cancellationToken);
                        break;

                    case NotificationSentEvent notificationSent:
                        // NotificationSentEvent already implements INotification — publish it directly
                        await _mediator.Publish(notificationSent, cancellationToken);
                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        // No wrapper notification needed for NotificationSentEvent — it implements INotification
    }
}
