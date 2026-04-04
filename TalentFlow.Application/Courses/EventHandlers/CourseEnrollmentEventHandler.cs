using MediatR;
using Microsoft.Extensions.Logging;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Enrollments.Events;
using TalentFlow.Application.Notifications;

namespace TalentFlow.Application.Enrollments.Handlers
{
    public class CourseEnrollmentEventHandler : INotificationHandler<CourseEnrollmentEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<CourseEnrollmentEventHandler> _logger;

        public CourseEnrollmentEventHandler(
            INotificationService notificationService,
            ILogger<CourseEnrollmentEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(CourseEnrollmentEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling CourseEnrollmentEvent: EnrollmentId={EnrollmentId}, CourseId={CourseId}, UserId={UserId}, OccurredOn={OccurredOn}",
                notification.EnrollmentId,
                notification.CourseId,
                notification.UserId,
                notification.OccurredOn);

            // Trigger a notification for the enrollment
            await _notificationService.SendNotificationAsync(notification.EnrollmentId, cancellationToken);
        }
    }
}
