using Microsoft.EntityFrameworkCore;
using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Common;

namespace TalentFlow.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TalentFlowDbContext _context;
        private readonly IMediator _mediator;

        public UnitOfWork(TalentFlowDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Save changes to the database
            var result = await _context.SaveChangesAsync(cancellationToken);

            // Collect domain events from tracked entities
            var entitiesWithEvents = _context.ChangeTracker
                .Entries<EntityBase>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear events from entities
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            // Dispatch domain events via MediatR: map domain events to application notifications
            foreach (var domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case TalentFlow.Domain.Events.UserCreatedDomainEvent userCreated:
                        await _mediator.Publish(new TalentFlow.Application.Users.Events.UserCreatedNotification(userCreated), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.UserProfileUpdatedDomainEvent profileUpdated:
                        await _mediator.Publish(new TalentFlow.Application.Users.Events.UserProfileUpdatedNotification(profileUpdated), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.CourseCreatedDomainEvent courseCreated:
                        await _mediator.Publish(new TalentFlow.Application.Courses.Events.CourseCreatedNotification(new TalentFlow.Application.Courses.Events.CourseCreatedEvent(courseCreated.Course.Id)), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.CourseEnrollmentDomainEvent enrollmentEvent:
                        await _mediator.Publish(new TalentFlow.Application.Courses.Events.CourseEnrollmentNotification(new TalentFlow.Application.Enrollments.Events.CourseEnrollmentEvent(enrollmentEvent.Enrollment.Id, enrollmentEvent.Course.Id, enrollmentEvent.User.Id)), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.NotificationSentDomainEvent notificationSent:
                        await _mediator.Publish(new TalentFlow.Application.Notifications.Events.NotificationSentEvent(notificationSent.Notification.Id), cancellationToken);
                        break;

                    case INotification notification:
                        await _mediator.Publish(notification, cancellationToken);
                        break;

                    default:
                        break;
                }
            }


            return result;
        }
    }
}
