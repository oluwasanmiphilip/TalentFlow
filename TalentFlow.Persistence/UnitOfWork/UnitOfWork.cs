using MediatR;
using TalentFlow.Application.Assessments.Events;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TalentFlowDbContext _context;
        private readonly IMediator _mediator;

        public UnitOfWork(
            TalentFlowDbContext context,
            IMediator mediator,
            IRoleRepository roleRepository,
            IAuditLogRepository auditLogRepository
        )
        {
            _context = context;
            _mediator = mediator;
            Roles = roleRepository;
            AuditLogs = auditLogRepository;
        }

        // Repositories exposed via UnitOfWork
        public IRoleRepository Roles { get; }
        public IAuditLogRepository AuditLogs { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context.SaveChangesAsync(cancellationToken);

            // Collect domain events
            var entitiesWithEvents = _context.ChangeTracker
                .Entries<EntityBase>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            // Publish domain events via MediatR
            foreach (var domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case TalentFlow.Domain.Events.UserCreatedEvent userCreated:
                        var appEvent = new TalentFlow.Application.Users.Events.UserCreatedEvent(userCreated.User.Id);
                        await _mediator.Publish(new TalentFlow.Application.Users.Events.UserCreatedNotification(appEvent), cancellationToken);
                        break;


                    case TalentFlow.Domain.Events.UserProfileUpdatedEvent profileUpdated:
                        await _mediator.Publish(new TalentFlow.Application.Users.Events.UserProfileUpdatedNotification(profileUpdated), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.CourseCreatedEvent courseCreated:
                        await _mediator.Publish(new TalentFlow.Application.Courses.Events.CourseCreatedNotification(
                            new TalentFlow.Application.Courses.Events.CourseCreatedEvent(courseCreated.Course.Id)), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.CourseEnrollmentDomainEvent enrollmentEvent:
                        await _mediator.Publish(new TalentFlow.Application.Courses.Events.CourseEnrollmentNotification(
                            new TalentFlow.Application.Enrollments.Events.CourseEnrollmentEvent(
                                enrollmentEvent.Enrollment.Id, enrollmentEvent.Course.Id, enrollmentEvent.User.Id)), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.NotificationSentEvent notificationSent:
                        await _mediator.Publish(new TalentFlow.Application.Notifications.Events.NotificationSentEvent(notificationSent.Notification.Id), cancellationToken);
                        break;

                    case TalentFlow.Domain.Events.AssessmentCreatedDomainEvent assessmentCreated:
                        {
                            var assessmentCreatedEvent = new TalentFlow.Application.Assessments.Events.AssessmentCreatedEvent(
                                assessmentCreated.Assessment.Id
                            );
                            await _mediator.Publish(assessmentCreatedEvent, cancellationToken);
                            break;
                        }


                    case TalentFlow.Domain.Events.QuestionAddedDomainEvent questionAdded:
                        await _mediator.Publish(new QuestionAddedNotification(questionAdded), cancellationToken);
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
